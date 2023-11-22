using System.Collections.Generic;
using Editor;

namespace TechObject
{
    public class UserObject : BaseObject
    {
        public UserObject(ITechObjectManager techObjectManager)
            : base(Name, techObjectManager) { }

        public const string Name = "Пользовательский объект";
        public override string DefaultEplanName => "USER";
    }
}
