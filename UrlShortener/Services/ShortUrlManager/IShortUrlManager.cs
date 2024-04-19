using UrlShortener.Models;

namespace UrlShortener.Services.ShortUrlManager
{
    public interface IShortUrlManager
    {
        IEnumerable<UrlMappingEntry> GetAllUrlMappingEntries();
        Task DeleteUrlMappingEntryAsync(string id);
        Task<UrlMappingEntry> AddUrlMappingEntryAsync(string originalUrl);
        Task<UrlMappingEntry> GetAndCountUrlMappingEntryAsync(string slug);
        UrlMappingEntry GetUrlMappingEntry(string slug);
        Task UpdateUrlMappingEntryAsync(string id, string slug, string url);
    }
}
