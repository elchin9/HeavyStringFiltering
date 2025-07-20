namespace HeavyStringFiltering.Application.Interfaces
{
    public interface ITextStorageCache
    {
        void Save(Guid id, string result);
    }
}
