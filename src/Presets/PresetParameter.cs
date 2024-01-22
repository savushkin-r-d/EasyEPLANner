using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner
{
    public class PresetParameter : TreeViewItem
    {
        public PresetParameter(Param parameter, ITechObjectManager techObjectManager)
        {
            this.Parameter = parameter;
            this.techObjectManager = techObjectManager;

            double.TryParse(parameter.GetValue(), out value);
            Index = parameter.GetParameterNumber;
        }

        public string SaveAsLuaTable(string prefix) 
            => $"{prefix}[ {Index} ] = {$"{Value},", -8} " +
               $"-- {$"{Parameter.GetMeter()}.", -5} [{Parameter.GetName()}]\n";

        public override string[] DisplayText 
            => new string[] 
            { 
                $"{GetIndex()}. {Parameter.GetName()}",
                $"{Value} {Parameter.GetMeter()}" 
            };

        public int GetIndex()
        {
            var index = Parameter.GetParameterNumber;
            if (index == 0)
            {
                (Parent as PresetTechObject)?.PresetParameters.Remove(this);
                Editor.Editor.GetInstance().RefreshObject(Parent);
            }

            Index = index;
            return index;
        }

        public override string[] EditText => new string[] { string.Empty, Value.ToString() };


        public override bool IsEditable => true;

        public override int[] EditablePart => new int[] { -1, 1 };


        public override bool SetNewValue(string newValue)
            => double.TryParse(newValue, out value);

        public override bool IsDeletable => true;

        private double value;

        public double Value 
        { 
            get => value;
            set => this.value = value; 
        }

        public int Index { get; private set; }

        public Param Parameter { get; private set; }

        private ITechObjectManager techObjectManager;
    }
}
