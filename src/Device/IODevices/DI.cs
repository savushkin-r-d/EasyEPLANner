namespace Device
{
    /// <summary>
    /// Технологическое устройство - дискретный вход.
    /// </summary>
    public class DI : IODevice
    {
        public DI(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DI_VIRT":
                    dSubType = DeviceSubType.DI_VIRT;
                    break;

                case "DI":
                case "":
                    parameters.Add("P_DT", null);
                    dSubType = DeviceSubType.DI;
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (DI, DI_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
}
