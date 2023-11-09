using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Moq;
using NUnit.Framework;
using TechObject;

namespace TechObjectTests
{
    public class UserObjectTest
    {
        [Test]
        public void InsertCuttedCopy()
        {
            var techObjectManagerMock = new Mock<ITechObjectManager>();
            var techObjectParentMock = new Mock<ITreeViewItem>();

            techObjectParentMock.Setup(o => o.Cut(It.IsAny<TechObject.TechObject>())).Returns<TechObject.TechObject>(to => to);

            var userObject = new UserObject(techObjectManagerMock.Object);
            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "", -1, "", "", null);
            techObject.Parent = techObjectParentMock.Object;

            var method = typeof(UserObject).GetMethod("InsertCuttedCopy",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method.Invoke(userObject, new object[] { techObject });

            Assert.AreSame(techObject, userObject.Items.SingleOrDefault());
        }
    }
}
