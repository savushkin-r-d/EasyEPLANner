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
    public class OperationParam : Editor.TreeViewItem
    {
        public OperationParam(Param par)
        {
            this.par = par;

            items = new List<Editor.ITreeViewItem>();
            items.Add(par.nameLua);
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

        private List<Editor.ITreeViewItem> items;
        private Param par;
    }
}
