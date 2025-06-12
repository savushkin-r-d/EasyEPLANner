using EplanDevice;

namespace EasyEPlanner
{
    public class ParameterNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (double.TryParse(newValue, out double parValue))
            {
                var parameter = (IODevice.Parameter)Tag;
                device.SetParameter(parameter.Name, parValue);
                device.UpdateParameters();
            }
            else Value = oldValue;
        }
    }
}
