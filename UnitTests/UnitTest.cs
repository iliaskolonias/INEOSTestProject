using System;
using NUnit.Framework;
using Services;
using System.Collections.Generic;

namespace INEOSTestProject.UnitTests
{
    public class UnitTests
    {
        private DateTime testDate;
        private int numPeriods;
        PowerService ps;
        private IEnumerable<PowerTrade> _result;
        private IEnumerable<PowerTrade> _resultAsync;

        [SetUp]
        public void Setup()
        {
            ps = new PowerService();
            testDate = new DateTime(2022, 4, 15, 23, 0, 0, System.DateTimeKind.Local);
            numPeriods = 24;
            _result = new List<PowerTrade>()
            {
                PowerTrade.Create(testDate, numPeriods),   PowerTrade.Create(testDate, numPeriods), PowerTrade.Create(testDate, numPeriods),
                PowerTrade.Create(testDate, numPeriods),   PowerTrade.Create(testDate, numPeriods), PowerTrade.Create(testDate, numPeriods)
            };
            _resultAsync = new List<PowerTrade>()
            {
                PowerTrade.Create(testDate, numPeriods),   PowerTrade.Create(testDate, numPeriods), PowerTrade.Create(testDate, numPeriods),
                PowerTrade.Create(testDate, numPeriods),   PowerTrade.Create(testDate, numPeriods), PowerTrade.Create(testDate, numPeriods)
            };
        }

        [Test]
        public void TestCall()
        {
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestCallAsync()
        {
            foreach (var result in _resultAsync)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestAggregation()
        {
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestAggregationAsync()
        {
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }
    }
}