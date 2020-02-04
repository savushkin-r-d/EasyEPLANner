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
            items.Add(new OperationParam(par));
        }

        public void DeleteParam(Param par)
        {
            items.Remove(par);
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
