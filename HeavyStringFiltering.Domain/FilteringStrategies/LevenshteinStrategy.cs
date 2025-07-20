namespace HeavyStringFiltering.Domain.FilteringStrategies
{
    public class LevenshteinStrategy : ISimilarityStrategy
    {
        public double CalculateSimilarity(string source, string target)
        {
            if (source == target)
                return 1.0;

            int distance = LevenshteinDistance(source, target);
            int maxLength = Math.Max(source.Length, target.Length);
            return 1.0 - (double)distance / maxLength;
        }

        private int LevenshteinDistance(string s, string t)
        {
            var d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = new[] {
                    d[i - 1, j] + 1,
                    d[i, j - 1] + 1,
                    d[i - 1, j - 1] + cost
                }.Min();
                }
            }

            return d[s.Length, t.Length];
        }
    }
}
