namespace TestDebug
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITryCrash _crashGenerator;

        public Worker(ILogger<Worker> logger, ITryCrash crashGenerator)
        {
            _logger = logger;
            _crashGenerator = crashGenerator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {0}", DateTimeOffset.Now);
                }

                _crashGenerator.TryCrash(new Exception("Test Crash"), 10);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
