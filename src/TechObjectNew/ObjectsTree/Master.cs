using NewEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechObject
{
    /// <summary>
    /// Объект мастера проектов.
    /// </summary>
    public class Master : TreeViewItem
    {
        public Master() 
        {
            objects = new List<TechObject>();
        }

        public List<TechObject> Objects
        {
            get
            {
                return objects;
            }
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                return new string[] { $"Мастер ({objects.Count})", "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }
        #endregion

        private List<TechObject> objects;
    }
}
