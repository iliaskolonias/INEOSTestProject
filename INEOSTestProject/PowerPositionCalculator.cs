using Services;

namespace INEOSTestProject
{
    internal class PowerPositionCalculator
    {
        private readonly ILogger<INEOSTestService> _logger;
        internal PowerPosition powerPosition { get; set; }
        internal IEnumerable<PowerTrade> powerTrades { get; set; }
        internal PowerPositionCalculator(ILogger<INEOSTestService> logger, DateTime date, IEnumerable<PowerTrade> _powerTrades)
        {
            _logger = logger;
            powerPosition = new PowerPosition(date);
            powerTrades = _powerTrades;
        }

        internal void doCalc()
        {
            try
            {
                foreach (var powerTrade in powerTrades)
                {
                    if (null != powerTrade)
                    {
                        foreach (PowerPeriod pp in powerTrade.Periods)
                        {
                            powerPosition.Periods[pp.Period].Volume += pp.Volume;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                throw new INEOSTestProjectException(e.Message, e);
            }
        }
    }
}
