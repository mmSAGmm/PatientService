
namespace PatientService.HostedServices
{
    public class CrawlerService : IHostedService, IDisposable
    {
        private readonly ILogger<CrawlerService> _logger;

        public CrawlerService(ILogger<CrawlerService> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _logger.LogInformation($"Disposing {nameof(CrawlerService)}");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"StartAsync {nameof(CrawlerService)}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"StopAsync {nameof(CrawlerService)}");
            return Task.CompletedTask;
        }
    }
}
