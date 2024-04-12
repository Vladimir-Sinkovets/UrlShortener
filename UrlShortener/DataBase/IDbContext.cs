using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.DataBase
{
    public interface IDbContext
    {
        DbSet<UrlMappingEntry> UrlMappingEntries { get; }
    }
}
