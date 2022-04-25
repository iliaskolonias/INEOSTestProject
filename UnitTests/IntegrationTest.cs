using System;
using NUnit.Framework;
using Services;
using System.Collections.Generic;

namespace INEOSTestProject.IntegrationTests
{
    internal class IntegrationTest
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
    }

    [Test]
        public void TestCall()
        {
            _result = ps.GetTrades(testDate);
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestCallAsync()
        {
            _resultAsync = ps.GetTradesAsync(testDate).Result;
            foreach (var result in _resultAsync)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestAggregation()
        {
            _result = ps.GetTrades(testDate);
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }

        [Test]
        public void TestAggregationAsync()
        {
            _result = ps.GetTradesAsync(testDate).Result;
            foreach (var result in _result)
            {
                Assert.AreEqual(numPeriods, result.Periods.Length);
            }
        }
    }
}