using Editor;
using EplanDevice;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner
{
    public class PresetDevices : TreeViewItem
    {
        private const string onDevs = "opened_devices";
        private const string offDevs = "closed_devices";

        public PresetDevices()
        {
            Actions = new Dictionary<string, IAction>();
            Actions[onDevs] = new Action("Включать", null, onDevs)
            {
                ImageIndex = ImageIndexEnum.ActionON
            };
            Actions[offDevs] = new Action("Выключать", null, offDevs)
            {
                ImageIndex = ImageIndexEnum.ActionOFF
            };

            Actions[onDevs].AddParent(this);
            Actions[offDevs].AddParent(this);
        }

        public string SaveAsLuaTable(string prefix)
        {
            if (Actions[onDevs].Empty && Actions[offDevs].Empty)
                return string.Empty;
            return new StringBuilder()
                .Append(prefix).Append("devices = {\n")
                .Append(Actions[onDevs].SaveAsLuaTable(prefix + '\t'))
                .Append(Actions[offDevs].SaveAsLuaTable(prefix + '\t'))
                .Append(prefix).Append("},\n")
                .ToString();
        }

        public void AddDev(string actionName, string devName)
        {
            Actions[actionName]?.AddDev(deviceManager.GetDeviceIndex(devName), 0, "");
        }

        private static IDeviceManager deviceManager { get; set; } = DeviceManager.GetInstance();

        public override string[] DisplayText => new string[] { "Устройства", string.Empty };

        public override ITreeViewItem[] Items => Actions.Values.Cast<ITreeViewItem>().ToArray();

        private Dictionary<string, IAction> Actions;
    }
}
