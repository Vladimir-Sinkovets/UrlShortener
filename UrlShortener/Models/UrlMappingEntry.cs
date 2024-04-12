namespace UrlShortener.Models
{
    public class UrlMappingEntry
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public int Count {  get; set; }
    }
}
