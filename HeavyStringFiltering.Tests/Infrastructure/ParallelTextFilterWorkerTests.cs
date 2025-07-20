using HeavyStringFiltering.Application.Interfaces;
using HeavyStringFiltering.Infrastructure.BackgroundServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace HeavyStringFiltering.Tests.Infrastructure
{
    public class ParallelTextFilterWorkerTests
    {
        [Fact]
        public async Task Should_Process_Text_From_Queue_And_Save_To_Cache()
        {
            var mockQueue = new Mock<ITextQueueService>();
            var mockFilter = new Mock<ITextFilteringService>();
            var mockLogger = new Mock<ILogger<ParallelTextFilterWorker>>();
            var mockCache = new Mock<ITextStorageCache>();

            string testText = "This is a test";
            string filteredText = "Filtered text";

            mockQueue.Setup(q => q.TryDequeue(out testText)).Returns(true);
            mockFilter.Setup(f => f.ParallelFilter(testText)).Returns(filteredText);

            var worker = new ParallelTextFilterWorker(
                mockQueue.Object,
                mockFilter.Object,
                mockLogger.Object,
                mockCache.Object
            );

            var cts = new CancellationTokenSource();
            var runTask = worker.StartAsync(cts.Token);

            await Task.Delay(200);

            cts.Cancel();
            await runTask;

            mockFilter.Verify(f => f.ParallelFilter(testText), Times.AtLeastOnce);
            mockCache.Verify(c => c.Save(It.IsAny<Guid>(), filteredText), Times.AtLeastOnce);
        }
    }
}
