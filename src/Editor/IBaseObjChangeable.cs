using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public interface IBaseObjChangeable
    {
        /// <summary>
        /// Изменить базовый объект у объекта
        /// </summary>
        /// <param name="techObject">Объект для изменений</param>
        void ChangeBaseObj(ITreeViewItem techObject);
    }
}
