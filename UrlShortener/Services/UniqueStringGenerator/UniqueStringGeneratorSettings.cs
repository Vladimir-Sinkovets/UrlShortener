namespace UrlShortener.Services.UniqueStringGenerator
{
    public class UniqueStringGeneratorSettings
    {
        public int Length { get; set; }
        public string AllowedChars { get; set; } = string.Empty;
    }
}
