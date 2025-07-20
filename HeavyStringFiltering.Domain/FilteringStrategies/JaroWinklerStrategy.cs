namespace HeavyStringFiltering.Domain.FilteringStrategies
{
    public class JaroWinklerStrategy : ISimilarityStrategy
    {
        private const double DefaultScalingFactor = 0.1;

        public double CalculateSimilarity(string s1, string s2)
        {
            if (s1 == null || s2 == null)
                return 0.0;

            if (s1 == s2)
                return 1.0;

            int matchDistance = Math.Max(s1.Length, s2.Length) / 2 - 1;

            var s1Matches = new bool[s1.Length];
            var s2Matches = new bool[s2.Length];

            int matches = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance + 1, s2.Length);

                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j]) continue;
                    if (s1[i] != s2[j]) continue;

                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0) return 0.0;

            double t = 0;
            int k = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (!s1Matches[i]) continue;
                while (!s2Matches[k]) k++;
                if (s1[i] != s2[k]) t++;
                k++;
            }

            t /= 2.0;

            double jaro = ((matches / (double)s1.Length) +
                           (matches / (double)s2.Length) +
                           ((matches - t) / matches)) / 3.0;

            int prefix = 0;
            for (int i = 0; i < Math.Min(4, Math.Min(s1.Length, s2.Length)); i++)
            {
                if (s1[i] == s2[i]) prefix++;
                else break;
            }

            return jaro + prefix * DefaultScalingFactor * (1 - jaro);
        }
    }
}
