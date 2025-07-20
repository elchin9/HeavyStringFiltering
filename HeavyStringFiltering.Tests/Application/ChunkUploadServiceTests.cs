using HeavyStringFiltering.Application.CQRS.Commands;
using HeavyStringFiltering.Application.Interfaces;
using Moq;

namespace HeavyStringFiltering.Tests.Application
{
    public class ChunkUploadServiceTests
    {
        [Fact]
        public async Task Should_Enqueue_Combined_Chunks_When_LastChunk()
        {
            var mockQueue = new Mock<ITextQueueService>();

            var handler = new CreateChunkCommandHandler(mockQueue.Object);

            var command1 = new CreateChunkCommand
            {
                UploadId = "upload-chunk-1",
                ChunkIndex = 0,
                Data = "Hello",
                IsLastChunk = false
            };

            var command2 = new CreateChunkCommand
            {
                UploadId = "upload-chunk-1",
                ChunkIndex = 1,
                Data = "Pasha Insurance!",
                IsLastChunk = true
            };

            await handler.Handle(command1, default);
            await handler.Handle(command2, default);

            mockQueue.Verify(q => q.Enqueue("Hello Pasha Insurance!"), Times.Once);
        }

        [Fact]
        public async Task Should_Not_Enqueue_If_Not_LastChunk()
        {
            var mockQueue = new Mock<ITextQueueService>();
            var handler = new CreateChunkCommandHandler(mockQueue.Object);

            var command = new CreateChunkCommand
            {
                UploadId = "upload-chunk-2",
                ChunkIndex = 0,
                Data = "Only one piece",
                IsLastChunk = false
            };

            await handler.Handle(command, default);

            mockQueue.Verify(q => q.Enqueue(It.IsAny<string>()), Times.Never);
        }
    }
}
