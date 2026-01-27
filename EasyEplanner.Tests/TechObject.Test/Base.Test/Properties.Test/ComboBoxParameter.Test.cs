using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace TechObjectTests
{
    internal class ComboBoxParameterTest
    {

        [TestCase("1", true, "AND")]
        [TestCase("0", true, "OR")]
        [TestCase("3", true, "OR")]
        [TestCase(null, true, "OR")]
        public void SetNewValueTest(string value, bool expRes, string expDisplay)
        {
            var combobox = new ComboBoxParameter("", "",
                new Dictionary<string, string>()
                {
                    { "OR", "0" },
                    { "AND", "1" },
                });

            var res = combobox.SetNewValue(value);

            Assert.AreEqual(expRes, res);
            Assert.AreEqual(expDisplay, combobox.DisplayText[1]);
        }

    }
}
