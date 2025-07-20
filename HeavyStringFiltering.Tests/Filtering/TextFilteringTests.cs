using FluentAssertions;
using HeavyStringFiltering.Application.Services;
using HeavyStringFiltering.Domain.FilteringStrategies;
using HeavyStringFiltering.Domain.FilterWords;
using Microsoft.Extensions.Options;

namespace HeavyStringFiltering.Tests.Filtering
{
    public class TextFilteringTests
    {
        private readonly TextFilteringService _filteringService;

        public TextFilteringTests()
        {
            var strategy = new LevenshteinStrategy();

            var config = new FilterConfig
            {
                Words = new List<string> { "bomb", "attack", "kill", "ugly" },
                Threshold = 0.8f
            };

            var optionsMock = Options.Create(config);

            _filteringService = new TextFilteringService(strategy, optionsMock);
        }

        [Fact]
        public void Should_Remove_Similar_FilterWords()
        {
            string input = "This is a attak on the city";
            var result = _filteringService.ParallelFilter(input);

            result.Should().NotContain("attak");
        }

        [Fact]
        public void Should_Keep_Non_Filtered_Words()
        {
            string input = "This is a normal sentence";
            var result = _filteringService.ParallelFilter(input);

            result.Should().Be("This is a normal sentence");
        }
    }
}
