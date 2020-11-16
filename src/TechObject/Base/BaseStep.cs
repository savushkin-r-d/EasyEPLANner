using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class BaseStep
    {
        public BaseStep(string name, string luaName, int defaultPosition = 0)
        {
            Name = name;
            LuaName = luaName;
            DefaultPosition = defaultPosition;
        }

        public BaseStep Clone()
        {
            var step =  new BaseStep(Name, LuaName, DefaultPosition);
            step.Owner = Owner;
            return step;
        }

        public string Name { get; private set; }

        public string LuaName { get; private set; }

        public int DefaultPosition { get; private set; } = 0;

        /// <summary>
        /// Объект-владелец параметра.
        /// </summary>
        public object Owner { get; set; }
    }
}
