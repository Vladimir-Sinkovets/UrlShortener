namespace UrlShortener.ViewModels
{
    public class EditViewModel
    {
        public string Url { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string NewId { get; set; } = string.Empty;
        public string Message { get; internal set; }
    }
}
