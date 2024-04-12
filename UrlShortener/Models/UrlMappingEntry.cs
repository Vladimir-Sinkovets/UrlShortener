namespace UrlShortener.Models
{
    public class UrlMappingEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public int ClicksCount {  get; set; }
    }
}
