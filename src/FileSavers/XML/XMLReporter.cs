using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using TechObject;
using EplanDevice;
using System.Text.RegularExpressions;
using System.Threading;
using EasyEPlanner.FileSavers.XML;
using System.Diagnostics.CodeAnalysis;
using System;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class XmlReporter
    {
        /// <summary>
        /// Экспорт из проекта базы каналов.
        /// </summary>
        public void SaveAsCDBX(string projectName, bool combineTag = false,
            bool useNewNames = false, bool rewrite = false)
        {
            Thread t = new Thread(
                new ParameterizedThreadStart(SaveAsXMLThread));

            var dataForSave = new DataForSaveAsXml(projectName, rewrite,
                combineTag, useNewNames);
            t.CurrentCulture = StaticHelper.CommonConst
                .CultureWithDotInsteadComma;
            t.Start(dataForSave);
        }

        public void AwaitSaveAsCDBX(string projectName, bool combineTag = false, bool useNewNames = false, bool rewrite = false)
        {
            SaveAsXML(projectName, rewrite, combineTag, useNewNames);
        }


        /// <summary>
        /// Экспорт базы каналов, поток.
        /// </summary>
        /// <param name="param">Параметр потока</param>
        private void SaveAsXMLThread(object param)
        {
            var data = param as DataForSaveAsXml;

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
                if (!Logs.IsNull())
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Класс для передачи данных при сохранении XML базы каналов
        /// </summary>
        private sealed class DataForSaveAsXml
        {
            public DataForSaveAsXml(string pathToFile, bool rewrite,
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

            var root = new Driver();
            techObjectManager.GetObjectForXML(root, cdbxTagView, cdbxNewNames);
            deviceManager.GetObjectForXML(root);

            var xmlDoc = new XmlDocument();
            if (!File.Exists(path) || rewrite)
            { // Create new chbase
                if (rewrite && File.Exists(path))
                    File.Delete(path);

                CreateNewChannelBase(path, xmlDoc, root);
            }
            else
            {
                xmlDoc.Load(path);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("driver", "http://brestmilk.by/driver/");
                XmlElement xmlroot = xmlDoc.DocumentElement;
                var elm = xmlroot.SelectSingleNode("//driver:id", nsmgr) as 
                    XmlElement;
                string baseId = elm.InnerText;
                               
                var subTypesId = GenerateSubTypesIdsList();

                elm = xmlroot.SelectSingleNode("//driver:subtypes", nsmgr) as 
                    XmlElement;
                nsmgr.AddNamespace("subtypes", "http://brestmilk.by/subtypes/");
                // Настройка элементов базы каналов
                foreach (XmlElement item in elm.ChildNodes)
                {
                    SetUpExistingChannelBase(subTypesId, item, root);                   
                }

                // Вычисление свободных идентификаторов и их присвоение
                foreach(ISubtype subtype in root.Subtypes)
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
        private void CreateNewChannelBase(string path, XmlDocument xmlDoc, IDriver root)
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
            CreateNewChannels(xmlDoc, subtypesNode, root.Subtypes);
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
        private void CreateNewChannels(XmlDocument xmlDoc, XmlElement subtypesNode, List<ISubtype> subtypes)
        {
            foreach (var i in Enumerable.Range(0, subtypes.Count))
            {
                XmlElement subtypeElm = AddSubType(xmlDoc, subtypesNode, subtypes[i], i);

                foreach (var j in Enumerable.Range(0, subtypes[i].Channels.Count))
                {
                    long channelId = long.Parse($"01{i:X2}{j:X4}", System.Globalization.NumberStyles.HexNumber);
                    AddChannel(xmlDoc, subtypes[i].Channels[j], subtypeElm, channelId);
                }
            }
        }

        /// <summary>
        /// Генерация заполненного списка с номерами узлов
        /// </summary>
        /// <returns></returns>
        private static List<string> GenerateSubTypesIdsList()
        {
            const int maxNodesCount = 256;
            var list = new List<string>();
            for (int i = 0; i < maxNodesCount; i++)
            {
                list.Add(i.ToString());
            }
            return list;
        }

        /// <summary>
        /// Настройка существующей базы каналов
        /// </summary>
        private void SetUpExistingChannelBase(List<string> subtupesId,
            XmlElement item, IDriver root)
        {
            const int channelLocationId = 9;
            XmlNodeList subTypeChannels = item.ChildNodes[channelLocationId]
                .ChildNodes;
            if (subtupesId.Contains(item.ChildNodes[0].InnerText))
            {
                subtupesId.Remove(item.ChildNodes[0].InnerText);
            }

            string objectName = item.ChildNodes[6].InnerText;
            var subtypes = root.Subtypes.Where(s => s.Description == objectName).ToList();
            SetUpChannelBaseObject(subtypes, item, subTypeChannels);
        }

        /// <summary>
        /// Настройка узла базы каналов
        /// </summary>
        /// <param name="subtypes">Редактируемые узлы базы каналов</param>
        /// <param name="item">Узел в XML</param>
        /// <param name="subTypeChannels">Тэги узла в XML</param>
        private void SetUpChannelBaseObject(List<ISubtype> subtypes, 
            XmlElement item, XmlNodeList subTypeChannels)
        {
            const int channelDescrNum = 4;

            ReplaceObjectNumber(subTypeChannels, subtypes, channelDescrNum);
            ReplaceStepsToRunSteps(subTypeChannels, channelDescrNum);
            OnOffNodesDependOnTheirState(subtypes, item, subTypeChannels,
                channelDescrNum);
        }

        /// <summary>
        /// Перезаписать номера каналов подтипа (в OBJECT).
        /// </summary>
        /// <param name="subTypeChannels">Список каналов</param>
        /// <param name="subtypes">Редактируемые узлы</param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private static void ReplaceObjectNumber(XmlNodeList subTypeChannels,
            List<ISubtype> subtypes, int channelDescrNum)
        {
            const string searchPattern = @"(?<name>OBJECT)(?<n>[0-9]+)+";

            XmlNode firstChannel = subTypeChannels[0];
            XmlNode firstChannelDescr =
                firstChannel.ChildNodes[channelDescrNum];
            if (firstChannelDescr != null &&
                firstChannelDescr.InnerText.Contains(DefaultNodeName) &&
                subtypes.SelectMany(st => st.Channels).Any())
            {
                string newNodeName = subtypes[0].Channels[0].Description;
                string newNum = Regex.Match(newNodeName, searchPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100))
                    .Groups["n"].Value;

                string oldNodeName = firstChannelDescr.InnerText;
                string oldNum = Regex.Match(oldNodeName, searchPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100))
                    .Groups["n"].Value;

                if (newNum != oldNum)
                {
                    foreach (XmlElement channel in subTypeChannels)
                    {
                        channel.ChildNodes[channelDescrNum].InnerText =
                            Regex.Replace(
                                channel.ChildNodes[channelDescrNum].InnerText,
                                searchPattern, $"{DefaultNodeName}{newNum}",
                                RegexOptions.None, TimeSpan.FromMilliseconds(100));
                    }
                }
            }
        }

        /// <summary>
        /// Замена устаревшего описания .STEPS на .RUN_STEPS
        /// </summary>
        /// <param name="channels">Список каналов </param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private static void ReplaceStepsToRunSteps(XmlNodeList channels,
            int channelDescrNum)
        {
            const string searchOldStepsPattern = "\\.STEPS[1-9]\\[";

            foreach (XmlElement channel in channels)
            {
                var channelDescription = channel.ChildNodes[channelDescrNum]
                    .InnerText;
                Match oldStepsRegex = Regex
                    .Match(channelDescription, searchOldStepsPattern,
                    RegexOptions.None, TimeSpan.FromMilliseconds(100));
                if (oldStepsRegex.Success)
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
        /// <param name="subtypes">Узел базы каналов</param>
        /// <param name="item">Узел в XML</param>
        /// <param name="subTypeChannels">Тэги узла в XML</param>
        /// <param name="channelDescrNum">Номер ячейки описания канала</param>
        private static void OnOffNodesDependOnTheirState(List<ISubtype> subtypes,
            XmlElement item, XmlNodeList subTypeChannels, int channelDescrNum)
        {
            const int channelEnabledId = 3;
            const string disable = "0";
            const string enable = "-1";

            if (subtypes.Count == 0)
            { 
                // Удалить узел
                item.ChildNodes[channelEnabledId].InnerText = disable; // Комментирование удаленных узлов.
            }
            else
            {
                item.ChildNodes[channelEnabledId].InnerText = enable;

                foreach (XmlElement chan in subTypeChannels)
                {
                    foreach (var subtype in subtypes)
                    {
                        var cahnnels = subtype.Channels.Where(ch => ch.Description == chan.ChildNodes[channelDescrNum].InnerText.Trim());
                        if (cahnnels.Any())
                        {
                            chan.ChildNodes[channelEnabledId].InnerText = enable;
                        }
                        else
                        {
                            chan.ChildNodes[channelEnabledId].InnerText = disable;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Расчет идентификаторов для базы каналов
        /// </summary>
        private void CalculateIdentificatorsForChannelBase(ISubtype subtype,
            XmlElement elm, XmlNamespaceManager nsmgr, string baseId,
            List<string> subTypesId, XmlDocument xmlDoc)
        {
            const int maxTagsCount = 65535;
            string xpath = "//subtypes:subtype[subtypes:sdrvname='" +
                subtype.Description + "']";
            var subElm = elm.SelectSingleNode(xpath, nsmgr) as XmlElement;
            if (subElm is null)
            {
                if (subTypesId.Count <= 0)
                {
                    Logs.AddMessage("Превышено количество подтипов " +
                        "в базе каналов.");
                    return;
                }
               
               
                long subtypeId = long.Parse(subTypesId[0]);
                subTypesId.RemoveAt(0);
                XmlElement newSubtype = AddSubType(xmlDoc, elm, subtype,
                    subtypeId);
                string hex = long.Parse(baseId).ToString("X2") +
                    subtypeId.ToString("X2");
                for (int i = 0; i < subtype.Channels.Count; i++)
                {
                    long channelId = long.Parse((hex + i.ToString("X4")),
                        System.Globalization.NumberStyles.HexNumber);
                    AddChannel(xmlDoc, subtype.Channels[i], newSubtype,
                        channelId);
                }
                return;
            }
            
            nsmgr.AddNamespace("channels", "http://brestmilk.by/channels/");
            var channelsElm = subElm.ChildNodes[9] as XmlElement;
            var channelsId = new List<long>();
            foreach (IChannel channel in subtype.Channels)
            {
                XmlNode tagNode = null;
                foreach (XmlNode node in channelsElm.ChildNodes)
                {
                    if (node.InnerText.Contains(channel.Name))
                    {
                        tagNode = node;
                    }
                }

                if (tagNode is not null)
                    continue;

               
                // Нахождение адреса канала среди свободных
                if (channelsId.Count == 0)
                {
                    long beginId = long.Parse(
                        long.Parse(baseId).ToString("X2") +
                        long.Parse(subElm.ChildNodes[0].InnerText).ToString("X2") + 
                        "0000",
                        System.Globalization.NumberStyles.HexNumber);
                    for (int i = 0; i < maxTagsCount; i++)
                    {
                        channelsId.Add(beginId + i);
                    }

                    foreach (XmlElement channId in channelsElm.ChildNodes)
                    {
                        long id = long.Parse(channId.FirstChild.InnerText);
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

        /// <summary>
        /// Добавление узла в базу каналов
        /// </summary>
        private static XmlElement AddSubType(XmlDocument xmlDoc,
            XmlElement subtypesNode, ISubtype subtype, long subTypeId)
        {
            string ns = "http://brestmilk.by/subtypes/";
            string np = "http://brestmilk.by/parameters/";
            string nc = "http://brestmilk.by/channels/";
            string prefix = "subtypes";

            var xmlSubType = subtypesNode.AddElement(prefix, "subtype", ns);

            xmlSubType.AddElement(prefix, "sid", ns, subTypeId.ToString());
            xmlSubType.AddElement(prefix, "stid", ns, "0");
            xmlSubType.AddElement(prefix, "maxchannels", ns, "0");
            xmlSubType.AddElement(prefix, "enabled", ns, "-1");
            xmlSubType.AddElement(prefix, "descr", ns, "Описание");
            xmlSubType.AddElement(prefix, "defdescr", ns, "Описание");
            xmlSubType.AddElement(prefix, "sdrvname", ns, subtype.Description);
            xmlSubType.AddElement(prefix, "sdrvdefname", ns, "Узел");
            xmlSubType.AddElement(prefix, "common_parameters", ns)
                .SetAttribute("xmlns:parameters", np);
            var channels = xmlSubType.AddElement(prefix, "channels", ns);
            channels.SetAttribute("xmlns:channels", nc);

            return channels;
        }

        /// <summary>
        /// Добавление канала с указанным адресом
        /// </summary>
        private void AddChannel(XmlDocument xmlDoc, IChannel channel,
            XmlElement subtypeElm, long channelId)
        {
            string prefix = "channels";
            string ns = "http://brestmilk.by/channels/";

            var xmlChannel = subtypeElm.AddElement(prefix, "channel", ns);
            xmlChannel.AddElement(prefix, "id", ns, channelId.ToString());
            xmlChannel.AddElement(prefix, "requesttype", ns, channel.IsRequestByTime ? "0" : "1");
            xmlChannel.AddElement(prefix, "requestperiod", ns, channel.RequestPeriod.ToString());
            xmlChannel.AddElement(prefix, "enabled", ns, "-1");
            xmlChannel.AddElement(prefix, "descr", ns, channel.Description);
            xmlChannel.AddElement(prefix, "delta", ns, channel.Delta.ToString());
            xmlChannel.AddElement(prefix, "apptime", ns, "0");
            xmlChannel.AddElement(prefix, "protocol", ns, channel.IsLogged ? "-1" : "0");

            xmlChannel.AddElement(prefix, "transexprin", ns)
                .AppendChild(xmlDoc.CreateCDataSection(""));

            xmlChannel.AddElement(prefix, "transexprout", ns)
                .AppendChild(xmlDoc.CreateCDataSection(""));

            var channelElm = xmlChannel.AddElement(prefix, "channel_parameters", ns);
            channelElm.SetAttribute("xmlns:parameters", "http://brestmilk.by/parameters/");

            foreach (var parameter in channel.Parameters)
            {
                AddChannelAtribute(xmlDoc, channelElm, parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Добавление атрибута канала
        /// </summary>
        private static void AddChannelAtribute(
            XmlDocument xmlDoc, XmlElement elm,
            string atribute, string value)
        {
            string prefix = "parameters";
            string nsParams = "http://brestmilk.by/parameters/";

            var par = elm.AddElement(prefix, "channel", nsParams);
            par.AddElement(prefix, "name", nsParams, atribute);
            par.AddElement(prefix, "value", nsParams, value);
        }

        private const string DefaultNodeName = "OBJECT";

        /// <summary>
        /// Получить количество тегов проекта.
        /// </summary>
        /// <returns></returns>
        public static int GetTagsCount()
        {
            var root = new Driver();
            bool useNewNames = false;
            bool combineTags = false;
            techObjectManager.GetObjectForXML(root, combineTags, useNewNames);
            deviceManager.GetObjectForXML(root);

            int tagsCount = 0;
            foreach (var subtype in root.Subtypes)
            {
                tagsCount += subtype.Channels.Count;
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

    public static class XmlExtension
    {
        public static XmlElement AddElement(this XmlElement xmlElement, string prefix, string attribute, string ns, string value)
        {
            var channelElm = xmlElement.OwnerDocument.CreateElement(prefix, attribute, ns);
            channelElm.InnerText = value;
            xmlElement.AppendChild(channelElm);

            return channelElm;
        }

        public static XmlElement AddElement(this XmlElement xmlElement, string prefix, string attribute, string ns)
        {
            var channelElm = xmlElement.OwnerDocument.CreateElement(prefix, attribute, ns);
            xmlElement.AppendChild(channelElm);

            return channelElm;
        }
    }
}
