using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner;
using Moq;
using NUnit.Framework;
using StaticHelper;

namespace EasyEplannerTests
{
    public class ConfigurationCheckerTest
    {
        [TestCase("1.0.0.1 - 1.0.0.9", "")]
        [TestCase("1.0.0.1 - 1.0.0.9, 1.0.0.11-1.0.0.19", "")]
        [TestCase("1.0.0.11", ConfigurationChecker.WrongIPRanges + ConfigurationChecker.ExampleIPRanges)]
        [TestCase("1.0.256.11-1.0.256.19", ConfigurationChecker.WrongIPRanges + ConfigurationChecker.ExampleIPRanges)]
        [TestCase("", ConfigurationChecker.WrongIPRanges + ConfigurationChecker.ExampleIPRanges)]
        public void CheckIPRanges(string ipRangesString, string expectedErrors)
        {
            var projectHelperMock = new Mock<IProjectHelper>();
            projectHelperMock.Setup(h => h.GetProjectProperty(It.IsAny<string>())).Returns(ipRangesString);

            var projectHealthCheckerMock = new Mock<IProjectHealthChecker>();

            var projectConfigurationMock = new Mock<IProjectConfiguration>();

            var configChecker = new ConfigurationChecker(projectHelperMock.Object,
                projectHealthCheckerMock.Object, projectConfigurationMock.Object);

            Assert.AreEqual(expectedErrors, configChecker.CheckProjectIPAddresses());
        }

    }
}
