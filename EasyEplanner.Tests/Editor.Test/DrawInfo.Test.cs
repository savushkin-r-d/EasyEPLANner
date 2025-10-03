using Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTest
{
    public class DrawInfoTest
    {
        [Test]
        [Repeat(30)]
        public void TestActionTypeAggregate()
        {
            var actions = GetRandomActionTypeList();

            var expected =
                actions.Contains(DrawInfo.ActionType.ON_DEVICE) && actions.Contains(DrawInfo.ActionType.OFF_DEVICE) ||
                actions.Contains(DrawInfo.ActionType.ON_DEVICE) && actions.Contains(DrawInfo.ActionType.DELAYED_ON_DEVICE) ||
                actions.Contains(DrawInfo.ActionType.OFF_DEVICE) && actions.Contains(DrawInfo.ActionType.DELAYED_OFF_DEVICE);

            var actual = actions.Aggregate((f, s) => f & s) == 0;

            Assert.AreEqual(expected, actual);
        }


        private List<DrawInfo.ActionType> GetRandomActionTypeList()
        {
            Array values = Enum.GetValues(typeof(DrawInfo.ActionType));
            Random random = new Random();

           return new List<DrawInfo.ActionType>()
            {
                (DrawInfo.ActionType)values.GetValue(random.Next(values.Length)),
                (DrawInfo.ActionType)values.GetValue(random.Next(values.Length)),
                (DrawInfo.ActionType)values.GetValue(random.Next(values.Length)),
                (DrawInfo.ActionType)values.GetValue(random.Next(values.Length)),
                (DrawInfo.ActionType)values.GetValue(random.Next(values.Length)),
            };
        }
    }
}
