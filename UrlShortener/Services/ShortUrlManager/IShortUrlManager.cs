using UrlShortener.Models;

namespace UrlShortener.Services.ShortUrlManager
{
    public interface IShortUrlManager
    {
        IEnumerable<UrlMappingEntry> GetAllUrlMappingEntries();
        Task DeleteUrlMappingEntryAsync(string id);
        Task<UrlMappingEntry> AddUrlMappingEntryAsync(string originalUrl);
        Task<UrlMappingEntry> GetAndCountUrlMappingEntryAsync(string id);
        UrlMappingEntry GetUrlMappingEntryAsync(string id);
    }
}
