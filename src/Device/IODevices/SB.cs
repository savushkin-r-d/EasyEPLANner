namespace Device
{
    /// <summary>
    /// Технологическое устройство - кнопка.
    /// </summary>
    public class SB : IODevice
    {
        public SB(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.SB;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == "")
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }
    }
}
