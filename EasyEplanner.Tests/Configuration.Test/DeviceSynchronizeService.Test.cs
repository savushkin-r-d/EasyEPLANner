using System.Collections.Generic;
using EasyEPlanner;
using NUnit.Framework;

namespace EasyEplanner.Tests
{
    public class DeviceSynchronizeServiceTest
    {
        [TestCaseSource(nameof(SynchronizeDevicesTestCaseSouse))]
        public void Synchronize_UseSynchronizeService_UpdateIndexesInList(
            int[] actualDevArray, List<int> actualDevIndexes,
            List<int> expectedDevIndexes)
        {
            IDeviceSynchronizeService deviceSynchronizeService =
                DeviceSynchronizer.GetSynchronizeService();

            deviceSynchronizeService.SynchronizeDevices(actualDevArray,
                ref actualDevIndexes);

            Assert.AreEqual(expectedDevIndexes, actualDevIndexes);
        }

        private static object[] SynchronizeDevicesTestCaseSouse()
        {
            var testCases = new object[]
            {
                new object[]
                {
                    new int[] { -2, -2, -2, -2, 5, 6, 7, -1, 8, 9, 10, 11, 12 },
                    new List<int> { 4, 7, 10 },
                    new List<int> { 5, 10 },
                },
                new object[]
                {
                    new int[0],
                    new List<int>(),
                    new List<int>(),
                }
            };

            return testCases;
        }
    }
}
