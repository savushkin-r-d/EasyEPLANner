namespace NewEditor
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
        public DrawInfo(Style style, Device.Device dev)
        {
            this.style = style;
            this.dev = dev;
        }

        /// <summary>
        /// Отрисовываемое устройство
        /// </summary>
        public Device.Device DrawingDevice
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

            RED_BOX,
            GREEN_BOX,

            GREEN_UPPER_BOX,
            GREEN_LOWER_BOX,

            GREEN_RED_BOX,
        }

        private Device.Device dev;
        private Style style;
    };
}
