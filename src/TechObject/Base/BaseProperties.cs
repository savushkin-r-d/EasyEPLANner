using Editor;
using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    public class BaseProperties : TreeViewItem
    {
        public BaseProperties()
        {
            Properties = new List<BaseParameter>();
        }

        public BaseProperties Clone()
        {
            var baseProperties = new BaseProperties();
            foreach(var property in Properties)
            {
                baseProperties.Properties.Add(property.Clone());
            }

            return baseProperties;
        }

        /// <summary>
        /// Добавить активный параметр
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        /// <returns>Добавленный параметр</returns>
        public ActiveParameter AddActiveParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveParameter(luaName, name, defaultValue);
            par.Owner = this;
            Properties.Add(par);
            return par;
        }

        /// <summary>
        /// Добавить активный булевый параметр
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddActiveBoolParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name,
                defaultValue);
            par.Owner = this;
            Properties.Add(par);
        }

        public int Count
        {
            get
            {
                return Properties.Count;
            }
        }

        #region реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("Доп. свойства ({0})", Items.Length);
                return new string[] { res, string.Empty };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return Properties.ToArray();
            }
        }

        public override bool Delete(object child)
        {
            if (child is ActiveParameter)
            {
                var property = child as ActiveParameter;
                property.SetNewValue(string.Empty);
                return true;
            }
            return false;
        }

        public override bool IsFilled
        {
            get
            {
                if (Items.Where(x => x.IsFilled).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public void Clear()
        {
            Properties.Clear();
        }

        public void Synch(int[] array)
        {
            foreach(var property in Properties)
            {
                property.Synch(array);
            }
        }

        public string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;

            if (Properties.Count == 0)
            {
                return res;
            }

            res += $"{prefix}properties =\n";
            res += $"{prefix}\t{{\n";
            foreach(var property in Properties)
            {
                res += $"{prefix}\t{property.LuaName} = " +
                    $"\'{property.Value}\',\n";
            }
            res += $"{prefix}\t}},\n";

            return res;
        }

        public BaseParameter GetProperty(string luaName)
        {
            return Properties.Where(x => x.LuaName == luaName).FirstOrDefault();
        }

        public List<BaseParameter> Properties { get; set; }
    }
}
