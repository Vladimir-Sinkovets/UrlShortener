using UrlShortener.DataBase;
using UrlShortener.Models;
using UrlShortener.Services.Exceptions;
using UrlShortener.Services.UniqueStringGenerator;

namespace UrlShortener.Services.ShortUrlManager
{
    public class ShortUrlManager : IShortUrlManager
    {
        private readonly IDbContext _dbContext;
        private readonly IUniqueStringGenerator _uniqueStringGenerator;

        public ShortUrlManager(IDbContext dbContext, IUniqueStringGenerator uniqueStringGenerator)
        {
            _dbContext = dbContext;
            _uniqueStringGenerator = uniqueStringGenerator;
        }

        public async Task<UrlMappingEntry> AddUrlMappingEntryAsync(string originalUrl)
        {
            if (IsUrlValid(originalUrl) == false)
            {
                throw new ArgumentException("Wrong url format");
            }

            var uniqueString = GenerateUniqueStringForCollection(_uniqueStringGenerator, _dbContext.UrlMappingEntries);

            var urlMappingEntry = new UrlMappingEntry()
            {
                ClicksCount = 0,
                Created = DateTime.Now,
                Url = originalUrl,
                Id = uniqueString,
            };

            _dbContext.UrlMappingEntries.Add(urlMappingEntry);

            await _dbContext.SaveChangesAsync();

            return urlMappingEntry;
        }

        public async Task DeleteUrlMappingEntryAsync(string id)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry != null)
            {
                _dbContext.UrlMappingEntries.Remove(entry);

                await _dbContext.SaveChangesAsync();
            }
        }

        public IEnumerable<UrlMappingEntry> GetAllUrlMappingEntries()
        {
            return _dbContext.UrlMappingEntries;
        }

        public async Task<UrlMappingEntry> GetAndCountUrlMappingEntryAsync(string id)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry == null)
                throw new NotFoundException($"Entry with id = {id} does not exist");

            entry.ClicksCount++;

            await _dbContext.SaveChangesAsync();

            return entry;
        }

        public UrlMappingEntry GetUrlMappingEntry(string id)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry == null)
                throw new NotFoundException($"Entry with id = {id} does not exist");

            return entry;
        }

        public async Task UpdateUrlMappingEntryAsync(string id, string newId, string url)
        {
            var entry = GetUrlMappingEntry(id);

            if (id != newId)
            {
                var isIdUnique = !_dbContext.UrlMappingEntries.Any(x => x.Id == newId);

                if (isIdUnique == false)
                    throw new ArgumentException($"Entry with id = {newId} has already been used", nameof(newId));
            }

            if (IsUrlValid(url) == false)
                throw new ArgumentException($"Url {url} is not valid", nameof(url));

            entry.Url = url;
            entry.Id = newId;

            await _dbContext.SaveChangesAsync();
        }

        private static bool IsUrlValid(string originalUrl)
        {
            return Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute);
        }

        private static string GenerateUniqueStringForCollection(IUniqueStringGenerator uniqueStringGenerator,
            IQueryable<UrlMappingEntry> urlMappingEntries)
        {
            var uniqueString = string.Empty;

            UrlMappingEntry? entry = null;

            do
            {
                uniqueString = uniqueStringGenerator.Generate();
                entry = urlMappingEntries.FirstOrDefault(x => x.Id == uniqueString);
            }
            while (entry != null);

            return uniqueString;
        }
    }
}
