using Microsoft.AspNetCore.Mvc;
using UrlShortener.DataBase;
using UrlShortener.ViewModels;

namespace UrlShortener.Controllers
{
    public class UrlController : Controller
    {
        private IDbContext _dbContext;

        public UrlController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult List()
        {
            string host = HttpContext.Request.Host.Value;

            var urlMappingEntries = _dbContext.UrlMappingEntries
                .ToList();

            var viewModel = new ListViewModel()
            {
                UrlMappingEntries = urlMappingEntries,
                Host = host
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateUrl()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUrl(string url)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteShortUrl(string redirectUrl)
        {
            return Redirect(redirectUrl);
        }
        
        [HttpGet]
        [Route("/{id}")]
        public IActionResult UseShortUrl(string id)
        {
            return Content("Ok");
        }
    }
}
