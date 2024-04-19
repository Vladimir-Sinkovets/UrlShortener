using Microsoft.Extensions.Options;
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
        private readonly string _allowedChars;

        public ShortUrlManager(IDbContext dbContext, IUniqueStringGenerator uniqueStringGenerator,
            IOptions<ShortUrlManagerSettings> options)
        {
            _dbContext = dbContext;
            _uniqueStringGenerator = uniqueStringGenerator;
            
            _allowedChars = options.Value.AllowedChars;
        }

        public async Task<UrlMappingEntry> AddUrlMappingEntryAsync(string originalUrl)
        {
            if (IsUrlValid(originalUrl) == false)
            {
                throw new ArgumentException("Wrong url format", nameof(originalUrl));
            }

            var slug = GenerateUniqueStringForCollection(_uniqueStringGenerator, _dbContext.UrlMappingEntries);

            var urlMappingEntry = new UrlMappingEntry()
            {
                Id = Guid.NewGuid().ToString(),
                ClicksCount = 0,
                Created = DateTime.Now,
                Url = originalUrl,
                Slug = slug,
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

        public async Task<UrlMappingEntry> GetAndCountUrlMappingEntryAsync(string slug)
        {
            var entry = GetUrlMappingEntry(slug);

            entry.ClicksCount++;

            await _dbContext.SaveChangesAsync();

            return entry;
        }

        public UrlMappingEntry GetUrlMappingEntry(string slug)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Slug == slug);

            if (entry == null)
                throw new NotFoundException($"Entry with slug = {slug} does not exist");

            return entry;
        }

        public async Task UpdateUrlMappingEntryAsync(string id, string slug, string url)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry == null)
                throw new NotFoundException($"Entry with id = {id} does not exist");

            if (entry.Slug != slug)
                ThrowSlugExceptions(slug);

            if (IsUrlValid(url) == false)
                throw new ArgumentException($"Url {url} is not valid", nameof(url));

            entry.Url = url;
            entry.Slug = slug;

            await _dbContext.SaveChangesAsync();
        }

        private void ThrowSlugExceptions(string slug)
        {
            var isIdUnique = !_dbContext.UrlMappingEntries.Any(x => x.Slug == slug);

            if (isIdUnique == false)
                throw new ArgumentException($"Entry with slug = {slug} has already been used", nameof(slug));

            var isSlugValid = slug.Select(x => _allowedChars.Contains(x))
                .All(x => x == true);

            if (isSlugValid == false)
                throw new ArgumentException($"Slug is not valid", nameof(slug));
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
