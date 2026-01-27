using EasyEPlanner;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.LogsTest
{
    internal class LogsTest
    {
        [Test]
        public void Log_Disable_Enable_buttons()
        {
            var mockLog = new Mock<ILog>();

            Logs.Init(mockLog.Object);
            Logs.EnableButtons();
            Logs.DisableButtons();

            mockLog.Verify(l => l.DisableButtons());
            mockLog.Verify(l => l.EnableButtons());
        }

    }
}
