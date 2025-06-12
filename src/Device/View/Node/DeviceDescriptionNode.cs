using EplanDevice;

namespace EasyEPlanner
{
    public class DeviceDescriptionNode(string value) : ColumnNode("Описание", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            device.Function.Description = newValue;
        }
    }
}
