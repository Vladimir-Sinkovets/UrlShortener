using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.DataBase
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        public DbSet<UrlMappingEntry> UrlMappingEntries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        void IDbContext.SaveChanges()
        {
            base.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
    }
}
