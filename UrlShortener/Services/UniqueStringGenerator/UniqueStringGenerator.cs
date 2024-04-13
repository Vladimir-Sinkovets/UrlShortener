using Microsoft.Extensions.Options;

namespace UrlShortener.Services.UniqueStringGenerator
{
    public class UniqueStringGenerator : IUniqueStringGenerator
    {
        private readonly string _allowedChars;

        private readonly int _length;

        public UniqueStringGenerator(IOptions<UniqueStringGeneratorSettings> options)
        {
            _allowedChars = options.Value.AllowedChars;
            _length = options.Value.Length;
        }

        public string Generate()
        {
            return Guid.NewGuid()
                .ToByteArray()
                .Take(_length)
                .Select(b => _allowedChars[b % _allowedChars.Length].ToString())
                .Aggregate((x, y) => x + y);
        }
    }
}
