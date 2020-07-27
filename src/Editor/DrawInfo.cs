namespace Editor
{
    /// <summary>
    /// Класс для настройки цвета отображения на странице.
    /// </summary>
    public struct DrawInfo
    {
        public DrawInfo(Style style, Device.Device dev)
        {
            this.style = style;
            this.dev = dev;
        }

        public void SetStyle(Style newStyle)
        {
            style = newStyle;
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

        public Device.Device dev;
        public Style style;
    };
}
