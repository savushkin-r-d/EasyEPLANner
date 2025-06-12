using EplanDevice;

namespace EasyEPlanner
{
    public class DeviceArticleNode(string value) : ColumnNode("Изделие", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            Value = oldValue;
        }
    }
}
