using System.Security.Claims;
using Bonjour;
using Bonjour.Lib.Services;
using Bonjour.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration.AddEnvironmentVariables().Build();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR();
builder.Services.AddAuthentication("cookie")
        .AddCookie("cookie", options =>
        {
            options.Cookie.Name = "MyAuthCookie";
            options.LoginPath = "/Account/Login"; // Specify your login page URL
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set cookie expiration
            options.SlidingExpiration = true; // Re-issue cookie if more than halfway expired
        });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Name, "admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("Loading", policy => policy.RequireRole("Loading"));
    options.AddPolicy("Unloading", policy => policy.RequireRole("Unloading"));
});
builder.Services.AddTransient<Bonjour.Domain.Users.PasswordHasher>();
var app = builder.Build();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "storage", "qrcode")), // Or use a direct path like "C:\\SharedImages"
    RequestPath = "/QrCode" // The URL path to access these files
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<RealTimeHub>("/realtimehub");
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};
app.Run();
