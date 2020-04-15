namespace Device
{
    /// <summary>
    /// Технологическое устройство - световая сигнализация.
    /// </summary>
    public class HL : IODevice
    {
        public HL(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HL;
            ArticleName = articleName;

            DO.Add(new IOChannel("DO", -1, -1, -1, ""));
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

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HL:
                    return dt.ToString();
            }
            return "";
        }
    }
}
