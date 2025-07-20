namespace HeavyStringFiltering.Application.Interfaces
{
    public interface ITextQueueService
    {
        void Enqueue(string fullText);
        bool TryDequeue(out string text);
    }
}
