using BrightIdeasSoftware;
using Editor;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class GroupableParameters : ActiveBoolParameter
    {
        public GroupableParameters(string luaName, string name, bool main)
            : base(luaName, name, "false", null)
        {
            Main = main;
        }

        public GroupableParameters(string luaName, string name,
            string defaultValue, List<DisplayObject> displayObjects, bool main)
            : base(luaName, name, defaultValue, displayObjects)
        {
            Main = main;
        }

        public bool Main { get; set; }

        public override string[] DisplayText => 
            Main ? base.DisplayText : [Name, ""];

        public override bool IsEditable => Main;

        public override bool SetNewValue(string newValue)
        {
            var succes = base.SetNewValue(newValue);
            SetUpParametersVisibility();

            return succes;
        }

        private bool Enabled => Value is trueLogicValue || !Main;

        public virtual void SetUpParametersVisibility()
        {
            foreach (var item in Items)
            {
                item.Visibility = Enabled;
            }
        }

        public override BaseParameter Clone()
        {
            var clone = new GroupableParameters(LuaName, Name, Main)
            {   
                NeedDisable = NeedDisable,
            };

            clone.Parameters = [.. Parameters.Select(p => { 
                var clonedParameter = p.Clone();
                clonedParameter.Owner = clone;
                return clonedParameter; 
            })];

            clone.SetNewValue(Value);

            return clone;
        }

        public GroupableParameters AddGroupParameter(string luaName,
            string name, bool main)
        {
            var par = new GroupableParameters(luaName, name, main);
            InitParameter(par, main);
            return par;
        }

        public ActiveParameter AddActiveParameter(string luaName,
            string name, string defaultValue)
        {
            var par = new ActiveParameter(luaName, name, defaultValue);
            InitParameter(par);
            return par;
        }

        public ActiveBoolParameter AddActiveBoolParameter(string luaName,
            string name, string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name, defaultValue);
            InitParameter(par);
            return par;
        }

        private void InitParameter(BaseParameter parameter, bool main = true)
        {
            parameter.Owner = this;
            parameter.BaseOperation = BaseOperation;

            Parameters.Add(parameter);

            if (main)
                AddParameter(parameter);

            parameter.ValueChanged += OnValueChanged;
        }

        public void AddParameter(BaseParameter parameter)
        {
            if (Owner is GroupableParameters group)
                group.AddParameter(parameter);

            if (Owner is BaseOperation baseOperation)        
                baseOperation.Properties.Add(parameter);
        }

        public void AddFloatParameter(string luaName, string name,
            double value, string meter)
        {
            // do nothing
        }


        public override ITreeViewItem[] Items => Enabled ? [.. Parameters] : [];

        public List<BaseParameter> Parameters { get; set; } = [];
    }
}
