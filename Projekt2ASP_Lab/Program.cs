using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projekt2ASP_Lab.Models.Movies;
using Projekt2ASP_Lab.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Projekt2ASP_Lab;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();
        builder.Services.AddControllersWithViews();

    
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            try
            {
                op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
            }
            catch (SqliteException)
            {
                op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionStringUpperCase"]);
            }
        });


        builder.Services.AddDbContext<AppDbContext>(op =>
        {
            try
            {
                op.UseSqlite(builder.Configuration["AccountDatabase:ConnectionString"]);
            }
            catch (SqliteException)
            {
                op.UseSqlite(builder.Configuration["AccountDatabase:ConnectionStringUpperCase"]);
            }
        });
        
    
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
    
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddDebug();
            logging.AddConsole();
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}