using InterprojectExchange;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerEasyEplannerTests.InterprojectExchangeTest
{

    public class ProjectModelTest
    {
        [Test]
        public void LoadedGetSetTest()
        {
            var model = new AdvancedProjectModel
            {
                Loaded = true
            };

            Assert.IsTrue(model.Loaded);
        }
    }
}
