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
        public BaseObject(string baseTechObjectName)
        {
            objects = new List<ITreeViewItem>();
            baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(baseTechObjectName);
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
                var name = baseTechObject.Name;
                return new string[] { name, "" };
            }
        }
        #endregion

        List<ITreeViewItem> objects;
        BaseTechObject baseTechObject;
    }
}
