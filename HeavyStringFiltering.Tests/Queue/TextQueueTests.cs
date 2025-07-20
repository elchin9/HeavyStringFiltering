using FluentAssertions;
using HeavyStringFiltering.Application.Services;

namespace HeavyStringFiltering.Tests.Queue
{
    public class TextQueueTests
    {
        private readonly TextQueueService _queue;

        public TextQueueTests()
        {
            _queue = new TextQueueService();
        }

        [Fact]
        public void Enqueue_And_Dequeue_Should_Work()
        {
            string text = "hello";

            _queue.Enqueue(text);
            var result = _queue.TryDequeue(out var dequeued);

            result.Should().BeTrue();
            dequeued.Should().Be(text);
        }

        [Fact]
        public void TryDequeue_Should_ReturnFalse_When_Empty()
        {
            var result = _queue.TryDequeue(out var dequeued);

            result.Should().BeFalse();
            dequeued.Should().BeNull();
        }
    }
}
