using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechObject;

namespace EasyEPlanner
{
    public class PresetsContainer : TreeViewItem
    {
        public PresetsContainer(ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
        }

        public override bool IsInsertable => true;

        public override ITreeViewItem Insert()
        {
            var preset = new Preset(techObjectManager);

            preset.AddParent(this);

            Presets.Add(preset);


            return preset;
        }

        public string SaveAsLuaTable(string prefix)
        {
            return $"Recipes = {{\n{string.Join("", Presets.Select(p => p.SaveAsLuaTable(prefix + "\t")))}}}";
        }

        public override string[] DisplayText 
            => new string[] {$"Пресеты {((Items.Count() > 0)? $"({Items.Count()})" : "")}", ""};

        public override ITreeViewItem[] Items => Presets.ToArray();

        public List<Preset> Presets { get; private set; } = new List<Preset>();

        private ITechObjectManager techObjectManager;
    }
}
