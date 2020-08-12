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
                if (objects.Count == 0)
                {
                    return new string[] { name, "" };
                }
                else
                {
                    return new string[] { $"{name} ({objects.Count})", "" };
                }
            }
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            //TODO: set TechType
            //TODO: nameBC set
            ObjectsAdder.Reset();
            var newObject = new TechObject(baseTechObject.Name,
                objects.Count + 1, 1, baseTechObject.EplanName.ToUpper(), -1,
                baseTechObject.EplanName, "");
            objects.Add(newObject);
            return newObject;
        }
        #endregion

        List<ITreeViewItem> objects;
        BaseTechObject baseTechObject;
    }
}
