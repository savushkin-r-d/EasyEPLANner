using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TechObject;

namespace TechObjectTests
{
    public class UserObjectTest
    {
        [Test]
        public void CreateUserObjectAndInsertTest()
        {
            var techobjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(o => o.TechObjects).Returns(techobjects);

            var baseTechObject = new BaseTechObject(null)
            {
                Name = "Пользовательский объект",
                EplanName = "user_object",
            };
            
            var userObject = new UserObject(techObjectManagerMock.Object);


            typeof(BaseObject).GetField("baseTechObject",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(userObject, baseTechObject);
            
            var resultTO = userObject.Insert() as TechObject.TechObject;

            Assert.Multiple(() =>
            {
                Assert.AreSame(techobjects[0], resultTO);
                Assert.AreSame("USER", resultTO.NameEplan);
            });
        }
    }
}
