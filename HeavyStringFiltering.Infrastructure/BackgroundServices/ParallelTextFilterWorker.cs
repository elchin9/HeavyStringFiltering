using HeavyStringFiltering.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeavyStringFiltering.Infrastructure.BackgroundServices
{
    public class ParallelTextFilterWorker : BackgroundService
    {
        private readonly ITextQueueService _queueService;
        private readonly ITextFilteringService _filteringService;
        private readonly ILogger<ParallelTextFilterWorker> _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly List<Task> _runningTasks = new();
        private readonly ITextStorageCache _cache;

        public ParallelTextFilterWorker(
            ITextQueueService queueService,
            ITextFilteringService filteringService,
            ILogger<ParallelTextFilterWorker> logger,
            ITextStorageCache cache)
        {
            _queueService = queueService;
            _filteringService = filteringService;
            _logger = logger;

            var maxParallelism = Environment.ProcessorCount;
            _semaphore = new SemaphoreSlim(maxParallelism);
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queueService.TryDequeue(out var text))
                {
                    await _semaphore.WaitAsync(stoppingToken);

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            var filtered = _filteringService.ParallelFilter(text);
                            var id = Guid.NewGuid();
                            _cache.Save(id, filtered);
                            Console.WriteLine($"Filtered : {filtered}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error filtering text.");
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, stoppingToken);

                    lock (_runningTasks)
                    {
                        _runningTasks.Add(task);
                        _runningTasks.RemoveAll(t => t.IsCompleted);
                    }
                }
                else
                {
                    await Task.Delay(100, stoppingToken);
                }
            }

            await Task.WhenAll(_runningTasks);
        }
    }
}
