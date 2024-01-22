using EasyEPlanner.PxcIolinkConfiguration.Models;
using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner
{
    public class PresetTechObjectsContainer : TreeViewItem
    {
        public PresetTechObjectsContainer(ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
            PresetsTechObjects = new List<PresetTechObject> { };
        }

        public override string[] DisplayText => new string[] { "Параметры объектов", string.Empty };

        public override bool IsEditable => true;

        public override bool SetNewValue(IDictionary<int, List<int>> newDictionary)
        {
            PresetsTechObjects.RemoveAll(obj => newDictionary.Keys.Contains(obj.Index) is false);

            foreach (var objIndexParsPair in newDictionary)
            {
                var presetTechObject = PresetsTechObjects.FirstOrDefault(obj => obj.Index == objIndexParsPair.Key);

                if (presetTechObject is null)
                {
                    presetTechObject = new PresetTechObject(techObjectManager.GetTObject(objIndexParsPair.Key), techObjectManager);
                    PresetsTechObjects.Add(presetTechObject);
                    presetTechObject.AddParent(this);
                }

                presetTechObject.SetNewValue(objIndexParsPair.Value);
            }

            return true;
        }

        public override ImageIndexEnum ImageIndex => ImageIndexEnum.ParamsManager;

        public override ITreeViewItem[] Items => PresetsTechObjects.ToArray();

        public string SaveAsLuaTable(string prefix)
        {
            if (PresetsTechObjects.Count == 0)
                return string.Empty;
            return new StringBuilder()
                .Append(prefix).Append($"tech_objects = {{\n")
                .Append(string.Join("", PresetsTechObjects.Select(pto => pto.SaveAsLuaTable(prefix + "\t"))))
                .Append(prefix).Append("},\n")
                .ToString();
        }

        public void AddParam(int objIndex, int paramIndex, double value)
        {
            var presetTechObject = PresetsTechObjects.FirstOrDefault(obj => obj.Index == objIndex);
            if (presetTechObject is null)
            {
                presetTechObject = new PresetTechObject(techObjectManager.GetTObject(objIndex), techObjectManager);
                PresetsTechObjects.Add(presetTechObject);
                presetTechObject.AddParent(this);
            }

            presetTechObject.AddParam(paramIndex, value);
        }

        public List<PresetTechObject> PresetsTechObjects { get; private set; }

        private ITechObjectManager techObjectManager;
    }
}
