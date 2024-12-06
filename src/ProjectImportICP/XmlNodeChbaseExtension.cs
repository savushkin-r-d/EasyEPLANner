using EplanDevice;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EasyEPlanner.ProjectImportICP
{
    public static class XmlNodeChbaseExtension
    {
        public static XmlNamespaceManager ChbaseNameSpace(this XmlNode node)
        {
            XmlNamespaceManager nsMngr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            nsMngr.AddNamespace("driver", "http://brestmilk.by/driver/");
            nsMngr.AddNamespace("subtypes", "http://brestmilk.by/subtypes/");
            nsMngr.AddNamespace("channels", "http://brestmilk.by/channels/");
             
            return nsMngr;
        }


        /// <summary>
        /// Обновить названия каналов устройств в старой (исходной) базе каналов
        /// </summary>
        /// <param name="srcRoot">Корневой узел исходной базы каналов</param>
        /// <param name="namingMap">Словарь переименования</param>
        public static void UpdateDeviceTags(this XmlNode srcRoot, IEnumerable<(string Name, string WagoName)> namingMap, IDeviceManager deviceManager)
        {
            foreach (var replacingName in namingMap)
            {
                if (string.IsNullOrEmpty(replacingName.WagoName))
                {
                    Logs.AddMessage($"{replacingName.Name}: wago name is null\n");

                    continue;
                }

                var replacingNode = srcRoot.SelectSingleNode($"//channels:channel[contains(channels:descr, '{replacingName.WagoName}')]", srcRoot.ChbaseNameSpace());

                if (replacingNode is null)
                {
                    Logs.AddMessage($"Канал {replacingName.WagoName} указанный в устройстве {replacingName.Name} не найден в старой базе каналов.\n");
                    continue;
                }

                var dev = deviceManager.GetDevice(replacingName.Name);

                if (dev.DeviceSubType is DeviceSubType.NONE)
                {
                    Logs.AddMessage($"У устройства {replacingName.Name} не задан подтип.\n");
                    continue;
                }

                var ST = dev.GetDeviceProperties(dev.DeviceType, dev.DeviceSubType).ContainsKey(IODevice.Tag.ST);
                replacingNode.SelectSingleNode("./channels:descr/text()", srcRoot.ChbaseNameSpace()).Value = $"{replacingName.Name}.{(ST ? IODevice.Tag.ST : IODevice.Tag.V)}";
            }
        }


        /// <summary>
        /// Смещение идентификаторов подтипов и каналов базы каналов
        /// </summary>
        /// <param name="root">Корневой узел базы каналов</param>
        /// <param name="driverID">Новый идентификатор драйвера</param>
        /// <param name="offsetID">Смещение для идентификаторов каналов</param>
        public static void ShiftIds(this XmlNode root, int driverID, int offsetID)
        {
            root.SelectSingleNode("//driver:id", root.ChbaseNameSpace()).InnerText = driverID.ToString();
            var channelDriverIdOctet = driverID << 24; // 0x[driver id]000000
            foreach (XmlNode subtype in root.SelectNodes("//subtypes:subtype", root.ChbaseNameSpace()))
            {
                var subtypeIdNode = subtype.SelectSingleNode("./subtypes:sid/text()", root.ChbaseNameSpace());
                var subtypeId = int.Parse(subtypeIdNode.Value) + offsetID;

                subtypeIdNode.Value = subtypeId.ToString();

                var channelSubtypeOctet = subtypeId << 16; // 0x00[channel subtype id]0000
                foreach (XmlNode channel in subtype.SelectNodes("./subtypes:channels/channels:channel", root.ChbaseNameSpace()))
                {
                    var channelIdNode = channel.SelectSingleNode("./channels:id/text()", root.ChbaseNameSpace());

                    channelIdNode.Value = (int.Parse(channelIdNode.Value) & 0x0000FFFF | channelDriverIdOctet | channelSubtypeOctet).ToString();
                }
            }
        }


        /// <summary>
        /// Вставка старой (исходной) базы каналов в новую
        /// </summary>
        /// <param name="srcRoot">Корневой узел исходной базы каналов</param>
        /// <param name="dstRoot">Корневой узел новой базы каналов</param>
        /// <param name="dstXmlDoc">Новая база каналов</param>
        public static void InsertSubtypes(this XmlNode dstRoot, XmlNode srcRoot)
        {
            var dstSubtypes = dstRoot.SelectSingleNode("//driver:subtypes", dstRoot.ChbaseNameSpace());
            foreach (XmlNode srcSubtype in srcRoot.SelectNodes("//subtypes:subtype", srcRoot.ChbaseNameSpace()).OfType<XmlNode>().Reverse())
            {
                dstSubtypes.PrependChild(dstRoot.OwnerDocument.ImportNode(srcSubtype, true));
            }
        }


        /// <summary>
        /// Выключение (установка 0) в найденные узлы по selector
        /// </summary>
        /// <param name="root">Корневой узел базы каналов</param>
        /// <param name="selector">XPath</param>
        public static void DisableTags(this XmlNode root, string selector)
        {
            foreach (XmlNode enabled in root.SelectNodes(selector, root.ChbaseNameSpace()))
            {
                enabled.Value = "0"; // disabled
            }
        }


        /// <summary>
        /// Лог изменений каналов устройств для новой базы каналов
        /// </summary>
        /// <param name="dstRoot">Новая база каналов</param>
        /// <param name="srcRoot">Старая база каналов</param>
        public static void CompareDevicesChannelsWith(this XmlNode dstRoot, XmlNode srcRoot)
        {
            Logs.AddMessage("\n\nМодификация тегов устройств:\n");

            var newChannels = dstRoot.SelectNodes("//subtypes:subtype[./subtypes:sid='0']/subtypes:channels/channels:channel", dstRoot.ChbaseNameSpace());
            var channels = srcRoot.SelectNodes("//subtypes:subtype[./subtypes:sid='0']/subtypes:channels/channels:channel", srcRoot.ChbaseNameSpace());

            for (var i = 0; i < channels.Count; ++i)
            {
                var newTag = newChannels[i].SelectSingleNode("./channels:descr/text()", dstRoot.ChbaseNameSpace());
                var tag = channels[i].SelectSingleNode("./channels:descr/text()", srcRoot.ChbaseNameSpace());
                var enabled = int.Parse(newChannels[i].SelectSingleNode("./channels:enabled/text()", dstRoot.ChbaseNameSpace()).Value) != 0;

                if (tag.Value != newTag.Value)
                    Logs.AddMessage($"{(enabled ? ">" : "- ")} {tag.Value.Split(':').First().Trim()} => {newTag.Value};\n");
                else Logs.AddMessage($"{(enabled ? ">" : "- ")} {tag.Value.Split(':').First().Trim()};\n");
            }
        }
    }
}
