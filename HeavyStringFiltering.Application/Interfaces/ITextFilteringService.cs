namespace HeavyStringFiltering.Application.Interfaces
{
    public interface ITextFilteringService
    {
        string NormalFilter(string text);
        string ParallelFilter(string text);
    }
}
