namespace Editor
{
    /// <summary>
    /// Класс для настройки цвета отображения на странице.
    /// </summary>
    public struct DrawInfo
    {
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
