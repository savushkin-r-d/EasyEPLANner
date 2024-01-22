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
    public class PresetTechObject : TreeViewItem
    {
        public PresetTechObject(TechObject.TechObject techObject, ITechObjectManager techObjectManager) 
        {
            TechObject = techObject;
            Index = TechObject.GlobalNum;

            this.techObjectManager = techObjectManager;

            PresetParameters = new List<PresetParameter>();
        }

        public string SaveAsLuaTable(string prefix)
        {
            return new StringBuilder()
                .Append(prefix).Append($"[ {Index} ] = {{ -- {TechObject.Name} {TechObject.TechNumber}\n")
                .Append(prefix).Append($"\tparams = {{\n")
                .Append(string.Join("", PresetParameters.Select(par => par.SaveAsLuaTable(prefix + "\t\t"))))
                .Append(prefix).Append($"\t}},\n")
                .Append(prefix).Append($"}},\n")
                .ToString();
        }

        public void AddParam(int paramIndex, double value)
        {
            var presetParameter = PresetParameters.FirstOrDefault(p => p.Index == paramIndex);
            if (presetParameter is null)
            {
                presetParameter = new PresetParameter(TechObject.GetParamsManager().Float.GetParam(paramIndex - 1), techObjectManager);
                PresetParameters.Add(presetParameter);
                presetParameter.AddParent(this);
            }

            presetParameter.Value = value;
        }

        public bool SetNewValue(List<int> parameters)
        {
            PresetParameters.RemoveAll(p => parameters.Contains(p.Index) is false);

            foreach (var parIndex in parameters)
            {
                var presetParameter = PresetParameters.FirstOrDefault(p => p.Index == parIndex);

                if (presetParameter != null)
                    continue;

                presetParameter = new PresetParameter(TechObject.GetParamsManager().Float.GetParam(parIndex - 1), techObjectManager);
                PresetParameters.Add(presetParameter);
                presetParameter.AddParent(this);
            }

            CheckToDelete();

            PresetParameters = PresetParameters.OrderBy(p => p.Index).ToList();

            return true;
        }

        public override bool IsEditable => true;

        public override bool SetNewValue(IDictionary<int, List<int>> newDictionary)
            => newDictionary.TryGetValue(Index, out var value)
            ? SetNewValue(value) : SetNewValue(new List<int>());

        public override bool Delete(object child)
        {
            if (child is PresetParameter par)
            {
                PresetParameters.Remove(par);
                CheckToDelete();
                return true;
            }

            return false;
        }

        public void CheckToDelete()
        {
            if (PresetParameters.Count == 0)
            {
                Owner?.PresetsTechObjects.Remove(this);
            }
        }

        public override string[] DisplayText => new string[] { TechObject.DisplayText[0], string.Empty };


        public override ImageIndexEnum ImageIndex => ImageIndexEnum.TechObject;


        public override ITreeViewItem[] Items => PresetParameters.ToArray();


        public PresetTechObjectsContainer Owner => Parent as PresetTechObjectsContainer;

        public List<PresetParameter> PresetParameters { get; private set; }

        public int Index { get; private set; }

        public TechObject.TechObject TechObject { get; private set; }

        private ITechObjectManager techObjectManager;   
    }
}
