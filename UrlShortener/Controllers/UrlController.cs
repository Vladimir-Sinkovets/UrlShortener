using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services.Exceptions;
using UrlShortener.Services.ShortUrlManager;
using UrlShortener.ViewModels;

namespace UrlShortener.Controllers
{
    public class UrlController : Controller
    {
        private readonly IShortUrlManager _shortUrlManager;

        public UrlController(IShortUrlManager shortUrlManager)
        {
            _shortUrlManager = shortUrlManager;
        }

        private string Host { get => HttpContext.Request.Host.Value; }

        [HttpGet]
        public IActionResult List()
        {
            var urlMappingEntries = _shortUrlManager.GetAllUrlMappingEntries();

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
            try
            {
                var entry = await _shortUrlManager.AddUrlMappingEntryAsync(viewModel.Url);

                viewModel.ShortUrl = $"https://{Host}/{entry.Id}";

                return View(viewModel);
            }
            catch (ArgumentException)
            {
                viewModel.IsCorrect = false;

                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShortUrl(string id, string redirectUrl)
        {
            await _shortUrlManager.DeleteUrlMappingEntryAsync(id);

            return Redirect(redirectUrl);
        }
        
        [HttpGet]
        [Route("/{id}")]
        public async Task<IActionResult> UseShortUrlAsync(string id)
        {
            try
            {
                var entry = await _shortUrlManager.GetAndCountUrlMappingEntryAsync(id);

                return Redirect(entry.Url);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
