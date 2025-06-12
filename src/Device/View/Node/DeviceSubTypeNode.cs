using EplanDevice;
using System.Linq;

namespace EasyEPlanner
{
    public class DeviceSubTypeNode(string value) : ColumnNode("Подтип", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (device.DeviceType.SubTypeNames().Contains(newValue))
            {
                device.Function.SubType = newValue;

                Value = "Обновите проект";
            }
            else Value = oldValue;
        }
    }
}
