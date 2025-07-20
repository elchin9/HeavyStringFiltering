namespace HeavyStringFiltering.Domain.FilteringStrategies
{
    public interface ISimilarityStrategy
    {
        double CalculateSimilarity(string word1, string word2);
    }
}
