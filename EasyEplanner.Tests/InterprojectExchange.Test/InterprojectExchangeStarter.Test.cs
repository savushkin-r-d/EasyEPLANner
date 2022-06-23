using System;
using NUnit.Framework;
using InterprojectExchange;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectExchangeStarterTest
    {

        [Test]
        public void SaveTest()
        {
            var ipe = new InterprojectExchangeStarter();
            InterprojectExchange.InterprojectExchange.GetInstance()
                 .AddModel(new AdvancedProjectModel());

            Assert.Throws<ApplicationException>(() => ipe.Save());
        }

    }
}
