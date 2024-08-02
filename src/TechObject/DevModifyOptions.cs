using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public interface IDevModifyOptions 
    {
        bool IsUnit { get; }

        bool NumberModified { get; }

        bool NameModified { get; }

        string OldTechObjectName { get; }

        int OldTechObjectNumber { get; }

        string NewTechObjectName { get; }

        int NewTechObjectNumber { get; }
    }


    public class DevModifyOptions : IDevModifyOptions
    {
        private TechObject techObject;
        private string oldTechObjectName;
        private int oldTechObjectNumber;

        public DevModifyOptions(TechObject techObject, string oldTechObjectName, int oldTechObjectNumber) 
        { 
            this.techObject = techObject;
            this.oldTechObjectName = oldTechObjectName;
            this.oldTechObjectNumber = oldTechObjectNumber;
        }


        public bool NumberModified => OldTechObjectNumber != NewTechObjectNumber;

        public bool NameModified => OldTechObjectName != NewTechObjectName;

        public bool IsUnit => techObject.BaseTechObject.S88Level == (int)BaseTechObjectManager.ObjectType.Unit;

        public string OldTechObjectName => oldTechObjectName;

        public int OldTechObjectNumber => oldTechObjectNumber;

        public string NewTechObjectName => techObject.NameEplan;

        public int NewTechObjectNumber => techObject.TechNumber;

    }
}
