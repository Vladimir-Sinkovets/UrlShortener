using Microsoft.EntityFrameworkCore;
using System;
using UrlShortener.DataBase;
using UrlShortener.Services.ShortUrlManager;
using UrlShortener.Services.UniqueStringGenerator;

namespace UrlShortener
{
    public class Program
    {
        private const string MySqlConnectionSection = "MySql";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connection = builder.Configuration.GetConnectionString(MySqlConnectionSection);
            
            ServerVersion version = ServerVersion.AutoDetect(connection);

            builder.Services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
            {
                options.UseMySql(connection, version);
            });

            builder.Services.AddTransient<IShortUrlManager, ShortUrlManager>();
            builder.Services.AddTransient<IUniqueStringGenerator, UniqueStringGenerator>();
            builder.Services.Configure<UniqueStringGeneratorSettings>(builder.Configuration.GetSection("UniqueStringGenerator"));
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database
                .Migrate();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Url}/{action=List}/{id?}");

            app.Run();
        }
    }
}