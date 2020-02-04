using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Все параметры технологической операции.
    /// </summary>
    public class OperationParams : Editor.TreeViewItem
    {
        public OperationParams()
        {
            items = new List<Editor.ITreeViewItem>();
        }

        public void AddParam(Param par)
        {
            bool containsThisParam = false;
            foreach (OperationParam param in items)
            {
                if (par.GetNameLua() == param.Param.GetNameLua())
                {
                    containsThisParam = true;
                }
            }

            if (!containsThisParam)
            {
                items.Add(new OperationParam(par));
            }
        }

        public void DeleteParam(Param paramForDelete)
        {
            foreach (OperationParam param in items)
            {
                if (paramForDelete.GetNameLua() == param.Param.GetNameLua())
                {
                    items.Remove(param);
                    break;
                }
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("{0} ({1})", "Параметры", 
                    items.Count);
                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                if (items.Count > 0)
                {
                    return items.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        private List<Editor.ITreeViewItem> items;
    }
}
