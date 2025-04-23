using MovieExplorer.Data;
using MovieExplorer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using MovieExplorer.Services.Interfaces;
namespace MovieExplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient<IMovieService, MovieService>(
                client=>
                {
                    client.BaseAddress = new Uri(builder.Configuration["TMDb:BaseUrl"]);
                }
                );
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDbContext<MovieExplorerDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("MovieExplorerDbContext")));

            builder.Services.AddScoped<IUserService, UserService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            using (var scope =app.Services.CreateScope())
            {
                var dbContext=scope.ServiceProvider.GetRequiredService<MovieExplorerDbContext>();
                dbContext.Database.EnsureCreated();
            }
            app.Run();
        }
    }
}
