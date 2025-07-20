using HeavyStringFiltering.Application.Interfaces;
using HeavyStringFiltering.Infrastructure.BackgroundServices;
using Moq;

namespace HeavyStringFiltering.Tests.Infrastructure
{
    public class TextFilterWorkerTests
    {
        [Fact]
        public async Task Should_Process_Text_When_Queue_Has_Item()
        {
            var mockQueue = new Mock<ITextQueueService>();
            var mockFilter = new Mock<ITextFilteringService>();
            var mockCache = new Mock<ITextStorageCache>();

            string dummy = "mock text";

            mockQueue.Setup(q => q.TryDequeue(out dummy)).Returns(true);
            mockFilter.Setup(f => f.ParallelFilter(It.IsAny<string>())).Returns("filtered text");

            var worker = new TextFilterWorker(mockQueue.Object, mockFilter.Object, mockCache.Object);

            var cts = new CancellationTokenSource();
            var task = worker.StartAsync(cts.Token);

            await Task.Delay(200);
            cts.Cancel();
            await task;

            mockFilter.Verify(f => f.ParallelFilter("mock text"), Times.AtLeastOnce);
        }

        private delegate void TryDequeueDelegate(out string text);

        [Fact]
        public async Task Should_Do_Nothing_When_Queue_Is_Empty()
        {
            var mockQueue = new Mock<ITextQueueService>();
            var mockFilter = new Mock<ITextFilteringService>();
            var mockCache = new Mock<ITextStorageCache>();

            string dummy;
            mockQueue.Setup(q => q.TryDequeue(out dummy)).Returns(false);

            var worker = new TextFilterWorker(mockQueue.Object, mockFilter.Object, mockCache.Object);

            var cts = new CancellationTokenSource();
            var runTask = worker.StartAsync(cts.Token);

            await Task.Delay(100);
            cts.Cancel();
            await runTask;

            mockFilter.Verify(f => f.ParallelFilter(It.IsAny<string>()), Times.Never);
        }

        private delegate void TryDequeueCallback(out string text);
    }
}
