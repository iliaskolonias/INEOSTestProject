using Services;
using System.Collections.Concurrent;

namespace INEOSTestProject
{
    public class INEOSTestService
    {
        private IPowerService _powerService;
        private readonly ILogger<INEOSTestService> _logger;
        CancellationTokenSource _tokenSource;
        private ConcurrentBag<Task<IEnumerable<PowerTrade>>> _runningTradeInstances;
        private ConcurrentBag<Task> _runningWriteInstances;
        private IConfiguration Configuration { get; set; }

        public INEOSTestService(IPowerService powerService, ILogger<INEOSTestService> logger, IConfiguration configuration)
        {
            _powerService = powerService;
            Configuration = configuration;
            _logger = logger;
            _tokenSource = new CancellationTokenSource();
            _runningTradeInstances = new ConcurrentBag<Task<IEnumerable<PowerTrade>>>();
            _runningWriteInstances = new ConcurrentBag<Task>();
        }

        internal async Task RunAsync()
        {
            //Set up and run new instance
            var timeToRun = DateTime.Now;
            var token = _tokenSource.Token;

            Task<IEnumerable<PowerTrade>> tradeTask = GetTradesAsync(timeToRun, token);
            _runningTradeInstances.Add(tradeTask);
            try
            {
                await Task.WhenAll(_runningTradeInstances.ToArray());
                foreach (var tradeInstance in _runningTradeInstances)
                {
                    var tradeSet = tradeInstance.Result;
                    if (default == tradeSet)
                    {
                        _logger.LogWarning($"Nothing returned from PowerService.GetTrades for time {timeToRun}");
                    }
                    else
                    {
                        PowerPositionCalculator powerPositionCalculator = new PowerPositionCalculator(_logger, timeToRun, tradeSet);
                        powerPositionCalculator.doCalc();
                        CSVWriter cSVWriter = new CSVWriter(Configuration, _logger, timeToRun);
                        Task writeTask = cSVWriter.doWrite(powerPositionCalculator, token);
                        _runningWriteInstances.Add(writeTask);

                        try
                        {
                            await Task.WhenAll(_runningWriteInstances.ToArray());
                        }
                        catch (OperationCanceledException oce)
                        {
                            _logger.LogWarning($"Operation cancelled - inner message: {oce.Message}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Operation error - inner message: {e.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogWarning($"Operation cancelled - inner message: {oce.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Operation error - inner message: {e.Message}");
                throw;
            }
            return;
        }

        internal async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            return await _powerService.GetTradesAsync(date);
        }

        internal IEnumerable<PowerTrade> GetTrades(DateTime date, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            return _powerService.GetTrades(date);
        }

    }
}