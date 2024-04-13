namespace UrlShortener.ViewModels
{
    public class ShortenViewModel
    {
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public bool IsCorrect { get; set; } = true;
    }
}
