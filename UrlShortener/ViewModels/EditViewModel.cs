namespace UrlShortener.ViewModels
{
    public class EditViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
