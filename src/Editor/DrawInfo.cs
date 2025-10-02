using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    /// <summary>
    /// Класс для настройки цвета отображения на странице.
    /// </summary>
    public struct DrawInfo
    {
        /// <summary>
        /// Фильтр устройств для подсветки
        /// </summary>
        public static List<DrawInfo> Filter(List<DrawInfo> draw)
        {
            var groups = from drawInfo in draw
                         group drawInfo by drawInfo.DrawingDevice.Name into g
                         select g;

            return [.. groups.Select(g =>
            {
                var styles = g.Select(p => p.DrawingStyle);

                if (styles.Contains(Style.RED_BOX))
                    return new DrawInfo(Style.RED_BOX, g.First().DrawingDevice);

                if (styles.Distinct().Count(s => s != Style.NO_DRAW) > 1)
                    return new DrawInfo(Style.GREEN_GRAY_BOX, g.First().DrawingDevice);

                return g.First();
            })];
        }

        /// <summary>
        /// Фильтр устройств для подсветки с проверкойц конфликтов действий
        /// </summary>
        public static List<DrawInfo> FilterByActions(List<DrawInfo> draw)
        {
            var groups = from drawInfo in draw
                         group drawInfo by drawInfo.DrawingDevice.Name into g
                         select g;

            return [..groups.Select(g =>
            {
                var styles = g.Select(p => p.DrawingStyle);
                var actions = g.Select(p => p.Action);

                if (actions.Contains(ActionType.ON_DEVICE) &&
                    actions.Contains(ActionType.OFF_DEVICE))
                    return new DrawInfo(Style.RED_BOX, g.First().DrawingDevice);

                if (styles.Distinct().Count(s => s != Style.NO_DRAW) > 1)
                    return new DrawInfo(Style.GREEN_GRAY_BOX, g.First().DrawingDevice);

                return g.First();
            })];
        }


        /// <summary>
        /// Создание экземпляра класса DrawInfo
        /// </summary>
        /// <param name="style">Стиль отображения</param>
        /// <param name="dev">Устройство</param>
        public DrawInfo(Style style, EplanDevice.IDevice dev)
        {
            this.style = style;
            this.dev = dev;
        }

        /// <summary>
        /// Отрисовываемое устройство
        /// </summary>
        public EplanDevice.IDevice DrawingDevice
        {
            get
            {
                return dev;
            }

            set
            {
                dev = value;
            }
        }

        /// <summary>
        /// Стиль отрисовки
        /// </summary>
        public Style DrawingStyle
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        public enum Style
        {
            NO_DRAW,

            GRAY_BOX,
            GREEN_BOX,

            GREEN_UPPER_BOX,
            GREEN_LOWER_BOX,

            GREEN_GRAY_BOX,

            RED_BOX, // WRONG BINDING
        }

        public ActionType Action { get; set; } = ActionType.OTHER;

        public enum ActionType
        {
            ON_DEVICE,
            OFF_DEVICE,
            OTHER,
        }

        private EplanDevice.IDevice dev;
        private Style style;
    };
}
