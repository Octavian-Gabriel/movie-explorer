using MovieExplorer.Data;
using MovieExplorer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace MovieExplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add HttpClient for TMDb API
            builder.Services.AddHttpClient<IMovieService, MovieService>(
                client=>
                {
                    client.BaseAddress = new Uri(builder.Configuration["TMDb:BaseUrl"]);
                }
                );

            builder.Services.AddDbContext<MovieExplorerDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("MovieExplorerDbContext")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MovieExplorerDbContext>()
    .AddDefaultTokenProviders();


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using(var scope =app.Services.CreateScope())
            {
                var dbContext=scope.ServiceProvider.GetRequiredService<MovieExplorerDbContext>();
                dbContext.Database.EnsureCreated();
            }
            app.Run();
        }
    }
}
