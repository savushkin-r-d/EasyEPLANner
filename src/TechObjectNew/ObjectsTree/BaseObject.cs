using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    class BaseObject : TreeViewItem
    {
        public BaseObject()
        {
            objects = new List<ITreeViewItem>();
        }

        public List<ITreeViewItem> Objects
        {
            get
            {
                return objects;
            }
        }

        List<ITreeViewItem> objects;
    }
}
