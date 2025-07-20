using HeavyStringFiltering.Application.Interfaces;
using HeavyStringFiltering.Domain.FilteringStrategies;
using HeavyStringFiltering.Domain.FilterWords;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace HeavyStringFiltering.Application.Services
{
    public class TextFilteringService : ITextFilteringService
    {
        private readonly ISimilarityStrategy _strategy;
        private readonly List<string> _filterWords;
        private readonly float _threshold;

        public TextFilteringService(ISimilarityStrategy strategy, IOptions<FilterConfig> options)
        {
            _filterWords = options.Value.Words.Select(w => w.ToLowerInvariant()).Distinct().ToList();
            _threshold = options.Value.Threshold;
            _strategy = strategy;
        }

        public string NormalFilter(string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var normalizedWords = _filterWords.Select(f => f.ToLower()).ToArray();
            words = words.Select(w => w.ToLower()).ToArray();
            var resultWords = new List<string>();

            foreach (var word in words)
            {
                bool isSimilar = normalizedWords.Any(filter =>
                    _strategy.CalculateSimilarity(word, filter) >= _threshold);

                if (!isSimilar)
                {
                    resultWords.Add(word);
                }
            }

            return string.Join(" ", resultWords);
        }

        public string ParallelFilter(string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var results = new ConcurrentBag<(int Index, string Word)>();

            Parallel.For(0, words.Length, i =>
            {
                var word = words[i];

                bool isSimilar = _filterWords.Any(filter =>
                    _strategy.CalculateSimilarity(word.ToLower(), filter.ToLower()) >= _threshold);

                if (!isSimilar)
                {
                    results.Add((i, word));
                }
            });


            var sb = new StringBuilder();

            foreach (var word in results.OrderBy(t => t.Index))
            {
                sb.Append(word.Word);
                sb.Append(' ');
            }

            return sb.ToString().TrimEnd();
        }
    }
}
