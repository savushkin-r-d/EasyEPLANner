using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    /// <summary>
    /// Объект аппаратов проекта.
    /// </summary>
    public class Unit : TreeViewItem
    {
        public Unit()
        {
            objects = new List<ITreeViewItem>();
        }

        #region реализация ITreeViewItem
        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override string[] DisplayText
        {
            get
            {
                return new string[] { $"Аппарат ({Items.Length})", "" };
            }
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }
        #endregion

        List<ITreeViewItem> objects;
    }
}
