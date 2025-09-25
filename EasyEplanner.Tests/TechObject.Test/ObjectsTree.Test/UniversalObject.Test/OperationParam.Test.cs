using Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace Tests.TechObject
{
    public class OperationParamTest
    {
        [Test]
        public void PropertiesTest()
        {
            var param = new Param(getN => 1, "название");
            var operationParam = new OperationParam(param);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(operationParam.IsEditable);
                CollectionAssert.AreEqual(param.EditText, operationParam.EditText);
                CollectionAssert.AreEqual(new int[]{ 0, 1 }, operationParam.EditablePart);
                CollectionAssert.AreEqual(new ITreeViewItem[] { param.ValueItem, param.MeterItem }, operationParam.Items);
            });
        }
    }
}
