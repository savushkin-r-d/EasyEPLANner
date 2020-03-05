using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using TechObject;
using Device;

namespace EasyEPlanner
{
    public static class XMLReporter
    {
        /// <summary>
        /// Получить количество тегов проекта.
        /// </summary>
        /// <returns></returns>
        public static int GetTagsCount()
        {
            TreeNode rootNode = new TreeNode("subtypes");
            techObjectManager.GetObjectForXML(rootNode);
            deviceManager.GetObjectForXML(rootNode);
            var tagsCount = rootNode.Nodes.Count;

            return tagsCount;
        }

        /// <summary>
        /// Экспорт проекта в базу каналов.
        /// </summary>
        /// <param name="prjName">Имя проекта</param>
        public static void SaveAsXML(string path)
        {
            projectConfig.SynchronizeDevices();

            TreeNode rootNode = new TreeNode("subtypes");
            techObjectManager.GetObjectForXML(rootNode);
            deviceManager.GetObjectForXML(rootNode);

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
                        TreeNode[] nodes = rootNode.Nodes.Cast<TreeNode>()
                            .Where(r => r.Text == item.ChildNodes[6].InnerText).ToArray();
                        if (nodes.Length == 0)
                        {
                            // нужно закомментировать не использующиеся узлы
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
                                        (long.Parse(baseId).ToString("X2") +
                                      long.Parse(
                                      subElm.ChildNodes[0].InnerText).ToString("X2") + "0000"),
                                      System.Globalization.NumberStyles.HexNumber);
                                    for (int i = 0; i < 65535; i++)
                                    {
                                        channelsId.Add(beginId + i);
                                    }
                                    foreach (XmlElement channId in channelsElm.ChildNodes)
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
                                AddChannel(xmlDoc, channel,
                                    channelsElm, channelId);
                            }
                        }
                    }
                    else
                    {
                        if (subtupesId.Count > 0)
                        {
                            long subtypeId = long.Parse(subtupesId[0]);
                            subtupesId.RemoveAt(0);
                            XmlElement newSubtype =
                                AddSubType(xmlDoc, elm, subtype, subtypeId);
                            string hex = long.Parse(baseId).ToString("X2") +
                                subtypeId.ToString("X2");
                            for (int i = 0; i < subtype.Nodes.Count; i++)
                            {
                                long channelId = long.Parse((hex +
                                    i.ToString("X4")),
                                    System.Globalization.NumberStyles.HexNumber);
                                AddChannel(xmlDoc, subtype.Nodes[i],
                                    newSubtype, channelId);
                            }
                        }
                        else
                        {
                            ProjectManager.GetInstance().AddLogMessage(
                                "Превышено количество подтипов в базе каналов.");
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
        private static XmlElement WriteCommonXMLPart(XmlDocument xmlDoc)
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
                ProjectManager.GetInstance()
                    .AddLogMessage("Не задано PLC_NAME.");
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
                Node.Text.Contains("OBJECT") &&
                (Node.Text.Contains("ST") ||
                Node.Text.Contains("MODES") ||
                Node.Text.Contains("OPERATIONS") ||
                Node.Text.Contains("STEPS"));
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
                if (!subtypeName.Contains("LE") && !subtypeName.Equals("V_V"))
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
                if (subtypeName.Contains("QT"))
                {
                    channelElm.InnerText = "0.1";
                }
                else if (subtypeName.Equals("V_V"))
                {
                    channelElm.InnerText = "1";
                }
                else if (subtypeName.Equals("VC_V") ||
                    subtypeName.Equals("M_V"))
                {
                    channelElm.InnerText = "0.5";
                }
                else
                {
                    channelElm.InnerText = "0.2";
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

        static ProjectConfiguration projectConfig = ProjectConfiguration
            .GetInstance();
        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
        static DeviceManager deviceManager = DeviceManager.GetInstance();
    }
}
