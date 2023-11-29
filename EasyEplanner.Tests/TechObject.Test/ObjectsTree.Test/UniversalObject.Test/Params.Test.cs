using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TechObject;

namespace TechObjectTests
{
    public class ParamsTest
    {
        [Test]
        public void UpdateOnGenericTechObject()
        {
            bool paramUpdateMethodCalled = false;

            var parameters = new Params("параметры", "params", false, "");
            var genericParameters = new Params("параметры", "params", false, "");

            genericParameters.AddParam(new Param(GetN => 1, "par 1", false, 0, "c", "par_1", true));
            genericParameters.AddParam(new Param(GetN => 1, "par 2", false, 0, "c", "par_2", true));

            var paramMock = new Mock<Param>(new GetN(x => 1), "par 1", false, 0, "c", "par_1", true);
            paramMock.Setup(p => p.UpdateOnGenericTechObject(It.IsAny<Param>()))
                .Callback<ITreeViewItem>(item =>
                {
                    Assert.AreSame(genericParameters.GetParam(0), item);
                    paramUpdateMethodCalled = true;
                });
            

            parameters.AddParam(paramMock.Object);

            Assert.Multiple(() =>
            {
                parameters.UpdateOnGenericTechObject(genericParameters);
                Assert.IsTrue(paramUpdateMethodCalled);
            });
        }

        [Test]
        public void Check()
        {
            var parameters = new Params("параметры", "params", false, "", true);
            var par1 = parameters.AddParam(new Param(GetN => 1, "par 1", false, 0, "c", "same_name", true));
            var par2 = parameters.AddParam(new Param(GetN => 2, "par 2", false, 0, "c", "same_name", true));

            parameters.Check("obj");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("P", par1.GetNameLua());
                Assert.AreEqual("P", par2.GetNameLua());
            });
        }
    }
}
