using Microsoft.AspNetCore.Mvc;
using UrlShortener.DataBase;
using UrlShortener.Models;
using UrlShortener.Services.UniqueStringGenerator;
using UrlShortener.ViewModels;

namespace UrlShortener.Controllers
{
    public class UrlController : Controller
    {
        private readonly IDbContext _dbContext;
        private readonly IUniqueStringGenerator _uniqueStringGenerator;

        public UrlController(IDbContext dbContext, IUniqueStringGenerator uniqueStringGenerator)
        {
            _dbContext = dbContext;
            _uniqueStringGenerator = uniqueStringGenerator;
        }

        private string Host { get => HttpContext.Request.Host.Value; }

        [HttpGet]
        public IActionResult List()
        {
            var urlMappingEntries = _dbContext.UrlMappingEntries.ToList();

            var viewModel = new ListViewModel()
            {
                UrlMappingEntries = urlMappingEntries,
                Host = Host,
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Shorten() => View();

        [HttpPost]
        public async Task<IActionResult> Shorten(ShortenViewModel viewModel)
        {
            if (Uri.IsWellFormedUriString(viewModel.Url, UriKind.Absolute) == false)
            {
                viewModel.IsCorrect = false;
                return View(viewModel);
            }

            var uniqueString = GenerateUniqueStringForCollection(_uniqueStringGenerator, _dbContext.UrlMappingEntries);

            var urlMappingEntry = new UrlMappingEntry()
            {
                ClicksCount = 0,
                Created = DateTime.Now,
                Url = viewModel.Url,
                Id = uniqueString,
            };

            _dbContext.UrlMappingEntries.Add(urlMappingEntry);

            await _dbContext.SaveChangesAsync();

            viewModel.ShortUrl = $"https://{Host}/{uniqueString}";

            return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> DeleteShortUrl(string id, string redirectUrl)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry != null)
            {
                _dbContext.UrlMappingEntries.Remove(entry);

                await _dbContext.SaveChangesAsync();
            }

            return Redirect(redirectUrl);
        }
        
        [HttpGet]
        [Route("/{id}")]
        public async Task<IActionResult> UseShortUrlAsync(string id)
        {
            var entry = _dbContext.UrlMappingEntries.FirstOrDefault(x => x.Id == id);

            if (entry == null)
            {
                return NotFound();
            }

            entry.ClicksCount++;

            await _dbContext.SaveChangesAsync();

            return Redirect(entry.Url);
        }
    }
}
