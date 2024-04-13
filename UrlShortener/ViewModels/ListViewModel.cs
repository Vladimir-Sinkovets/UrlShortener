using UrlShortener.Models;

namespace UrlShortener.ViewModels
{
    public class ListViewModel
    {
        public IEnumerable<UrlMappingEntry> UrlMappingEntries { get; set; } = null!;
        public string Host { get; set; } = string.Empty;
    }
}
