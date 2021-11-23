using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using TechObject;
using Device;
using System.Text.RegularExpressions;
using System.Threading;

namespace EasyEPlanner
{
    public class XMLReporter
    {
        /// <summary>
        /// Экспорт из проекта базы каналов.
        /// </summary>
        public void SaveAsCDBX(string projectName, bool combineTag = false,
            bool useNewNames = false, bool rewrite = false)
        {
            Thread t = new Thread(
                new ParameterizedThreadStart(SaveAsXMLThread));

            var dataForSave = new DataForSaveAsXML(projectName, rewrite,
                combineTag, useNewNames);
            t.CurrentCulture = StaticHelper.CommonConst
                .CultureWithDotInsteadComma;
            t.Start(dataForSave);
        }

        /// <summary>
        /// Экспорт базы каналов, поток.
        /// </summary>
        /// <param name="param">Параметр потока</param>
        private void SaveAsXMLThread(object param)
        {
            var data = param as DataForSaveAsXML;

            Logs.Show();
            Logs.DisableButtons();
            Logs.Clear();
            Logs.SetProgress(0);

            try
            {
                Logs.SetProgress(1);
                SaveAsXML(data.pathToFile, data.rewrite, data.cdbxTagView,
                    data.cdbxNewNames);
                Logs.SetProgress(50);
                Logs.AddMessage("Done.");
            }
            catch (System.Exception ex)
            {
                Logs.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (Logs.IsNull() == false)
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Класс для передачи данных при сохранении XML базы каналов
        /// </summary>
        private class DataForSaveAsXML
        {
            public DataForSaveAsXML(string pathToFile, bool rewrite,
                bool cdbxTagView, bool cdbxNewNames)
            {
                this.pathToFile = pathToFile;
                this.rewrite = rewrite;
                this.cdbxNewNames = cdbxNewNames;
                this.cdbxTagView = cdbxTagView;
            }

            public string pathToFile;
            public bool rewrite;
            public bool cdbxTagView;
            public bool cdbxNewNames;
        }

        /// <summary>
        /// Экспорт проекта в базу каналов.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="rewrite">Перезаписывать или нет</param>
        /// <param name="cdbxNewNames">Использовать имена объектов вместо OBJECT
        /// </param>
        /// <param name="cdbxTagView">Сгруппировать тэги в один подтип</param>
        private void SaveAsXML(string path, bool rewrite = false, 
            bool cdbxTagView = false, bool cdbxNewNames = false)
        {
            projectConfig.SynchronizeDevices();

            var rootNode = new TreeNode("subtypes");
            techObjectManager.GetObjectForXML(rootNode, cdbxTagView,
                cdbxNewNames);
            deviceManager.GetObjectForXML(rootNode);

            var xmlDoc = new XmlDocument();
            bool createNewChannelBase = !File.Exists(path) || rewrite == true;
            if (createNewChannelBase)
            {
                bool rewriteExistFile = rewrite == true && File.Exists(path);
                if (rewriteExistFile)
                {
                    File.Delete(path);
                }

                CreateNewChannelBase(path, xmlDoc, rootNode);
            }
            else
            {
                xmlDoc.Load(path);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("driver", "http://brestmilk.by/driver/");
                XmlElement root = xmlDoc.DocumentElement;
                var elm = root.SelectSingleNode("//driver:id", nsmgr) as 
                    XmlElement;
                string baseId = elm.InnerText;
                               
                var subTypesId = GenerateSubTypesIdsList();

                elm = root.SelectSingleNode("//driver:subtypes", nsmgr) as 
                    XmlElement;
                nsmgr.AddNamespace("subtypes", "http://brestmilk.by/subtypes/");
                // Настройка элементов базы каналов
                foreach (XmlElement item in elm.ChildNodes)
                {
                    SetUpExistingChannelBase(subTypesId, item, rootNode);                   
                }

                // Вычисление свободных идентификаторов и их присвоение
                foreach(TreeNode subtype in rootNode.Nodes)
                {
                    CalculateIdentificatorsForChannelBase(subtype, elm, nsmgr,
                        baseId, subTypesId, xmlDoc);
                }
            }

            xmlDoc.Save(path);
        }

        /// <summary>
        /// Сгенерировать новую базу каналов
        /// </summary>
        /// <param name="path">Путь к месту хранения</param>
        /// <param name="xmlDoc">XML-документ, куда писать</param>
        /// <param name="rootNode">Узел с данными</param>
        private void CreateNewChannelBase(string path, 
            XmlDocument xmlDoc, TreeNode rootNode)
        {
            var textWritter = new XmlTextWriter(path,
                EncodingDetector.UTF8Bom);
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
                Logs.AddMessage("Не задано PLC_NAME.");
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
        /// Создание узлов и каналов в новой пустой базе каналов
        /// </summary>
        private void CreateNewChannels(XmlDocument xmlDoc,
            XmlElement subtypesNode, TreeNodeCollection Nodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                XmlElement subtypeElm = AddSubType(xmlDoc, subtypesNode,
                    Nodes[i], i);

                for (int j = 0; j < Nodes[i].Nodes.Count; j++)
                {
                    long channelId = long.Parse(("01" + i.ToString("X2") +
                        j.ToString("X4")),
                        System.Globalization.NumberStyles.HexNumber);
                    AddChannel(xmlDoc, Nodes[i].Nodes[j], subtypeElm,
                        channelId);
                }
            }
        }

        /// <summary>
        /// Генерация заполненного списка с номерами узлов
        /// </summary>
        /// <returns></returns>
        private List<string> GenerateSubTypesIdsList()
        {
            const int maxNodesCount = 256;
            var list = new List<string>();
            for (int i = 0; i < maxNodesCount; i++)
            {
                list.Add(i.ToString());
            };
            return list;
        }

        /// <summary>
        /// Настройка существующей базы каналов
        /// </summary>
        private void SetUpExistingChannelBase(List<string> subtupesId,
            XmlElement item, TreeNode rootNode)
        {
            const int channelLocationId = 9;
            XmlNodeList subTypeChannels = item.ChildNodes[channelLocationId]
                .ChildNodes;
            if (subtupesId.Contains(item.ChildNodes[0].InnerText))
            {
                subtupesId.Remove(item.ChildNodes[0].InnerText);
            }

            string objectName = item.ChildNodes[6].InnerText;
            TreeNode[] nodes = rootNode.Nodes.Cast<TreeNode>()
                .Where(r => r.Text == objectName).ToArray();
            SetUpChannelBaseObject(nodes, item, subTypeChannels);
        }

        /// <summary>
        /// Настройка узла базы каналов
        /// </summary>
        /// <param name="nodes">Редактируемые узлы базы каналов</param>
        /// <param name="item">Узел в XML</param>
        /// <param name="subTypeChannels">Тэги узла в XML</param>
        private void SetUpChannelBaseObject(TreeNode[] nodes, 
            XmlElement item, XmlNodeList subTypeChannels)
        {
            const int channelDescrNum = 4;

            ReplaceObjectNumber(subTypeChannels, nodes, channelDescrNum);
            ReplaceStepsToRunSteps(subTypeChannels, channelDescrNum);
            OnOffNodesDependOnTheirState(nodes, item, subTypeChannels,
                channelDescrNum);
        }

        /// <summary>
        /// Перезаписать номера каналов подтипа (в OBJECT).
        /// </summary>
        /// <param name="subTypeChannels">Список каналов</param>
        /// <param name="nodes">Редактируемые узлы</param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private void ReplaceObjectNumber(XmlNodeList subTypeChannels,
            TreeNode[] nodes, int channelDescrNum)
        {
            const string searchPattern = @"(?<name>OBJECT)(?<n>[0-9]+)+";

            XmlNode firstChannel = subTypeChannels[0];
            XmlNode firstChannelDescr =
                firstChannel.ChildNodes[channelDescrNum];
            bool hasTags = nodes.Length > 0;
            if (firstChannelDescr != null &&
                firstChannelDescr.InnerText.Contains(DefaultNodeName) &&
                hasTags)
            {
                string newNodeName = nodes.First().FirstNode.Text;
                string newNum = Regex.Match(newNodeName, searchPattern)
                    .Groups["n"].Value;

                string oldNodeName = firstChannelDescr.InnerText;
                string oldNum = Regex.Match(oldNodeName, searchPattern)
                    .Groups["n"].Value;

                if (newNum != oldNum)
                {
                    foreach (XmlElement channel in subTypeChannels)
                    {
                        channel.ChildNodes[channelDescrNum].InnerText =
                            Regex.Replace(channel.ChildNodes[channelDescrNum]
                            .InnerText, searchPattern,
                            $"{DefaultNodeName}{newNum}");
                    }
                }
            }
        }

        /// <summary>
        /// Замена устаревшего описания .STEPS на .RUN_STEPS
        /// </summary>
        /// <param name="channels">Список каналов </param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private void ReplaceStepsToRunSteps(XmlNodeList channels,
            int channelDescrNum)
        {
            const string searchOldStepsPattern = "\\.STEPS[1-9]\\[";

            foreach (XmlElement channel in channels)
            {
                var channelDescription = channel.ChildNodes[channelDescrNum]
                    .InnerText;
                Match oldStepsRegex = Regex
                    .Match(channelDescription, searchOldStepsPattern);
                if(oldStepsRegex.Success)
                {
                    channel.ChildNodes[channelDescrNum].InnerText =
                        channelDescription.Replace(".STEPS", ".RUN_STEPS");
                }
            }
        }

        ///<summary>
        /// Отключение и включение узлов и их тэгов в зависимости от их
        /// нового состояния (если удалены -> отключаются).
        ///</summary>
        /// <param name="nodes">Узел базы каналов</param>
        /// <param name="item">Узел в XML</param>
        /// <param name="subTypeChannels">Тэги узла в XML</param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private void OnOffNodesDependOnTheirState(TreeNode[] nodes,
            XmlElement item, XmlNodeList subTypeChannels, int channelDescrNum)
        {
            const int channelEnabledId = 3;
            const string disable = "0";
            const string enable = "-1";

            bool deletedNode = nodes.Length == 0;
            if (deletedNode)
            {
                // Комментирование удаленных узлов.
                item.ChildNodes[channelEnabledId].InnerText = disable;
            }
            else
            {
                item.ChildNodes[channelEnabledId].InnerText = enable;

                foreach (XmlElement chan in subTypeChannels)
                {
                    foreach (TreeNode node in nodes)
                    {
                        TreeNode[] chanNodes = node.Nodes.Find(
                            chan.ChildNodes[channelDescrNum].InnerText.Trim(),
                            true);
                        if (chanNodes.Length == 0)
                        {
                            chan.ChildNodes[channelEnabledId].InnerText = disable;
                            break;
                        }
                        else
                        {
                            chan.ChildNodes[channelEnabledId].InnerText = enable;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Расчет идентификаторов для базы каналов
        /// </summary>
        private void CalculateIdentificatorsForChannelBase(TreeNode subtype,
            XmlElement elm, XmlNamespaceManager nsmgr, string baseId,
            List<string> subTypesId, XmlDocument xmlDoc)
        {
            const int maxTagsCount = 65535;
            string xpath = "//subtypes:subtype[subtypes:sdrvname='" +
                subtype.Text + "']";
            var subElm = elm.SelectSingleNode(xpath, nsmgr) as XmlElement;
            if (subElm != null)
            {
                nsmgr.AddNamespace("channels", "http://brestmilk.by/channels/");
                var channelsElm = subElm.ChildNodes[9] as XmlElement;
                var channelsId = new List<long>();
                foreach (TreeNode channel in subtype.Nodes)
                {
                    XmlNode tagNode = null;
                    foreach(XmlNode node in channelsElm.ChildNodes)
                    {
                        if(node.InnerText.Contains(channel.Text))
                        {
                            tagNode = node;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (tagNode == null)
                    {
                        // Нахождение адреса канала среди свободных
                        if (channelsId.Count == 0)
                        {
                            long beginId = long
                                .Parse((long.Parse(baseId).ToString("X2") +
                              long.Parse(subElm.ChildNodes[0].InnerText)
                              .ToString("X2") + "0000"),
                              System.Globalization.NumberStyles.HexNumber);
                            for (int i = 0; i < maxTagsCount; i++)
                            {
                                channelsId.Add(beginId + i);
                            }

                            foreach (XmlElement channId in channelsElm
                                .ChildNodes)
                            {
                                long id = long.Parse(
                                    channId.FirstChild.InnerText);
                                if (channelsId.Contains(id))
                                {
                                    channelsId.Remove(id);
                                }
                            }
                        }

                        long channelId = channelsId[0];
                        channelsId.RemoveAt(0);
                        AddChannel(xmlDoc, channel, channelsElm, channelId);
                    }
                }
            }
            else
            {
                if (subTypesId.Count > 0)
                {
                    long subtypeId = long.Parse(subTypesId[0]);
                    subTypesId.RemoveAt(0);
                    XmlElement newSubtype = AddSubType(xmlDoc, elm, subtype, 
                        subtypeId);
                    string hex = long.Parse(baseId).ToString("X2") +
                        subtypeId.ToString("X2");
                    for (int i = 0; i < subtype.Nodes.Count; i++)
                    {
                        long channelId = long.Parse((hex + i.ToString("X4")),
                            System.Globalization.NumberStyles.HexNumber);
                        AddChannel(xmlDoc, subtype.Nodes[i], newSubtype, 
                            channelId);
                    }
                }
                else
                {
                    Logs.AddMessage("Превышено количество подтипов " +
                        "в базе каналов.");
                    return;
                }
            }
        }

        /// <summary>
        /// Добавление узла в базу каналов
        /// </summary>
        private XmlElement AddSubType(XmlDocument xmlDoc,
            XmlElement subtypesNode, TreeNode Node, long subTypeId)
        {
            string ns = "http://brestmilk.by/subtypes/";
            string np = "http://brestmilk.by/parameters/";
            string nc = "http://brestmilk.by/channels/";
            string prefix = "subtypes";

            XmlElement subType = xmlDoc.CreateElement(prefix, "subtype", ns);
            subtypesNode.AppendChild(subType);
            XmlElement subtypeElm = xmlDoc.CreateElement(prefix, "sid", ns);
            subtypeElm.InnerText = subTypeId.ToString();
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "stid", ns);
            subtypeElm.InnerText = "0";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "maxchannels", ns);
            subtypeElm.InnerText = "0";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "enabled", ns);
            subtypeElm.InnerText = "-1";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "descr", ns);
            subtypeElm.InnerText = "Описание";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "defdescr", ns);
            subtypeElm.InnerText = "Описание";
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "sdrvname", ns);
            subtypeElm.InnerText = Node.Text;
            subType.AppendChild(subtypeElm);
            subtypeElm = xmlDoc.CreateElement(prefix, "sdrvdefname", ns);
            subtypeElm.InnerText = "Узел";
            subType.AppendChild(subtypeElm);

            subtypeElm = xmlDoc.CreateElement(prefix, "common_parameters", ns);
            subtypeElm.SetAttribute("xmlns:parameters", np);
            subType.AppendChild(subtypeElm);

            subtypeElm = xmlDoc.CreateElement(prefix, "channels", ns);
            subtypeElm.SetAttribute("xmlns:channels", nc);
            subType.AppendChild(subtypeElm);
            return subtypeElm;
        }

        /// <summary>
        /// Добавление канала с указанным адресом
        /// </summary>
        private void AddChannel(XmlDocument xmlDoc, TreeNode node,
            XmlElement subtypeElm, long channelId)
        {
            string prefix = "channels";
            string nsChannels = "http://brestmilk.by/channels/";
            XmlElement channel = xmlDoc.CreateElement(prefix, "channel", 
                nsChannels);
            subtypeElm.AppendChild(channel);
            XmlElement channelElm = xmlDoc.CreateElement(prefix, "id", 
                nsChannels);
            channelElm.InnerText = channelId.ToString();
            channel.AppendChild(channelElm);

            string subtypeName = subtypeElm.ParentNode.ChildNodes[6].InnerText;
            bool needSetPeriod = Period.Contains(subtypeName);
            if (needSetPeriod)
            {
                channelElm = xmlDoc.CreateElement(prefix, "requesttype", 
                    nsChannels);
                channelElm.InnerText = "0";
                channel.AppendChild(channelElm);
                channelElm = xmlDoc.CreateElement(prefix, "requestperiod", 
                    nsChannels);
                channelElm.InnerText = GetRequestPeriodForTag(subtypeName);
            }
            else
            {
                channelElm = xmlDoc.CreateElement(prefix, "requesttype", 
                    nsChannels);
                channelElm.InnerText = "1";
                channel.AppendChild(channelElm);
                channelElm = xmlDoc.CreateElement(prefix, "requestperiod", 
                    nsChannels);
                channelElm.InnerText = "1";
            }

            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "enabled", nsChannels);
            channelElm.InnerText = "-1";
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "descr", nsChannels);
            channelElm.InnerText = node.Text;
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "delta", nsChannels);
            
            if (needSetPeriod)
            {
                channelElm.InnerText = GetDeltaForTag(subtypeName);
            }
            else
            {
                channelElm.InnerText = "0";
            }
            
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "apptime", nsChannels);
            channelElm.InnerText = "0";
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "protocol", nsChannels);

            bool needSetProtocol = GetNeedProtocolCondition(subtypeName, node);
            if (needSetProtocol)
            {
                channelElm.InnerText = "-1";
            }
            else
            {
                channelElm.InnerText = "0";
            }

            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "transexprin", 
                nsChannels);
            channelElm.AppendChild(xmlDoc.CreateCDataSection(""));
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "transexprout", 
                nsChannels);
            channelElm.AppendChild(xmlDoc.CreateCDataSection(""));
            channel.AppendChild(channelElm);
            channelElm = xmlDoc.CreateElement(prefix, "channel_parameters", 
                nsChannels);
            string nsParams = "http://brestmilk.by/parameters/";
            channelElm.SetAttribute("xmlns:parameters", nsParams);
            
            if (node.Text.Contains("UP_TIME") || 
                node.Text.Contains("CMD_ANSWER") ||
                node.Text.Contains("VERSION"))
            {
                AddChannelAtribute(xmlDoc, channelElm, "IsString");
            
            }
            channel.AppendChild(channelElm);
        }

        /// <summary>
        /// Получить период опроса для тэга
        /// </summary>
        /// <param name="tagName">Тэг</param>
        /// <returns></returns>
        private string GetRequestPeriodForTag(string tagName)
        {
            if (!tagName.Contains("LE") && !tagName.Equals("V_V"))
            {
                return "3000";
            }
            else
            {
                return "5000";
            }
        }

        /// <summary>
        /// Получить дельту для тэга
        /// </summary>
        /// <param name="tagName">Тэг</param>
        /// <returns></returns>
        private string GetDeltaForTag(string tagName)
        {
            if (tagName.Contains("QT"))
            {
                return "0.1";
            }
            else if (tagName.Equals("V_V"))
            {
                return "1";
            }
            else if (tagName.Equals("VC_V") || tagName.Equals("M_V"))
            {
                return "0.5";
            }
            else
            {
                return "0.2";
            }
        }

        /// <summary>
        /// Получить необходимость протоколирования тэга
        /// </summary>
        /// <param name="tagName">Имя тэга</param>
        /// <param name="node">Имя узла</param>
        /// <returns></returns>
        private bool GetNeedProtocolCondition(string tagName, 
            TreeNode node)
        {
            bool protocolDev = Protocol.Contains(tagName);
            bool protocolObject =
                node.Text.Contains(DefaultNodeName) &&
                (node.Text.Contains("ST") ||
                node.Text.Contains("MODES") ||
                node.Text.Contains("OPERATIONS") ||
                node.Text.Contains("STEPS"));
            bool protocolPID = GetPIDProtocolability(tagName, node.Text);

            if (protocolDev || protocolObject || protocolPID) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Получить протоколируемость ПИД-регулятора
        /// </summary>
        /// <param name="tagName">Имя тэга</param>
        /// <param name="nodeText">Имя узла</param>
        /// <returns></returns>
        private bool GetPIDProtocolability(string tagName, string nodeText)
        {
            bool isPID = deviceManager
                .GetDeviceByEplanName(tagName).DeviceType == DeviceType.C;
            if (isPID && ProtocolPID.Any(x => nodeText.Contains(x)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Добавление атрибута канала
        /// </summary>
        private void AddChannelAtribute(XmlDocument xmlDoc,
            XmlElement elm, string atribute)
        {
            string prefix = "parameters";
            string nsParams = "http://brestmilk.by/parameters/";
            XmlElement parametersElm = xmlDoc.CreateElement(prefix, "channel", 
                nsParams);
            elm.AppendChild(parametersElm);
            XmlElement parElm = xmlDoc.CreateElement(prefix, "name", nsParams);
            parElm.InnerText = atribute;
            parametersElm.AppendChild(parElm);
            parElm = xmlDoc.CreateElement(prefix, "value", nsParams);
            parElm.InnerText = "1";
            parametersElm.AppendChild(parElm);

        }

        private const string DefaultNodeName = "OBJECT";

        /// <summary>
        /// Узлы, в которых устанавливается протоколирование элементов.
        /// </summary>
        private static HashSet<string> Protocol =
            new HashSet<string>(new string[]
            {
                "TE_V",
                "QT_V",
                "FQT_F",
                "PT_V",
                "VC_V",
                "M_V",
                "M_ST",
                "LT_CLEVEL",
                "V_ST",
                "LS_ST",
                "FS_ST",
                "GS_ST",
                "SB_ST",
                "DI_ST",
                "DO_ST",
                "SB_ST",
                "HL_ST",
                "HA_ST",
                "AO_V",
                "AI_V"
            });

        private static HashSet<string> ProtocolPID =
            new HashSet<string>(new string[]
            {
                ".V",
                ".Z"
            });

        /// <summary>
        /// Узлы, в которых устанавливается опрос по времени.
        /// </summary>
        private static HashSet<string> Period =
            new HashSet<string>(new string[] {
                "TE_V",
                "QT_V",
                "LT_V",
                "PT_V",
                "AO_V",
                "AI_V",
                "FQT_F",
                "M_V",
                "VC_V",
                "LT_CLEVEL",
                "V_V"
            });

        /// <summary>
        /// Получить количество тегов проекта.
        /// </summary>
        /// <returns></returns>
        public static int GetTagsCount()
        {
            var rootNode = new TreeNode("subtypes");
            bool useNewNames = false;
            bool combineTags = false;
            techObjectManager.GetObjectForXML(rootNode, combineTags,
                useNewNames);
            deviceManager.GetObjectForXML(rootNode);

            int tagsCount = 0;
            foreach (TreeNode node in rootNode.Nodes)
            {
                tagsCount += node.Nodes.Count;
            }

            return tagsCount;
        }

        public void AutomaticExportNewChannelBaseCombineTags(
            string projectDirPath, string projectName)
        {
            string path = Path.Combine(projectDirPath, "chbase");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, $"{projectName}.cdbx");
            SaveAsXML(path, true, true);
        }

        static ProjectConfiguration projectConfig = ProjectConfiguration
            .GetInstance();
        static ITechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
        static IDeviceManager deviceManager = DeviceManager.GetInstance();
    }
}
