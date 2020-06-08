using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class MainAggregateParameter : ActiveBoolParameter
    {
        public MainAggregateParameter(string luaName, string name,
            string defaultValue) : base(luaName, name, defaultValue)
        { }

        public override BaseParameter Clone()
        {
            var newProperty = new MainAggregateParameter(this.LuaName, 
                this.Name, this.DefaultValue);
            newProperty.SetValue(this.Value);
            return newProperty;
        }

        public override bool SetNewValue(string newValue)
        {
            var result = base.SetNewValue(newValue);
            if (Value == "false")
            {
                DisableOwnerProperties();
            }

            return result;
        }

        private void DisableOwnerProperties()
        {
            var parentMode = Parent as Mode;
            var ownerProperties = (Owner as BaseTechObject).BaseOperations
                .Where(x => x.LuaName == parentMode.BaseOperation.LuaName)
                .First().Properties;

            foreach(var property in parentMode.BaseOperation.Properties)
            {
                var foundProperty = ownerProperties
                    .Where(x => x.LuaName == property.LuaName)
                    .FirstOrDefault();
                property.NeedDisable = true;
            }
        }
    }
}
