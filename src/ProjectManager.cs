///@file ProjectManager.cs
///@brief Классы, реализующие минимальную функциональность, необходимую для 
///экспорта описания проекта для PAC.
///
/// @author  Иванюк Дмитрий Сергеевич.
///
/// @par Текущая версия:
/// @$Rev: --- $.\n
/// @$Author: sedr $.\n
/// @$Date:: 2019-10-21#$.
/// 

using Device;
using Editor;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Менеджер проекта.
    /// </summary>
    public class ProjectManager
    {
        /// <summary>
        /// Вычисление CRC16 кода строки.
        /// </summary>
        public static ushort CRC16(string str)
        {
            byte[] pcBlock = System.Text.Encoding.Default.GetBytes(str);
            int len = pcBlock.GetLength(0);

            ushort crc = 0xFFFF;

            ushort idx = 0;
            while (len-- > 0)
            {
                crc ^= (ushort)(pcBlock[idx++] << 8);

                for (int i = 0; i < 8; i++)
                    crc = (ushort)((crc & 0x8000) > 0 ?
                         ((crc << 1) ^ 0x1021) : (crc << 1));
            }

            return crc;
        }

        /// <summary>
        /// Добавление сообщения в лог.
        /// </summary>
        public void AddLogMessage(string msg)
        {
            log.AddMessage(msg);
        }

        /// <summary>
        /// Установление прогресса.
        /// </summary>
        public void SetLogProgress(int msg)
        {
            log.SetProgress(msg);
        }

        /// <summary>
        /// Отображение окна сообщений лога.
        /// </summary>
        public void ShowLog()
        {
            log.ShowLog();
        }

        /// <summary>
        /// Сохранение описания в виде скрипта Lua.
        /// </summary>
        public void SaveAsLua(string PAC_Name, string path, bool silentMode)
        {
            Params param = new Params(PAC_Name, path, silentMode);

            if (silentMode)
            {
                SaveAsLuaThread(param);
            }
            else
            {
                System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(SaveAsLuaThread));

                t.Start(param);
            }
        }

        /// <summary>
        /// Считывание описания.
        /// </summary>
        private int LoadDescriptionFromFile(out string LuaStr,
            out string errStr, string projectName, string fileName)
        {
            LuaStr = "";
            errStr = "";

            StreamReader sr = null;
            string path = GetPtusaProjectsPath() + projectName + fileName;

            try
            {
                if (!File.Exists(path))
                {
                    errStr = "Файл описания проекта \"" + path + "\" отсутствует!" +
                        " Создано пустое описание.";
                    return 1;
                }
            }
            catch (DriveNotFoundException)
            {
                errStr = "Укажите правильные настройки каталога!";
                return 1;
            }

            sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding(1251));
            LuaStr = sr.ReadToEnd();
            sr.Close();

            return 0;
        }

        /// <summary>
        /// Путь к файлам .lua (к проекту)
        /// </summary>
        /// <returns></returns>
        public string GetPtusaProjectsPath()
        {
            try
            {
                // Поиск пути к каталогу с надстройкой
                string[] originalAssemblypath = AddInModule.OriginalAssemblyPath.Split('\\');
                string configFileName = "configuration.ini";

                int sourceEnd = originalAssemblypath.Length - 1;
                string path = @"";
                for (int source = 0; source < sourceEnd; source++)
                {
                    path += originalAssemblypath[source].ToString() + "\\";
                }
                path += configFileName;

                // Поиск файла .ini
                if (!File.Exists(path))
                {
                    // Если не нашли - создаем новый и записываем дефолтные данные
                    new PInvoke.IniFile(path);
                    StreamWriter sr = new StreamWriter(path, true);
                    sr.WriteLine("[path]\nfolder_path=");
                    sr.Close();
                    sr.Flush();
                }
                PInvoke.IniFile iniFile = new PInvoke.IniFile(path);

                // Считывание и возврат пути каталога проектов
                string projectsFolder = iniFile.ReadString("path", "folder_path", "");
                if (projectsFolder.Last() == '\\')
                {
                    return projectsFolder;
                }
                else
                {
                    return projectsFolder + "\\";
                }           
            }
            catch
            {
                MessageBox.Show("Файл конфигурации не найден - будет создан новый со стандартным описанием." +
                    " Пожалуйста, измените путь к каталогу с проектами, где хранятся Lua файлы!", 
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return "";
        }

        /// <summary>
        /// Считывание описания.
        /// </summary>
        public int LoadDescription(out string errStr,
            string projectName, bool loadFromLua)
        {
            errStr = "";
            log.Clear();

            string LuaStr;
            int res = 0;

            Eplan.EplApi.Base.Progress oProgress =
                new Eplan.EplApi.Base.Progress("EnhancedProgress");
            oProgress.SetAllowCancel(false);
            oProgress.SetTitle("Считывание данных проекта");

            try
            {
                oProgress.BeginPart(15, "Считывание IO");
                iOManager.ReadConfiguration();
                oProgress.EndPart();

                oProgress.BeginPart(15, "Считывание устройств");
                if (projectConfiguration.DevicesIsRead == true)
                {
                    projectConfiguration.SynchronizeDevices();
                }
                else
                {
                    projectConfiguration.ReadDevices();
                }
                oProgress.EndPart();

                oProgress.BeginPart(25, "Считывание привязки устройств");
                deviceManager.ReadConfigurationFromIOModules();
                oProgress.EndPart();

                if (loadFromLua)
                {
                    oProgress.BeginPart(15, "Считывание технологических объектов");
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, projectName, "\\main.objects.lua");
                    techObjectManager.LoadFromLuaStr(LuaStr, projectName);
                    errStr = "";
                    LuaStr = "";
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, projectName, "\\main.restrictions.lua");
                    techObjectManager.LoadRestriction(LuaStr);
                    oProgress.EndPart();
                }

                oProgress.BeginPart(15, "Проверка данных");
                projectConfiguration.Check();
                oProgress.EndPart();

                oProgress.BeginPart(15, "Расчет IO-Link");
                IOManager.CalculateIOLinkAdresses();
                oProgress.EndPart(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                oProgress.EndPart(true);
            }

            return res;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        public void Init(IIOManager iOManager, IDeviceManager deviceManager,
            IEditor editor, ITechObjectManager techObjectManager, ILog log,
            IOManager IOManager, DeviceManager DeviceManager,
            ProjectConfiguration projectConfiguration)
        {
            this.iOManager = iOManager;
            this.deviceManager = deviceManager;
            this.editor = editor;
            this.techObjectManager = techObjectManager;
            this.log = log;
            
            //TODO: скорректировать метод не забыть.
            this.IOManager = IOManager;
            this.DeviceManager = DeviceManager;
            this.projectConfiguration = projectConfiguration;
        }

        /// <summary>
        /// Узлы, в которых устанавливается протоколирование элементов.
        /// </summary>
        private static HashSet<string> Protocol =
            new HashSet<string>(new string[] { "TE_V", "QT_V", "FQT_F", "PT_V", "LT_V", "AO_V", "VC_V", "AI_V", "M_V", "LT_CLEVEL" });

        /// <summary>
        /// Узлы, в которых устанавливается опрос по времени.
        /// </summary>
        private static HashSet<string> Period =
            new HashSet<string>(new string[] { "TE_V", "QT_V", "LT_V", "PT_V", "AO_V", "AI_V", "FQT_F", "M_V", "VC_V", "LT_CLEVEL" });

        /// <summary>
        /// Сохранение описания в виде таблицы Excel.
        /// </summary>
        public void SaveAsExcelDescription(string path)
        {
            System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(ExportToExcel));

            t.Start(path);
        }

        /// <summary>
        /// Экспорт технологических объектов в Excel.
        /// </summary>
        /// <param name="path">Путь к директории сохранения</param>
        /// <param name="projectName">Имя проекта</param>
        private void ExportToExcel(object param)
        {
            string par = param as string;
            log.ShowLog();

            log.DisableOkButton();
            log.Clear();
            log.SetProgress(0);

            try
            {

                log.SetProgress(1);

                ExcelRepoter.ExportTechDevs(par);

                log.AddMessage("Done.");
            }

            catch (System.Exception ex)
            {
                log.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (log != null)
                {
                    log.EnableOkButton();
                    log.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Вызов синхронизации названий устройств и модулей.
        /// устройств и модулей
        /// </summary>
        public void UpdateModulesBinding()
        {
            var errors = string.Empty;
            try
            {
                log.Clear();
                log.ShowLog();
                log.AddMessage("Выполняется синхронизация..");
                errors = EplanIOManager.GetInstance().UpdateModulesBinding();
                log.Clear();
            }
            catch (System.Exception ex)
            {
                log.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (errors != string.Empty)
                {
                    log.AddMessage(errors);
                }

                if (log != null)
                {
                    log.AddMessage("Синхронизация завершена. ");
                    log.SetProgress(100);
                    log.EnableOkButton();
                }
            }
        }

        /// <summary>
        /// Экспорт из проекта базы каналов.
        /// </summary>
        public void SaveAsCDBX(string projectName, bool combineTag = false)
        {
            techObjectManager.SetCDBXTagView(combineTag);
            System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(SaveAsXMLThread));

            t.Start(projectName);

        }

        private void SaveAsXMLThread(object param)
        {
            string par = param as string;

            log.ShowLog();

            log.DisableOkButton();
            log.Clear();
            log.SetProgress(0);

            try
            {
                log.SetProgress(1);

                SaveAsXML(par);
                log.SetProgress(50);

                log.AddMessage("Done.");
            }

            catch (System.Exception ex)
            {
                log.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (log != null)
                {
                    log.EnableOkButton();
                    log.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Экспорт проекта в базу каналов.
        /// </summary>
        /// <param name="prjName">Имя проекта</param>
        private void SaveAsXML(string path)
        {
            projectConfiguration.SynchronizeDevices();

            TreeNode rootNode = new TreeNode("subtypes");
            techObjectManager.GetObjectForXML(rootNode);
            DeviceManager.GetObjectForXML(rootNode);

            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path))
            {
                XmlTextWriter textWritter = new XmlTextWriter(path,
                    System.Text.Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("driver");
                textWritter.WriteAttributeString("xmlns", "driver", null,
                    "http://brestmilk.by/driver/");
                textWritter.WriteEndElement();
                textWritter.Close();
                xmlDoc.Load(path);

                XmlElement subtypesNode = WriteCommonXMLPart(xmlDoc);
                CreateNewChannels(xmlDoc, subtypesNode, rootNode.Nodes);
            }
            else
            {
                xmlDoc.Load(path);
                XmlNamespaceManager nsmgr =
                    new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("driver", "http://brestmilk.by/driver/");
                XmlElement root = xmlDoc.DocumentElement;
                XmlElement elm =
                    root.SelectSingleNode("//driver:id", nsmgr) as XmlElement;
                string baseId = elm.InnerText;
                elm = root.SelectSingleNode("//driver:subtypes", nsmgr) as XmlElement;
                nsmgr.AddNamespace("subtypes", "http://brestmilk.by/subtypes/");
                List<string> subtupesId = new List<string>();

                for (int i = 0; i < 256; i++)
                {
                    subtupesId.Add(i.ToString());
                }

                foreach (XmlElement item in elm.ChildNodes)
                {
                    if (subtupesId.Contains(item.ChildNodes[0].InnerText))
                    {
                        subtupesId.Remove(item.ChildNodes[0].InnerText);
                    }

                    if (!item.ChildNodes[6].InnerText.Contains("PID"))
                    {
                        TreeNode[] nodes = rootNode.Nodes.Cast<TreeNode>().Where(r => r.Text == item.ChildNodes[6].InnerText).ToArray();
                        if (nodes.Length == 0)
                        {
                            // нужно закомментировать неиспользующиеся узлы
                            item.ChildNodes[3].InnerText = "0";
                        }
                        else
                        {
                            foreach (XmlElement chan in item.ChildNodes[9].ChildNodes)
                            {
                                foreach (TreeNode node in nodes)
                                {
                                    TreeNode[] chanNodes = node.Nodes.Find(chan.ChildNodes[4].InnerText, true);
                                    if (chanNodes.Length == 0)
                                    {
                                        chan.ChildNodes[3].InnerText = "0";
                                        break;
                                    }
                                    else
                                    {
                                        chan.ChildNodes[3].InnerText = "-1";
                                    }
                                }
                            }
                        }
                    }

                }

                foreach (TreeNode subtype in rootNode.Nodes)
                {
                    string xpath = "//subtypes:subtype[subtypes:sdrvname='" +
                        subtype.Text + "']";
                    XmlElement subElm =
                        elm.SelectSingleNode(xpath, nsmgr) as XmlElement;
                    if (subElm != null)
                    {
                        nsmgr.AddNamespace("channels", "http://brestmilk.by/channels/");

                        XmlElement channelsElm = subElm.ChildNodes[9] as XmlElement;

                        List<long> channelsId = new List<long>();
                        foreach (TreeNode channel in subtype.Nodes)
                        {
                            string xpathChan = xpath + "//channels:channel[channels:descr='" +
                                channel.Text + "']";

                            if (channelsElm.SelectSingleNode(xpathChan, nsmgr) == null)
                            {

                                //нахождение адреса канала среди свободных
                                if (channelsId.Count == 0)
                                {
                                    long beginId = long.Parse(
                                        (System.Int64.Parse(baseId).ToString("X2") +
                                      System.Int64.Parse(
                                      subElm.ChildNodes[0].InnerText).ToString("X2") + "0000"),
                                      System.Globalization.NumberStyles.HexNumber);
                                    for (int i = 0; i < 65535; i++)
                                    {
                                        channelsId.Add(beginId + i);
                                    }
                                    foreach (XmlElement channId in channelsElm.ChildNodes)
                                    {
                                        long id = System.Int64.Parse(
                                            channId.FirstChild.InnerText);
                                        if (channelsId.Contains(id))
                                        {
                                            channelsId.Remove(id);
                                        }
                                    }
                                }

                                long channelId = channelsId[0];
                                channelsId.RemoveAt(0);
                                AddChannel(xmlDoc, channel,
                                    channelsElm, channelId);
                            }
                        }
                    }
                    else
                    {
                        if (subtupesId.Count > 0)
                        {
                            long subtypeId = System.Int64.Parse(subtupesId[0]);
                            subtupesId.RemoveAt(0);
                            XmlElement newSubtype =
                                AddSubType(xmlDoc, elm, subtype, subtypeId);
                            string hex = System.Int64.Parse(baseId).ToString("X2") +
                                subtypeId.ToString("X2");
                            for (int i = 0; i < subtype.Nodes.Count; i++)
                            {
                                long channelId = System.Int64.Parse((hex +
                                    i.ToString("X4")),
                                    System.Globalization.NumberStyles.HexNumber);
                                AddChannel(xmlDoc, subtype.Nodes[i],
                                    newSubtype, channelId);
                            }
                        }
                        else
                        {
                            log.AddMessage("Превышено количество подтипов в базе каналов.");
                            return;
                        }
                    }
                }
            }

            xmlDoc.Save(path);

        }

        /// <summary>
        /// Создание узлов и каналов в новой пустой базе каналов
        /// </summary>
        private static void CreateNewChannels(XmlDocument xmlDoc,
            XmlElement subtypesNode, TreeNodeCollection Nodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                XmlElement subtypeElm = AddSubType(xmlDoc, subtypesNode,
                    Nodes[i], i);

                for (int j = 0; j < Nodes[i].Nodes.Count; j++)
                {
                    long channelId = long.Parse(("01" +
                        i.ToString("X2") + j.ToString("X4")),
                        System.Globalization.NumberStyles.HexNumber);
                    AddChannel(xmlDoc, Nodes[i].Nodes[j],
                        subtypeElm, channelId);
                }
            }
        }

        /// <summary>
        /// Формирование общей структуры базы каналов
        /// </summary>
        private XmlElement WriteCommonXMLPart(XmlDocument xmlDoc)
        {
            string nsDriver = "http://brestmilk.by/driver/";
            string prefixDriver = "driver";
            XmlElement firstLevel = xmlDoc.CreateElement(prefixDriver, "inf", nsDriver);
            firstLevel.InnerText = "BASE F11F3DCC-09F8-4D04-BCB7-81D5D7C48C78";
            xmlDoc.DocumentElement.AppendChild(firstLevel);
            firstLevel = xmlDoc.CreateElement(prefixDriver, "dbbuild", nsDriver);
            firstLevel.InnerText = "4";
            xmlDoc.DocumentElement.AppendChild(firstLevel);
            firstLevel = xmlDoc.CreateElement(prefixDriver, prefixDriver, nsDriver);
            xmlDoc.DocumentElement.AppendChild(firstLevel);

            XmlElement secondLevel = xmlDoc.CreateElement(prefixDriver, "id", nsDriver);
            secondLevel.InnerText = "1";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "tid", nsDriver);
            secondLevel.InnerText = "0";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "dllname", nsDriver);
            secondLevel.InnerText = "PAC_easy_drv_LZ.dll";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "access", nsDriver);
            secondLevel.InnerText = "2";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "maxsubtypescount", nsDriver);
            secondLevel.InnerText = "10";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "enabled", nsDriver);
            secondLevel.InnerText = "-1";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "descr", nsDriver);
            secondLevel.InnerText = "Система PLC-X1";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "drvname", nsDriver);
            secondLevel.InnerText = Path.GetFileNameWithoutExtension(xmlDoc.BaseURI);
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "defname", nsDriver);
            secondLevel.InnerText = "Opc Driver";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "defdescr", nsDriver);
            secondLevel.InnerText = "Универсальный драйвер для протоколов Modbus и SNMP";
            firstLevel.AppendChild(secondLevel);
            secondLevel = xmlDoc.CreateElement(prefixDriver, "communication", nsDriver);
            secondLevel.SetAttribute(
                "xmlns:communication", "http://brestmilk.by/communication/");
            firstLevel.AppendChild(secondLevel);

            string nsParam = "http://brestmilk.by/parameters/";
            string pefixParam = "parameters";
            XmlElement thirdLevel = xmlDoc.CreateElement(
                "communication", pefixParam, "http://brestmilk.by/communication/");
            thirdLevel.SetAttribute("xmlns:parameters", nsParam);
            secondLevel.AppendChild(thirdLevel);

            XmlElement forthLevel = xmlDoc.CreateElement(
                pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            XmlElement fifthLevel =
                xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "TYPE";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "COM";
            forthLevel.AppendChild(fifthLevel);

            forthLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "PORTNAME";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "COM4";
            forthLevel.AppendChild(fifthLevel);

            forthLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "SPEED";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "12";
            forthLevel.AppendChild(fifthLevel);

            forthLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "PARITY";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "0";
            forthLevel.AppendChild(fifthLevel);

            forthLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "DATABITS";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "4";
            forthLevel.AppendChild(fifthLevel);

            forthLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            thirdLevel.AppendChild(forthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "STOPBITS";
            forthLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "0";
            forthLevel.AppendChild(fifthLevel);

            secondLevel = xmlDoc.CreateElement(prefixDriver, "init_parameters",
                nsDriver);
            secondLevel.SetAttribute("xmlns:parameters", nsParam);
            firstLevel.AppendChild(secondLevel);

            thirdLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            secondLevel.AppendChild(thirdLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "IP";
            thirdLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "IP127.0.0.1";
            thirdLevel.AppendChild(fifthLevel);

            thirdLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            secondLevel.AppendChild(thirdLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "PLC_NAME";
            thirdLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            if (string.IsNullOrEmpty(EProjectManager.GetInstance().GetCurrentProjectName()))
            {
                log.AddMessage("Не задано PLC_NAME.");
                fifthLevel.InnerText = "PLC_NAME";
            }
            else
            {
                string projectName = EProjectManager.GetInstance().GetCurrentProjectName();
                EProjectManager.GetInstance().CheckProjectName(ref projectName);
                fifthLevel.InnerText = projectName;
            }


            thirdLevel.AppendChild(fifthLevel);

            thirdLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            secondLevel.AppendChild(thirdLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "PORT";
            thirdLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "10000";
            thirdLevel.AppendChild(fifthLevel);

            thirdLevel = xmlDoc.CreateElement(pefixParam, "parameter", nsParam);
            secondLevel.AppendChild(thirdLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "name", nsParam);
            fifthLevel.InnerText = "Kontroller";
            thirdLevel.AppendChild(fifthLevel);
            fifthLevel = xmlDoc.CreateElement(pefixParam, "value", nsParam);
            fifthLevel.InnerText = "LINUX";
            thirdLevel.AppendChild(fifthLevel);

            secondLevel = xmlDoc.CreateElement(
                prefixDriver, "common_parameters", nsDriver);
            secondLevel.SetAttribute("xmlns:parameters", nsParam);
            firstLevel.AppendChild(secondLevel);

            secondLevel = xmlDoc.CreateElement(
                prefixDriver, "final_parameters", nsDriver);
            secondLevel.SetAttribute("xmlns:parameters", nsParam);
            firstLevel.AppendChild(secondLevel);

            secondLevel = xmlDoc.CreateElement(prefixDriver, "subtypes", nsDriver);
            secondLevel.SetAttribute("xmlns:subtypes",
                "http://brestmilk.by/subtypes/");
            firstLevel.AppendChild(secondLevel);
            return secondLevel;
        }

        /// <summary>
        /// Добавление канала с указанным адресом
        /// </summary>
        private static void AddChannel(XmlDocument xmlDoc, TreeNode Node,
            XmlElement subtypeElm, long channelId)
        {
            string subtypeName = subtypeElm.ParentNode.ChildNodes[6].InnerText;
            bool needSetPeriod =
                Period.Contains(subtypeName);
            bool needSetProtocol =
                Protocol.Contains(subtypeName) ||
                Node.Text.Contains("OBJECT") && (Node.Text.Contains("ST") ||
                Node.Text.Contains("MODES") || Node.Text.Contains("OPERATIONS")) && (!Node.Text.Contains("STEPS"));
            string prefixChannels = "channels";
            string nsChannels = "http://brestmilk.by/channels/";
            XmlElement channel = xmlDoc.CreateElement(prefixChannels, "channel", nsChannels);
            subtypeElm.AppendChild(channel);
            XmlElement channelElm = xmlDoc.CreateElement(prefixChannels, "id", nsChannels);
            channelElm.InnerText = channelId.ToString();
            channel.AppendChild(channelElm);
            if (needSetPeriod)
            {
                channelElm = xmlDoc.CreateElement(prefixChannels, "requesttype", nsChannels);
                channelElm.InnerText = "0";
                channel.AppendChild(channelElm);
                channelElm = xmlDoc.CreateElement(prefixChannels, "requestperiod", nsChannels);
                if (!subtypeName.Contains("LE"))
                {
                    channelElm.InnerText = "3000";
                }
                else
                {
                    channelElm.InnerText = "5000";
                }
            }
            else
            {
                channelElm = xmlDoc.CreateElement(
                    prefixChannels, "requesttype", nsChannels);
                channelElm.InnerText = "1";
                channel.AppendChild(channelElm);
                channelElm = xmlDoc.CreateElement(
                    prefixChannels, "requestperiod", nsChannels);
                channelElm.InnerText = "1";
            }
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "enabled", nsChannels);
            channelElm.InnerText = "-1";
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "descr", nsChannels);
            channelElm.InnerText = Node.Text;
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "delta", nsChannels);
            if (needSetPeriod)
            {
                if (!subtypeName.Contains("QT"))
                {
                    channelElm.InnerText = "0.2";
                }
                else
                {
                    channelElm.InnerText = "0.3";
                }
            }
            else
            {
                channelElm.InnerText = "0";
            }
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "apptime", nsChannels);
            channelElm.InnerText = "0";
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "protocol", nsChannels);
            if (needSetProtocol)
            {
                channelElm.InnerText = "-1";
            }
            else
            {
                channelElm.InnerText = "0";
            }
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "transexprin", nsChannels);
            channelElm.AppendChild(xmlDoc.CreateCDataSection(""));
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "transexprout", nsChannels);
            channelElm.AppendChild(xmlDoc.CreateCDataSection(""));
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefixChannels, "channel_parameters", nsChannels);
            channelElm.SetAttribute("xmlns:parameters", "http://brestmilk.by/parameters/");
            if (Node.Text.Contains("UP_TIME") || Node.Text.Contains("CMD_ANSWER"))
            {
                AddChannelAtribute(xmlDoc, channelElm, "IsString");
            }
            channel.AppendChild(channelElm);
        }


        private static void AddChannelAtribute(XmlDocument xmlDoc, XmlElement elm, string atribute)
        {
            string prefixParams = "parameters";
            string nsParams = "http://brestmilk.by/parameters/";
            XmlElement parametersElm = xmlDoc.CreateElement(prefixParams, "channel", nsParams);
            elm.AppendChild(parametersElm);
            XmlElement parElm = xmlDoc.CreateElement(prefixParams, "name", nsParams);
            parElm.InnerText = atribute;
            parametersElm.AppendChild(parElm);
            parElm = xmlDoc.CreateElement(prefixParams, "value", nsParams);
            parElm.InnerText = "1";
            parametersElm.AppendChild(parElm);

        }

        /// <summary>
        /// Добавление узла в базу каналов
        /// </summary>
        private static XmlElement AddSubType(XmlDocument xmlDoc,
            XmlElement subtypesNode, TreeNode Node, long i)
        {
            string ns = "http://brestmilk.by/subtypes/";
            string prefixSubtypes = "subtypes";

            XmlElement subType = xmlDoc.CreateElement(prefixSubtypes, "subtype", ns);
            subtypesNode.AppendChild(subType);
            XmlElement subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "sid", ns);
            subtypeElm.InnerText = i.ToString();
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "stid", ns);
            subtypeElm.InnerText = "0";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "maxchannels", ns);
            subtypeElm.InnerText = "0";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "enabled", ns);
            subtypeElm.InnerText = "-1";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "descr", ns);
            subtypeElm.InnerText = "Описание";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "defdescr", ns);
            subtypeElm.InnerText = "Описание";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "sdrvname", ns);
            subtypeElm.InnerText = Node.Text;
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "sdrvdefname", ns);
            subtypeElm.InnerText = "Узел";
            subType.AppendChild(subtypeElm);

            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "common_parameters", ns);
            subtypeElm.SetAttribute("xmlns:parameters", "http://brestmilk.by/parameters/");
            subType.AppendChild(subtypeElm);

            subtypeElm = xmlDoc.CreateElement(prefixSubtypes, "channels", ns);
            subtypeElm.SetAttribute("xmlns:channels", "http://brestmilk.by/channels/");
            subType.AppendChild(subtypeElm);
            return subtypeElm;
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static ProjectManager GetInstance()
        {
            if (null == instance)
            {
                instance = new ProjectManager();
            }

            return instance;
        }

        /// <summary>
        /// Редактирование технологических объектов.
        /// </summary>
        /// <param name="objectsLuaStr">Скрипт Lua с текущими описанием.</param>        
        /// <returns>Результат редактирования.</returns>
        public string Edit()
        {
            string res = editor.Edit(techObjectManager as ITreeViewItem);

            return res;
        }

        //Участвующие в операции устройства, подсвеченные на карте Eplan.
        List<object> highlightedObjects = new List<object>();

        public void RemoveHighLighting()
        {
            foreach (object obj in highlightedObjects)
            {
                (obj as Eplan.EplApi.DataModel.Graphics.GraphicalPlacement).Remove();
            }

            highlightedObjects.Clear();
        }

        public void SetHighLighting(List<DrawInfo> objectsToDraw)
        {
            if (objectsToDraw == null)
            {
                return;
            }
            foreach (DrawInfo drawObj in objectsToDraw)
            {
                DrawInfo.Style howToDraw = drawObj.style;

                if (howToDraw == DrawInfo.Style.NO_DRAW)
                {
                    continue;
                }

                Eplan.EplApi.DataModel.Function oF =
                    (drawObj.dev as IODevice).EplanObjectFunction;
                if (oF == null)
                {
                    continue;
                }

                Eplan.EplApi.Base.PointD[] points = oF.GetBoundingBox();
                int number = 0;

                Eplan.EplApi.DataModel.Graphics.Rectangle rc =
                    new Eplan.EplApi.DataModel.Graphics.Rectangle();
                rc.Create(oF.Page);
                rc.IsSurfaceFilled = true;
                rc.DrawingOrder = -1;

                short colour = 0;
                switch (howToDraw)
                {
                    case DrawInfo.Style.GREEN_BOX:
                        colour = 3; //Green.

                        //Для сигналов подсвечиваем полностью всю линию.
                        if (oF.Name.Contains("DI") || oF.Name.Contains("DO"))
                        {
                            if (oF.Connections.Length > 0)
                            {
                                points[1].X =
                                oF.Connections[0].StartPin.ParentFunction.GetBoundingBox()[1].X;
                            }
                        }
                        break;

                    case DrawInfo.Style.RED_BOX:
                        colour = 252; //Red.
                        break;

                    case DrawInfo.Style.GREEN_UPPER_BOX:
                        points[0].Y += (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;

                    case DrawInfo.Style.GREEN_LOWER_BOX:
                        points[1].Y -= (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;

                    case DrawInfo.Style.GREEN_RED_BOX:
                        Eplan.EplApi.DataModel.Graphics.Rectangle rc2 =
                            new Eplan.EplApi.DataModel.Graphics.Rectangle();
                        rc2.Create(oF.Page);
                        rc2.IsSurfaceFilled = true;
                        rc2.DrawingOrder = 1;
                        rc2.SetArea(
                            new Eplan.EplApi.Base.PointD(
                                points[0].X, points[0].Y + (points[1].Y - points[0].Y) / 2),
                                points[1]);

                        rc2.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(
                            252 /*Red*/, -16002, -16002, -16002, 0);

                        rc2.Properties.set_PROPUSER_TEST(1, oF.ToStringIdentifier());
                        highlightedObjects.Add(rc2);

                        points[1].Y -= (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;
                }

                rc.SetArea(points[0], points[1]);
                rc.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(
                    colour, -16002, -16002, -16002, 0);

                if (number != 0)
                {
                    Eplan.EplApi.DataModel.Graphics.Text txt =
                        new Eplan.EplApi.DataModel.Graphics.Text();
                    txt.Create(oF.Page, number.ToString(), 3);
                    txt.Location = new Eplan.EplApi.Base.PointD(
                        points[1].X, points[1].Y);
                    txt.Justification =
                        Eplan.EplApi.DataModel.Graphics.TextBase.JustificationType.BottomRight;
                    txt.TextColorId = 0;

                    highlightedObjects.Add(txt);
                }

                rc.Properties.set_PROPUSER_TEST(1, oF.ToStringIdentifier());
                highlightedObjects.Add(rc);
            }
        }

        private ProjectManager()
        {
        }

        private const int MAIN_IO_FILE_VERSION = 1;
        private const int MAIN_TECH_OBJECTS_FILE_VERSION = 1;
        private const int MAIN_TECH_DEVICES_FILE_VERSION = 1;
        private const int MAIN_RESTRICTIONS_FILE_VERSION = 1;
        private const int MAIN_PRG_FILE_VERSION = 1;

        private const string MAIN_IO_FILE_NAME = "main.io.lua";
        private const string MAIN_WAGO_FILE_NAME = "main.wago.lua";
        private const string MAIN_TECH_OBJECTS_FILE_NAME = "main.objects.lua";
        private const string MAIN_TECH_DEVICES_FILE_NAME = "main.devices.lua";
        private const string MAIN_RESTRICTIONS_FILE_NAME = "main.restrictions.lua";

        private const string MAIN_FILE_NAME = "main.plua";
        private const string MAIN_MODBUS_SRV_FILE_NAME = "main.modbus_srv.lua";
        private const string MAIN_PROFIBUS_FILE_NAME = "main.profibus.lua";
        private const string MAIN_PRG_FILE_NAME = "prg.lua";

        private class Params
        {
            public string PAC_Name;
            public string path;
            public bool silentMode;

            public Params(string PAC_Name, string path, bool silentMode)
            {
                this.PAC_Name = PAC_Name;
                this.path = path;
                this.silentMode = silentMode;
            }
        }

        private void SaveAsLuaThread(object param)
        {
            Params par = param as Params;

            StreamWriter mainIOFileWriter = null;
            StreamWriter mainTechObjectsFileWriter = null;
            StreamWriter mainTechDevicesFileWriter = null;

            StreamWriter mainRestrictionsFileWriter = null;
            StreamWriter mainFileWriter = null;
            StreamWriter prgFileWriter = null;

            if (!par.silentMode)
            {
                log.ShowLog();
                log.DisableOkButton();
                log.SetProgress(0);
            }

            try
            {
                try
                {
                    if (!Directory.Exists(par.path))
                    {
                        Directory.CreateDirectory(par.path);
                    }
                }
                catch (DriveNotFoundException)
                {
                    if (!par.silentMode)
                    {
                        log.AddMessage("Ошибка подключения к диску с проектами. Подключите диск!");
                        log.SetProgress(100);
                    }
                    return;
                }

                string FILE_NAME = par.path + @"\" + MAIN_IO_FILE_NAME;
                mainIOFileWriter = new StreamWriter(FILE_NAME,
                    false, System.Text.Encoding.GetEncoding(1251));

                mainIOFileWriter.WriteLine("--version  = {0}", MAIN_IO_FILE_VERSION);
                mainIOFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainIOFileWriter.WriteLine("PAC_name       = \'{0}\'", par.PAC_Name);
                ushort crc = CRC16(par.PAC_Name);
                mainIOFileWriter.WriteLine("PAC_id         = \'{0}\'", crc);
                mainIOFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                if (par.silentMode == false)
                {
                    log.SetProgress(1);
                }

                mainIOFileWriter.Write(IOManager.SaveAsLuaTable(""));
                if (par.silentMode == false)
                {
                    log.SetProgress(50);
                }

                mainIOFileWriter.Write(DeviceManager.SaveAsLuaTable(""));

                string FILE_NAME2 = par.path + @"\" + MAIN_TECH_OBJECTS_FILE_NAME;
                mainTechObjectsFileWriter = new StreamWriter(FILE_NAME2,
                    false, System.Text.Encoding.GetEncoding(1251));

                mainTechObjectsFileWriter.WriteLine("--version  = {0}", MAIN_TECH_OBJECTS_FILE_VERSION);
                mainTechObjectsFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                mainTechObjectsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainTechObjectsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                string LuaStr = techObjectManager.SaveAsLuaTable("");
                mainTechObjectsFileWriter.WriteLine(LuaStr);

                string FILE_NAME3 = par.path + @"\" + MAIN_TECH_DEVICES_FILE_NAME;
                mainTechDevicesFileWriter = new StreamWriter(FILE_NAME3,
                    false, System.Text.Encoding.GetEncoding(1251));

                mainTechDevicesFileWriter.WriteLine("--version  = {0}", MAIN_TECH_DEVICES_FILE_VERSION);
                mainTechDevicesFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                mainTechDevicesFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainTechDevicesFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                mainTechDevicesFileWriter.Write(DeviceManager.SaveDevicesAsLuaScript());

                string FILE_NAME4 = par.path + @"\" + MAIN_RESTRICTIONS_FILE_NAME;
                mainRestrictionsFileWriter = new StreamWriter(FILE_NAME4,
                    false, System.Text.Encoding.GetEncoding(1251));

                mainRestrictionsFileWriter.WriteLine("--version  = {0}", MAIN_RESTRICTIONS_FILE_VERSION);
                mainRestrictionsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainRestrictionsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainRestrictionsFileWriter.Write(techObjectManager.SaveRestrictionAsLua(""));

                string mainFileName = par.path + @"\" + MAIN_FILE_NAME;
                if (!File.Exists(mainFileName))
                {
                    //Создаем пустое описание управляющей программы.
                    mainFileWriter = new StreamWriter(mainFileName,
                        false, System.Text.Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--Проект \'{0}\'", par.PAC_Name);

                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Пользовательская функция инициализации, выполняемая однократно в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function user_init()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Пользовательская функция, выполняемая каждый цикл в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function user_eval()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Функция инициализации параметров, выполняемая однократно в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function init_params()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string modbusSrvFileName = par.path + @"\" + MAIN_MODBUS_SRV_FILE_NAME;
                if (!File.Exists(modbusSrvFileName))
                {
                    //Создаем пустое описание сервера MODBUS.
                    mainFileWriter = new StreamWriter(modbusSrvFileName,
                        false, System.Text.Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--version  = 1");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string profibusFileName = par.path + @"\" + MAIN_PROFIBUS_FILE_NAME;
                if (!File.Exists(profibusFileName))
                {
                    //Создаем пустое описание конфигурации PROFIBUS.
                    mainFileWriter = new StreamWriter(profibusFileName,
                        false, System.Text.Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--version  = 1");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("system = system or { }");
                    mainFileWriter.WriteLine("system.init_profibus = function()");
                    mainFileWriter.WriteLine("end");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string FILE_NAME6 = par.path + @"\" + MAIN_PRG_FILE_NAME;
                prgFileWriter = new StreamWriter(FILE_NAME6,
                    false, System.Text.Encoding.GetEncoding(1251));

                prgFileWriter.WriteLine("--version  = {0}", MAIN_PRG_FILE_VERSION);
                prgFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                prgFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                prgFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                prgFileWriter.WriteLine("-- Основные объекты проекта (объекты, описанные в Eplan'е).");
                prgFileWriter.WriteLine(techObjectManager.SavePrgAsLuaTable("\t"));

                if (par.silentMode == false)
                {
                    if (!log.IsEmpty())
                    {
                        log.AddMessage("Done.");
                        log.ShowLastLine();
                    }
                    else
                    {
                        log.HideLog();
                    }
                }
            }

            catch (System.Exception ex)
            {
                if (par.silentMode == false)
                {
                    log.AddMessage("Exception - " + ex);
                    log.AddMessage("");
                    log.AddMessage("");
                    log.ShowLastLine();
                }
            }
            finally
            {
                if (mainIOFileWriter != null)
                {
                    mainIOFileWriter.Flush();
                    mainIOFileWriter.Close();

                    // Делаем копию с другим именем (IO.lua и WAGO.lua идентичный)
                    File.Copy(par.path + @"\" + MAIN_IO_FILE_NAME, 
                        par.path + @"\" + MAIN_WAGO_FILE_NAME, true);
                }

                if (mainTechObjectsFileWriter != null)
                {
                    mainTechObjectsFileWriter.Flush();
                    mainTechObjectsFileWriter.Close();
                }

                if (mainTechDevicesFileWriter != null)
                {
                    mainTechDevicesFileWriter.Flush();
                    mainTechDevicesFileWriter.Close();
                }

                if (mainRestrictionsFileWriter != null)
                {
                    mainRestrictionsFileWriter.Flush();
                    mainRestrictionsFileWriter.Close();
                }

                if (prgFileWriter != null)
                {
                    prgFileWriter.Flush();
                    prgFileWriter.Close();
                }

                if (!par.silentMode && log != null)
                {
                    log.EnableOkButton();
                    log.SetProgress(100);
                }
            }
        }

        private IIOManager iOManager;       /// Менеджер модулей ввода\вывода IO.
        private IDeviceManager deviceManager;   /// Менеджер устройств.
        private IEditor editor;                 /// Редактор технологических объектов.
        private ITechObjectManager techObjectManager; /// Менеджер технологических объектов.
        private ILog log;

        private IOManager IOManager; // Менеджер модулей ввода/вывода
        private DeviceManager DeviceManager; // Менеджер устройств
        private ProjectConfiguration projectConfiguration; // Конфигурация проекта

        private static ProjectManager instance;       /// Экземпляр класса.         
    }
}