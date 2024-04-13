namespace UrlShortener.ViewModels
{
    public class ShortenViewModel
    {
        public string Url { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = true;
    }
}
