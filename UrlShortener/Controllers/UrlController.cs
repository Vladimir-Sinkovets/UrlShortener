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

                viewModel.ShortUrl = $"https://{Host}/{entry.Slug}";

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
        [Route("/{slug}")]
        public async Task<IActionResult> UseShortUrlAsync(string slug)
        {
            try
            {
                var entry = await _shortUrlManager.GetAndCountUrlMappingEntryAsync(slug);

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

        [HttpGet]
        public IActionResult Edit(string slug)
        {
            try
            {
                var entry = _shortUrlManager.GetUrlMappingEntry(slug);

                var viewModel = new EditViewModel()
                {
                    Id = entry.Id,
                    Slug = entry.Slug,
                    Url = entry.Url,
                };

                return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel viewModel)
        {
            try
            {
                await _shortUrlManager.UpdateUrlMappingEntryAsync(viewModel.Id, viewModel.Slug, viewModel.Url);

                return RedirectToAction("Edit", new { slug = viewModel.Slug } );
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException e)
            {
                if (e.ParamName == "url")
                    viewModel.Message = "Wrong url";

                if (e.ParamName == "slug")
                    viewModel.Message = "Wrong slug";

                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
