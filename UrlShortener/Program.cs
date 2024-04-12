using Microsoft.EntityFrameworkCore;
using UrlShortener.DataBase;

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

            builder.Services.AddControllersWithViews();

            var app = builder.Build();


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