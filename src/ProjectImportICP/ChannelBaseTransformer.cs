using EplanDevice;
using StaticHelper;
using System;
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

            public string OldID { get; set; }

            public string NewID { get; set; }
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
        /// Изменить ID новой базы каналов на старые ID тегов
        /// </summary>
        /// <param name="newChannelDB">Текст новой базы каналов</param>
        /// <param name="oldChannelDB">Текст старой базы каналов</param>
        /// <param name="devices">Список новых и старых названий устройств</param>
        public string TransformID(string newChannelDB, string oldChannelDB, IEnumerable<(string devName, string wagoName)> devices)
        {
            var oldChannelsTags = ParseChannelsBase(oldChannelDB);
            var newChannelsTags = ParseChannelsBase(newChannelDB);

            var tags = LeftJoinNewTagsWithOldTags(
                GetNewTagsValueAndState(newChannelsTags),
                oldChannelsTags, devices);

            foreach (var tag in tags.Where(t => t.NewID is null || t.OldID is null))
            {
                Logs.AddMessage($"Устройства {tag.Name} ({tag.WagoName}) не найдено в базе каналов\n");
            }

            var IdToReplaced = tags
                .Where(t => t.NewID != null && t.OldID != null)
                .ToDictionary(j => j.NewID, j => j.OldID);

            var replaceRegex = new Regex($@"(?<=<channels:id>){string.Join("|", IdToReplaced.Keys)}(?=<\/channels:id>)",
                RegexOptions.None, TimeSpan.FromMilliseconds(100));

            return replaceRegex.Replace(newChannelDB, m => IdToReplaced[m.Value]);
        }


        /// <summary>
        /// Фильтрация новых тегов из базы каналов ( выборка тегов .V или .ST )
        /// </summary>
        /// <param name="newChannelsID">Теги новой базы каналов</param>
        IEnumerable<Tag> GetNewTagsValueAndState(IEnumerable<(string description, string id)> newChannelsID)
        {
            return from xml in newChannelsID
                   select new Tag
                   {
                       Name = xml.description.Split('.')[0],
                       Property = xml.description.Split('.')[1],
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
        IEnumerable<Tag> LeftJoinNewTagsWithOldTags(
            IEnumerable<Tag> tags,
            IEnumerable<(string description, string id)> oldChannelsTags,
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
                       NewID = newTag.newID
                   };
        }


        /// <summary>
        /// Получить список тегов (description(название тега) - ID) из текста базы каналов
        /// </summary>
        /// <param name="channelBaseData">Текст базы каналов</param>
        public static IEnumerable<(string description, string id)> ParseChannelsBase(string channelBaseData)
        {
            var result = new List<(string, string)>();

            var regex = new Regex(
                @"<channels:channel>(?:[\s\S]*?)<channels:id>(?<id>\d*?)<\/channels:id>(?:[\s\S]*?)<channels:descr>(?<descr>[\s\S]*?)\s?(?::|<\/)(?:[\s\S]*?)<\/channels:channel>",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            var matches = regex.Matches(channelBaseData);

            foreach (Match match in matches)
            {
                if (!match.Success) 
                    continue;

                 result.Add((match.Groups["descr"].Value, match.Groups["id"].Value));
            }

            return result;
        }

    }
}
