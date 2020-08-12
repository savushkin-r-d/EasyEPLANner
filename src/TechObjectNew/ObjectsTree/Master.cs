using NewEditor;
using System.Collections.Generic;

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

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if(Items.Length > 0)
                {
                    return new string[] { $"{name} ({objects.Count})", "" };
                }
                else
                {
                    return new string[] { name, "" };
                }
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override ITreeViewItem Insert()
        {
            if(Items.Length == 0)
            {
                var newObject = new TechObject(name/*, GetTechObjectN*/, 
                    1, 1, "MASTER", -1, "MasterObj", "");
                objects.Add(newObject);
                return newObject;
            }
            else
            {
                return null;
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

        string name = "Мастер";
        List<TechObject> objects;
    }
}
