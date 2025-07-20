using HeavyStringFiltering.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace HeavyStringFiltering.Infrastructure.BackgroundServices
{
    public class TextFilterWorker : BackgroundService
    {
        private readonly ITextQueueService _queueService;
        private readonly ITextFilteringService _filteringService;
        private readonly ITextStorageCache _cache;
        public TextFilterWorker(ITextQueueService queueService, ITextFilteringService filteringService, ITextStorageCache cache)
        {
            _queueService = queueService;
            _filteringService = filteringService;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queueService.TryDequeue(out var text) && !string.IsNullOrWhiteSpace(text))
                {
                    var id = Guid.NewGuid();
                    var filtered = _filteringService.ParallelFilter(text);
                    _cache.Save(id, filtered);
                    Console.WriteLine($"Filtered : {filtered}");
                }

                await Task.Delay(10, stoppingToken);
            }
        }
    }
}
