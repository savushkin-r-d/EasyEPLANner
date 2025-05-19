using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Общие методы для <see cref="ActionCustom"/> и 
    /// устаревшего <see cref="ActionWash"/> во избежание дубликации
    /// </summary>
    public abstract class BaseActionCustom : GroupableAction
    {
        protected BaseActionCustom(string name, Step owner, string luaName) 
            : base(name, owner, luaName)
        {
        }

        public override bool CanMoveUp(object child) => false;

        public override bool CanMoveDown(object child) => false;
    }
}
