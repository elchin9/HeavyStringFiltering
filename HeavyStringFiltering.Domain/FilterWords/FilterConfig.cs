namespace HeavyStringFiltering.Domain.FilterWords
{
    public class FilterConfig
    {
        public List<string> Words { get; set; } = new();
        public float Threshold { get; set; }
    }
}
