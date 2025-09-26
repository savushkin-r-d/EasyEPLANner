using BrightIdeasSoftware;
using Editor;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TechObject
{
    public class GroupableParameters : ActiveBoolParameter
    {
        public GroupableParameters(string luaName, string name, bool main, bool ignoreCompoundName)
            : base(luaName, name, "false", null)
        {
            Main = main;
            IgnoreCompoundName = ignoreCompoundName;
        }

        public GroupableParameters(string luaName, string name,
            string defaultValue, List<DisplayObject> displayObjects, bool main)
            : base(luaName, name, defaultValue, displayObjects)
        {
            Main = main;
            IgnoreCompoundName = true;
        }

        public bool Main { get; private set; }

        public bool IgnoreCompoundName { get; private set; }

        public override string[] DisplayText => 
            Main ? base.DisplayText : [Name, ""];

        public override bool IsEditable => Main;

        public override bool SetNewValue(string newValue)
        {
            var success = base.SetNewValue(newValue);
            SetUpParametersVisibility();

            return success;
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
            var clone = new GroupableParameters(LuaName, Name, Main, IgnoreCompoundName)
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
            string name, bool main, bool ignoreCompoundName)
        {
            var par = new GroupableParameters(luaName, name, main, ignoreCompoundName);
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
            if (!IgnoreCompoundName)
                parameter.LuaName = $"{LuaName}_{parameter.LuaName}";

            if (Owner is GroupableParameters group)
                group.AddParameter(parameter);

            if (Owner is BaseOperation baseOperation)        
                baseOperation.Properties.Add(parameter);
        }

        public override List<BaseParameter> GetDescendants()
        {
            var r = new List<BaseParameter>();
            if (Main)
                r.Add(this);

            r.AddRange(Parameters.SelectMany(p => p.GetDescendants()));
            return r;
        }


        public override ITreeViewItem[] Items => Enabled ? [.. Parameters] : [];

        public List<BaseParameter> Parameters { get; set; } = [];
    }
}
