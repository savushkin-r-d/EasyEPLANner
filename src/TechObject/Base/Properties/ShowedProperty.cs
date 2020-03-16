using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class ShowedBaseProperty : BaseProperty
    {
        public ShowedBaseProperty(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public ShowedBaseProperty(string luaName, string name) : base(luaName,
            name, true)
        { }

        public override BaseProperty Clone()
        {
            var newProperty = new ShowedBaseProperty(this.LuaName, this.Name,
                this.CanSave());
            newProperty.SetNewValue(this.Value);
            return newProperty;
        }

        #region реализация ItreeViewItem
        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        public override Editor.ITreeViewItem Replace(object child, 
            object copyObject)
        {
            return null; //TODO: надо ли этот метод?
        }
        #endregion
    }
}