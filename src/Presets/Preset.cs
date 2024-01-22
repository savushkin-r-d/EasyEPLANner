using Editor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using static Eplan.EplApi.DataModel.E3D.BasePointMate.Enums;

namespace EasyEPlanner
{
    

    public class Preset : TreeViewItem
    {
        public Preset(ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;

            PresetTechObjectsContainer = new PresetTechObjectsContainer(techObjectManager);
            presetDevices = new PresetDevices();

            PresetTechObjectsContainer.AddParent(this);
            presetDevices.AddParent(this);
        }

        public string SaveAsLuaTable(string prefix)
        {
            var techObjects = PresetTechObjectsContainer.SaveAsLuaTable(prefix + "\t");
            var devices = presetDevices.SaveAsLuaTable(prefix + '\t');

            var res = new StringBuilder()
                .Append(prefix).Append($"[ {(Parent as PresetsContainer).Presets.IndexOf(this) + 1} ] = {{\n")
                .Append(prefix).Append($"\tname = '{Name}',\n");

            if (techObjects != string.Empty)
                res.Append(techObjects);

            if (devices != string.Empty)
                res.Append(devices);

            res.Append(prefix).Append("},\n");

            return res.ToString().Replace("\t", "    "); ;
        }

        public void AddParam(int objIndex, int paramIndex, double value)
        {
            PresetTechObjectsContainer.AddParam(objIndex, paramIndex, value);
        }

        public void AddDev(string actionName, string devName)
        {
            presetDevices.AddDev(actionName, devName);
        }

        public override bool SetNewValue(string newName)
        {
            Name = newName;
            return true;
        }

        public const string PRESET_NAME = "Пресет";

        public override string[] DisplayText => new string[] { Name, string.Empty };

        public override string[] EditText => new string[] { Name, string.Empty };

        public override bool IsEditable => true;

        public override int[] EditablePart => new int[] { 0, -1 };

        public override ImageIndexEnum ImageIndex => ImageIndexEnum.Preset;

        public string Name { get; private set; } = PRESET_NAME;

        public override ITreeViewItem[] Items 
            => (new List<ITreeViewItem>() { PresetTechObjectsContainer, presetDevices }).ToArray();

        public override bool SetNewValue(IDictionary<int, List<int>> newDictionary)
        {
            return PresetTechObjectsContainer.SetNewValue(newDictionary);
        }

        public PresetTechObjectsContainer PresetTechObjectsContainer {  get; private set; }

        private PresetDevices presetDevices;

        private ITechObjectManager techObjectManager;
    }
}
