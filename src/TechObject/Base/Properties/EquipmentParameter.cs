using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Параметр оборудования объекта
    /// </summary>
    public class EquipmentParameter : BaseParameter
    {
        public EquipmentParameter(string luaName, string name)
            : base(luaName, name) { }

        public EquipmentParameter(string luaName, string name,
            string defaultValue) : base(luaName, name, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveParameter(this.LuaName, this.Name,
                this.DefaultValue);
            newProperty.SetNewValue(this.Value);
            newProperty.NeedDisable = this.NeedDisable;
            return newProperty;
        }
    }
}
