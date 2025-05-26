using MSGM.Core;
using MSGM.Entity;
using MSGM.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("UAT");

        // Ensure that the "Logs" folder exists
        var logsFolder = Path.Combine(builder.Environment.ContentRootPath, "Logs");
        if (!Directory.Exists(logsFolder))
        {
            Directory.CreateDirectory(logsFolder);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(logsFolder, "Log.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add Identity services
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Add DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // Register unitOfWork
        builder.Services.AddScoped<IunitOfWork, unitOfWork>();

        builder.Services.AddRazorPages();
        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use exact property names
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Ignore null values
        });

        var app = builder.Build();

        // Middleware for non-development environments
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await DbInitializer.SeedAsync(services);
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            //pattern: "{controller=User}/{action=Shop}/{id?}");
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.Run();

        // Close and flush Serilog logger
        Log.CloseAndFlush();
    }
}
