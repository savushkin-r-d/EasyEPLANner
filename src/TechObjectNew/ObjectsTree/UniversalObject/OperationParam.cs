using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    /// <summary>
    /// Параметр технологической операции.
    /// </summary>
    public class OperationParam : TreeViewItem
    {
        public OperationParam(Param par)
        {
            this.par = par;

            items = new List<ITreeViewItem>();
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

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }
        #endregion

        private List<ITreeViewItem> items;
        private Param par;
    }
}
