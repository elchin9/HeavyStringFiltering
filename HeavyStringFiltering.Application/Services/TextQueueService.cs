using HeavyStringFiltering.Application.Interfaces;
using System.Collections.Concurrent;

namespace HeavyStringFiltering.Application.Services
{
    public class TextQueueService : ITextQueueService
    {
        private readonly ConcurrentQueue<string> _queue = new();

        public void Enqueue(string fullText)
        {
            _queue.Enqueue(fullText);
        }

        public bool TryDequeue(out string text)
        {
            return _queue.TryDequeue(out text);
        }
    }
}
