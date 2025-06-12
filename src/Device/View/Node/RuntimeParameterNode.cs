using EplanDevice;

namespace EasyEPlanner
{
    public class RuntimeParameterNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (int.TryParse(newValue, out int parValue))
            {
                device.SetRuntimeParameter(Name, parValue);
                device.UpdateRuntimeParameters();
            }
            else Value = oldValue;
        }
    }
}
