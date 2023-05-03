using NUnit.Framework;
using StaticHelper;
using System;
using System.Text.RegularExpressions;

namespace EasyEplannerTests.ConfigurationTest
{
    public class CommonConstPatterns
    {
        private MatchCollection MatchesRangesIP(string rangesIP_str)
        {
            return Regex
                .Matches(rangesIP_str, CommonConst.RangeOfIPAddresses,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
        }

        private Match MatchIP(string ip_str)
        {
            return Regex
                .Match(ip_str, CommonConst.IPAddressPattern,
                    RegexOptions.None, TimeSpan.FromMilliseconds(100));
        }


        [TestCase("1.2.3.4", "1.2.3.4")]
        [TestCase("0.111.000.255", "0.111.000.255")]
        [TestCase("  1.2.3.4  ", "1.2.3.4")]
        [TestCase(" 10.11.2.30", "10.11.2.30")]
        public void CommonConst_IPAddressPattern(string ipString, string expected)
        {
            var matchIP = MatchIP(ipString);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(matchIP.Success);
                Assert.AreEqual(expected, matchIP.Groups["ip"].Value);
            });
        }

        [TestCase("wrong string")]
        [TestCase("1.1.1.256")]
        [TestCase("1.1.1.-1")]
        [TestCase("1.1.1.a")]
        [TestCase(", 1.1.1.255 ")]
        public void CommonConst_IPAddressPattern_WrongCases(string ipString)
        {
            var matchIP = MatchIP(ipString);

            Assert.IsFalse(matchIP.Success);
        }

        [TestCase("1.2.3.4 - 1.2.3.10",
            new string[] { "1.2.3.4", "1.2.3.10" })]
        [TestCase("1.2.3.4 - 1.2.3.10, 1.2.3.15 - 1.2.3.20",
            new string[] { "1.2.3.4", "1.2.3.10", "1.2.3.15", "1.2.3.20" })]
        [TestCase("   000.000.000.000-000.000.000.100    , 1.1.1.1    -    1.1.1.2     ,",
            new string[] { "000.000.000.000", "000.000.000.100", "1.1.1.1", "1.1.1.2" })]
        public void CommonConst_RangeOfIPAddresses(string ipRangesString, string[] expected)
        {
            var matchesIP = MatchesRangesIP(ipRangesString);

            Assert.Multiple(() =>
            {
                int index = 0;
                foreach (Match matchIP in matchesIP)
                {
                    Assert.IsTrue(matchIP.Success);
                    Assert.AreEqual(expected[index * 2], matchIP.Groups["ip"].Captures[0].Value);
                    Assert.AreEqual(expected[(index * 2) + 1], matchIP.Groups["ip"].Captures[1].Value);
                    index++;
                }
            });
        }

        [TestCase("1.2.3.4 - 1.2.3.10.1")]
        [TestCase("1.2.3.4 - 1.2.3.10 . , ")]
        [TestCase("1.2.3.4 - 1 1.2.3.10")]
        public void CommonConst_RangeOfIPAddresses_WrongCases(string ipRangesString)
        {
            var matchesIP = MatchesRangesIP(ipRangesString);
            Assert.AreEqual(0, matchesIP.Count);
        }
    }
}
