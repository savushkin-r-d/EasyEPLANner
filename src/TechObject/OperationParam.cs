using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Параметр технологической операции.
    /// </summary>
    public class OperationParam : Editor.TreeViewItem, 
        BrightIdeasSoftware.IModelFilter
    {
        public OperationParam(Param par)
        {
            this.par = par;

            items = new List<Editor.ITreeViewItem>();
            items.Add(par.LuaNameProperty);
        }

        public Param Param
        {
            get
            {
                return par;
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return par.DisplayText;
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }
        #endregion

        /// <summary>
        /// Реализация Filter для IModelFilter
        /// </summary>
        /// <param name="filterobject">Фильтруемый объект</param>
        /// <returns></returns>
        public bool Filter(object filterobject)
        {
            //TODO: определение, когда нужно скрыть или показать объект.
            return true;
        }

        private List<Editor.ITreeViewItem> items;
        private Param par;
    }
}
