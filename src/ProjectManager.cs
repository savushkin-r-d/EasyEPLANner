using Device;
using Editor;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
            string path = GetPtusaProjectsPath(projectName) + projectName + fileName;

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
        public string GetPtusaProjectsPath(string projectName)
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
                string projectsFolders =
                    iniFile.ReadString("path", "folder_path", "");
                string[] projectsFolderArray = projectsFolders.Split(';');
                string projectsFolder = "";
                bool firstPathIsSaved = false;
                string firstPath = "";
                foreach (string pathFromArray in projectsFolderArray)
                {
                    if (pathFromArray != "")
                    {
                        if (firstPathIsSaved == false)
                        {
                            firstPath = pathFromArray;
                            firstPathIsSaved = true;
                        }
                        projectsFolder = pathFromArray;
                        if (projectsFolder.Last() != '\\')
                        {
                            projectsFolder += '\\';
                        }
                        string projectsPath = projectsFolder + projectName;
                        if (Directory.Exists(projectsPath))
                        {
                            return projectsFolder;
                        }
                    }
                }

                if (firstPathIsSaved == false && firstPath == "")
                {
                    MessageBox.Show("Путь к каталогу с проектами не найден.\n" +
                        "Пожалуйста, проверьте конфигурацию!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    return firstPath + '\\';
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
                projectConfiguration.ReadIO();
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
                projectConfiguration.ReadBinding();
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
        public void Init(IEditor editor, ITechObjectManager techObjectManager, 
            ILog log, IOManager IOManager, DeviceManager deviceManager,
            ProjectConfiguration projectConfiguration)
        {
            this.editor = editor;
            this.techObjectManager = techObjectManager;
            this.log = log;
            
            this.IOManager = IOManager;
            this.deviceManager = deviceManager;
            this.projectConfiguration = projectConfiguration;
        }

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
        /// Обновление подписей к клеммам модулей IO
        /// в соответствии с актуальным названием устройств.
        /// </summary>
        public void UpdateModulesBinding()
        {
            var errors = "";
            try
            {
                log.Clear();
                log.ShowLog();
                log.AddMessage("Выполняется синхронизация..");
                errors = ModulesBindingUpdate.GetInstance().Execute();
                log.Clear();
            }
            catch (System.Exception ex)
            {
                log.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (errors != "")
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

                XMLReporter.SaveAsXML(par);
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

        /// <summary>
        /// Отключить подсветку устройств
        /// </summary>
        /// <param name="isClosingProject">Флаг закрытия проекта</param>
        public void RemoveHighLighting(bool isClosingProject = false)
        {
            foreach (object obj in highlightedObjects)
            {
                var drawedObject = obj as Eplan.EplApi.DataModel.Graphics
                    .GraphicalPlacement;
                if (isClosingProject)
                {
                    drawedObject.SmartLock();
                }
                drawedObject.Remove();
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

                mainIOFileWriter.Write(deviceManager.SaveAsLuaTable(""));

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

                mainTechDevicesFileWriter.Write(deviceManager.SaveDevicesAsLuaScript());

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
                prgFileWriter.WriteLine(string.Format("--Базовая функциональность\n{0}\n{1}\n{2}\n{3}\n", 
                    "require( \"tank\" )", 
                    "require(\"mixer\")", 
                    "require(\"line\")", 
                    "require(\"master\")"));
                prgFileWriter.WriteLine("-- Основные объекты проекта (объекты, описанные в Eplan'е).");
                prgFileWriter.WriteLine(PrgLuaSaver.Save("\t"));

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

        private IEditor editor; /// Редактор технологических объектов.
        private ITechObjectManager techObjectManager; /// Менеджер технологических объектов.
        private ILog log;

        private IOManager IOManager; /// Менеджер модулей ввода/вывода
        private DeviceManager deviceManager; // Менеджер устройств
        private ProjectConfiguration projectConfiguration; // Конфигурация проекта

        private static ProjectManager instance; /// Экземпляр класса.         
    }
}
