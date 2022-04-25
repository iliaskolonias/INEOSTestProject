namespace INEOSTestProject
{
    public sealed class INEOSTestBackgroundService : BackgroundService
    {
        private readonly INEOSTestService _INEOSTestService;
        private readonly ILogger<INEOSTestBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _refreshInterval;

        public INEOSTestBackgroundService(INEOSTestService ineosTestService, IConfiguration configuration, ILogger<INEOSTestBackgroundService> logger)
        {
            _INEOSTestService = ineosTestService;
            _logger = logger;
            _configuration = configuration;

            _refreshInterval = _configuration.GetValue<int>("Configuration:RefreshInterval");
            if (default == _refreshInterval)
            {
                string errorString = "Configuration setting must be set to a positive integer value for key 'Configuration:RefreshInterval'";
                _logger.LogError(errorString);
                throw new ArgumentException(errorString);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowStamp = DateTimeOffset.Now;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await _INEOSTestService.RunAsync();
                }
                catch (AggregateException ae)
                {
                    _logger.LogError($"{ae.Message}");
                    _logger.LogError($"{ae.StackTrace}");
                    LogExceptions(ae.InnerExceptions);
                }
                finally
                {
                    //Trying to hit the closest minute to the required interval of N minutes - jump N-1, then see if the last jump takes us further than not
                    var newTime = nowStamp.AddMinutes(_refreshInterval);
                    for (int i=0; i< _refreshInterval - 1;i++) await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    if ((newTime - DateTimeOffset.Now).TotalSeconds > 30) await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private void LogExceptions(IReadOnlyCollection<Exception> innerExceptions)
        {
            if (null != innerExceptions)
            {
                foreach (Exception e in innerExceptions)
                {
                    _logger.LogError($"{e.Message}");
                    _logger.LogError($"{e.StackTrace}");
                    if (e is AggregateException) LogExceptions((e as AggregateException)?.InnerExceptions);
                }
            }
        }
    }
}