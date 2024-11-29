using EplanDevice;
using StaticHelper;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyEPlanner.ProjectImportICP
{
    /// <summary>
    /// Трансформатор базы каналов
    /// </summary>
    public class ChannelBaseTransformer
    {
        /// <summary>
        /// Тег
        /// </summary>
        public class Tag
        {
            public string Name { get; set; }

            public string WagoName { get; set; }

            public string Property { get; set; }

            public string Descr { get; set; }

            public string OldID { get; set; }

            public string NewID { get; set; }

            public string Enabled { get; set; }
        }

        private static readonly Dictionary<DeviceType, string> WagoTypes = new Dictionary<DeviceType, string>
        {
            { DeviceType.V, "V" },
            { DeviceType.LS, "LS"},
            { DeviceType.TE, "TE"},
            { DeviceType.FQT, "CTR"},
            { DeviceType.FS, "FS" },
            { DeviceType.AO, "AO"},
            { DeviceType.LT, "LE"},
            { DeviceType.DI, "FB"},
            { DeviceType.DO, "UPR"},
            { DeviceType.QT, "QE"},
            { DeviceType.AI, "AI"},
        };


        /// <summary>
        /// Получить старое название устройства
        /// </summary>
        /// <param name="device"></param>
        public static string ToWagoDevice(IODevice device)
        {
            if (WagoTypes.TryGetValue(device.DeviceType, out var wagoType))
            {
                if (device.ObjectName == "TANK")
                    return $"{wagoType}{device.ObjectNumber}{device.DeviceNumber:00}";

                if (device.ObjectName.Length == 1)
                    return $"{wagoType}{(device.ObjectName[0] - 'A') * 20 + 200 + device.ObjectNumber}{device.DeviceNumber:00}";
            }

            Logs.AddMessage($"Старое название тега для устройства {device.Name} не указано, сигнатура устройства не распознана\n");
            return "";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newChannelDB"></param>
        /// <param name="oldChannelDB"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static string ModifyDescription(string oldChannelDB, string newChannelDB, IEnumerable<(string devName, string wagoName)> devices)
        {
            var tags = GetNewTagsValueAndState(ParseChannelsBase(newChannelDB));

            foreach (var dev in devices.Where(d => d.wagoName == string.Empty || d.devName == string.Empty || tags.All(t => t.Name != d.devName)))
            {
                Logs.AddMessage($"Тег устройства {dev.devName} ({dev.wagoName}) не найден в базе каналов\n");
            }

            var namesToReplaced = devices
                .Where(d => d.wagoName != string.Empty && d.devName != string.Empty && tags.Any(t => t.Name == d.devName))
                .ToDictionary(j => j.wagoName, j => tags.First(t => t.Name == j.devName).Descr);

            var replaceRegex = new Regex($@"(?<=<channels:descr>)(?<wago_name>{string.Join("|", namesToReplaced.Keys)}) :[\w\W]*?(?=<\/channels:descr>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(10000));


            return replaceRegex.Replace(oldChannelDB, m => namesToReplaced[m.Groups["wago_name"].Value]);
        }


        /// <summary>
        /// Изменить ID новой базы каналов на старые ID тегов
        /// </summary>
        /// <param name="newChannelDB">Текст новой базы каналов</param>
        /// <param name="oldChannelDB">Текст старой базы каналов</param>
        /// <param name="devices">Список новых и старых названий устройств</param>
        public static string ModifyID(string newChannelDB, string oldChannelDB, IEnumerable<(string devName, string wagoName)> devices)
        {
            var oldChannelsTags = ParseChannelsBase(oldChannelDB);
            var newChannelsTags = ParseChannelsBase(newChannelDB);

            var tags = LeftJoinNewTagsWithOldTags(
                GetNewTagsValueAndState(newChannelsTags),
                oldChannelsTags, devices);

            foreach (var tag in tags.Where(t => t.NewID is null || t.OldID is null))
            {
                Logs.AddMessage($"Тег устройства {tag.Name} ({tag.WagoName}) не найден в базе каналов\n");
            }

            var IdToReplaced = tags
                .Where(t => t.NewID != null && t.OldID != null)
                .ToDictionary(j => j.NewID, j => j.OldID);

            var IdEnable = tags
                .Where(t => t.NewID != null && t.OldID != null)
                .ToDictionary(j => j.NewID, j => j.Enabled);

            var replaceEnableRegex = new Regex($@"(?<=<channels:id>(?<id>{string.Join("|", IdEnable.Keys)})<\/channels:id>[\d\w\s</>:]*<channels:enabled>)0",
                RegexOptions.None, TimeSpan.FromMilliseconds(10000));

            // enable used tags
            var chbaseWithEnabled = replaceEnableRegex.Replace(newChannelDB, m => IdEnable[m.Groups["id"].Value]);

            var replaceRegex = new Regex($@"(?<=<channels:id>){string.Join("|", IdToReplaced.Keys)}(?=<\/channels:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(10000));

            return replaceRegex.Replace(chbaseWithEnabled, m => IdToReplaced[m.Value]);
        }


        /// <summary>
        /// Фильтрация новых тегов из базы каналов ( выборка тегов .V или .ST )
        /// </summary>
        /// <param name="newChannelsID">Теги новой базы каналов</param>
        private static IEnumerable<Tag> GetNewTagsValueAndState(IEnumerable<(string description, string id, string enabled)> newChannelsID)
        {
            return from xml in newChannelsID
                   select new Tag
                   {
                       Name = xml.description.Split('.')[0],
                       Property = xml.description.Split('.')[1],
                       Descr = xml.description,
                       NewID = xml.id
                   } into tag
                   where tag.Property == "V" || tag.Property == "ST"
                   group tag by tag.Name into tagGroup
                   select tagGroup.FirstOrDefault(g => g.Property == "V") ?? tagGroup.FirstOrDefault(g => g.Property == "ST");
        }

        /// <summary>
        /// Присоединение по устройствам проекта новой и старой базы каналов (name - wagoName - oldID - newID)
        /// </summary>
        /// <param name="tags">Новые теги</param>
        /// <param name="oldChannelsTags">Старые теги</param>
        /// <param name="devices">Новые и старые названия устройств</param>
        private static IEnumerable<Tag> LeftJoinNewTagsWithOldTags(
            IEnumerable<Tag> tags,
            IEnumerable<(string description, string id, string enabled)> oldChannelsTags,
            IEnumerable<(string devName, string wagoName)> devices)
        {
            return from dev in devices
                   join tag in tags on dev.devName equals tag.Name into allTags
                   from nullableTag in allTags.DefaultIfEmpty()
                   select new { dev, newID = nullableTag?.NewID ?? null } into newTag
                   join oldTag in oldChannelsTags on newTag.dev.wagoName equals oldTag.description into allOldTags
                   from nullableOldTag in allOldTags.DefaultIfEmpty()
                   select new Tag
                   {
                       Name = newTag.dev.devName,
                       WagoName = newTag.dev.wagoName,
                       OldID = nullableOldTag.id,
                       NewID = newTag.newID,
                       Enabled = nullableOldTag.enabled,
                   };
        }


        /// <summary>
        /// Получить список тегов (description(название тега) - ID) из текста базы каналов
        /// </summary>
        /// <param name="channelBaseData">Текст базы каналов</param>
        public static IEnumerable<(string description, string id, string enabled)> ParseChannelsBase(string channelBaseData)
        {
            var result = new List<(string, string, string)>();

            var regex = new Regex(
                @"<channels:channel>(?:[\s\S]*?)<channels:id>(?<id>\d*?)<\/channels:id>(?:[\s\S]*?)<channels:enabled>(?<enabled>-?\d*?)</channels:enabled>(?:[\s\S]*?)<channels:descr>(?<descr>[\s\S]*?)\s?(?::|<\/)(?:[\s\S]*?)<\/channels:channel>",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            var matches = regex.Matches(channelBaseData);

            foreach (Match match in matches)
            {
                if (!match.Success) 
                    continue;

                 result.Add((match.Groups["descr"].Value, match.Groups["id"].Value, match.Groups["enabled"].Value));
            }

            return result;
        }


        /// <summary>
        /// Выключить все теги
        /// </summary>
        public static string DisableAllSubtypesChannels(string chbase)
        {
            var regex = new Regex(@"(?<=<(channels|subtypes):enabled>)(?:-?\d*?)(?=<\/(channels|subtypes):enabled>)",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            return regex.Replace(chbase, "0");
        }

        /// <summary>
        /// Сместить все ID устройств
        /// </summary>
        /// <param name="chbase">База каналов</param>
        /// <param name="bit_offset">Смещение, по-умолчанию 0x8000</param>
        public static string ShiftID(string chbase, int bit_offset = 0b1000_0000_0000_0000)
        {
            var replaceChannelIdRegex = new Regex(@"(?<=<channels:id>)\d*?(?=<\/channels:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            return replaceChannelIdRegex.Replace(chbase,
                m => $"{int.Parse(m.Value) | bit_offset}");
        }


        /// <summary>
        /// Сместить типы в базе каналов
        /// </summary>
        /// <param name="chbase">База каналов</param>
        /// <param name="offset">Смещение</param>
        public static string ShiftSubtypeID(string chbase, int offset)
        {
            var replaceTypeSidRegex = new Regex(@"(?<=<subtypes:sid>)\d*?(?=<\/subtypes:sid>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            var replaceIDRegex = new Regex(@"(?<=<channels:id>)\d*?(?=<\/channels:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            var idOffset = offset << 16;

            chbase = replaceTypeSidRegex.Replace(chbase, m => $"{int.Parse(m.Value) + offset}");
            return replaceIDRegex.Replace(chbase, m => $"{int.Parse(m.Value) + idOffset}");
        }


        /// <summary>
        /// Изменить индекс драйвера (также изменяются и ID драйвера всех каналов)
        /// </summary>
        /// <param name="chbase">база каналов</param>
        /// <param name="driverID">Новый индекс драйвера</param>
        public static string ModifyDriverID(string chbase, int driverID)
        {
            var replaceDriverIdRegex = new Regex(@"(?<=<driver:id>)\d*?(?=<\/driver:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            chbase = replaceDriverIdRegex.Replace(chbase, driverID.ToString());

            var replaceChannelIdRegex = new Regex(@"(?<=<channels:id>)\d*?(?=<\/channels:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(1000));


            var channelIdDriverPref = driverID << 24;

            return replaceChannelIdRegex.Replace(chbase,
                m => $"{int.Parse(m.Value) & 0x00FFFFFF | channelIdDriverPref}");
        }


        /// <summary>
        /// Проверка индексов базы каналов на повторения
        /// </summary>
        public static void CheckChbaseID(string chbase)
        {
            var regex = new Regex(@"<channels:id>(?<id>\d*?)<\/channels:id>",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(1000));

            var set = new HashSet<string>();

            foreach (Match match in regex.Matches(chbase))
            {
                var id = match.Groups["id"].Value;
                if (!set.Add(id))
                {
                    Logs.AddMessage($"Ошибка: канал с ID:{id} уже существует\n");
                }
            }
        }


        /// <summary>
        /// Получить индекс драйвера
        /// </summary>
        /// <param name="chbase">База каналов</param>
        /// <returns>ID драйвера (1 - если не распознан)</returns>
        public static int GetDriverID(string chbase)
        {
            var getDriverRegex = new Regex(@"(?<=<driver:id>)\d*(?=</driver:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(100));

            var matchDriverID = getDriverRegex.Match(chbase);
            if (matchDriverID.Success)
                return int.Parse(matchDriverID.Value);

            return 1;
        }


        /// <summary>
        /// Получение первого свободного ID базы каналов
        /// </summary>
        /// <param name="chbase">Базаф каналов</param>
        public static int GetFreeSubtypeID(string chbase)
        {
            var getSubtypeIDRegex = new Regex(@"(?<=<subtypes:sid>)\d*(?=</subtypes:sid>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(100));

            var matches = getSubtypeIDRegex.Matches(chbase);

            int res = -1;
            foreach (Match match in matches)
            {
                res = Math.Max(res, int.Parse(match.Value));
            }

            return res + 1;
        }


    }
}
