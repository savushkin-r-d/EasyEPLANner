using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Менеджер ограничений. Содержит ограничения среди всех объектов 
    /// и для конкретного объекта
    /// </summary>
    public class RestrictionManager : TreeViewItem
    {
        public RestrictionManager()
        {
            name = "Ограничения";
            restrictions = new List<Restriction>();

            var TotalResriction = new Restriction("Общие ограничения", "",
                "TotalRestriction", new SortedDictionary<int, List<int>>());
            var ThisObjResriction = new LocalRestriction(
                "Ограничения внутри объекта", "", "LocalRestriction", 
                new SortedDictionary<int, List<int>>());
            var NextModeResriction = new LocalRestriction(
                "Ограничения на последующие операции", "", 
                "NextModeRestriction", new SortedDictionary<int, List<int>>());
            restrictions.Add(ThisObjResriction);
            restrictions.Add(TotalResriction);
            restrictions.Add(NextModeResriction);

        }

        public RestrictionManager Clone()
        {
            RestrictionManager clone = (RestrictionManager)MemberwiseClone();
            clone.restrictions = new List<Restriction>();
            foreach (Restriction rest in restrictions)
            {
                clone.restrictions.Add(rest.Clone());
            }

            return clone;
        }

        public void CheckRestriction(int prev, int curr)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ChangeObjNum(prev, curr);
            }
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ChangeModeNum(objNum, prev, curr);
            }
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ModifyRestrictObj(oldObjN, newObjN);
            }
        }

        public void ChangeCrossRestriction(RestrictionManager oldRestrictMngr = null)
        {
            for (int i = 0; i < restrictions.Count; i++)
            {
                if (oldRestrictMngr != null)
                {
                    restrictions[i].ChangeCrossRestriction(oldRestrictMngr
                        .Restrictions[i].RestrictDictionary);
                }
                else
                {
                    restrictions[i].ChangeCrossRestriction();
                }
            }
        }

        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            int totalCount = 0;
            for (int i = 0; i < restrictions.Count; i++)
            {
                string tmp = restrictions[i]
                    .SaveRestrictionAsLua(prefix + "\t\t");
                if (tmp != "")
                {
                    res += prefix + "\t[ " + (i + 1).ToString() + " ] =\n" + 
                        tmp;
                }
                totalCount += restrictions[i].RestrictDictionary.Count;
            }

            if (totalCount == 0)
            {
                res = "";
            }

            return res;
        }

        public List<Restriction> Restrictions
        {

            get
            {
                return restrictions;
            }

        }

        #region реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, "" };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return restrictions.ToArray();
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            var restriction = child as Restriction;
            var copy = copyObject as Restriction;
            bool objectsNotNull = restriction != null && copy != null;
            if (objectsNotNull)
            {
                restriction.SetNewValue(copy.EditText[1]);

                restriction.AddParent(this);
                return restriction;
            }
            return null;
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=process_parameter";
        }

        private string name;
        private List<Restriction> restrictions;
    }
}
