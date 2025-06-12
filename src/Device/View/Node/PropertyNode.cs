using EplanDevice;

namespace EasyEPlanner
{
    public class PropertyNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (device.MultipleProperties().Contains(Name) &&
                newValue.Contains(","))
            {
                device.SetProperty(Name, newValue);
                device.UpdateProperties();
            }
            else Value = oldValue;
        }
    }
}
