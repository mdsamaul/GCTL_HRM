using Serilog;
using GCTL.UI.Core.Extensions;
using System.ComponentModel;
using System.Globalization;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureContext(builder.Configuration); // Application connection
builder.Services.ConfigureServices();
builder.Services.ConfigureMapper();
builder.Services.AddControllersWithViews();
//
QuestPDF.Settings.License = LicenseType.Community;
var cultureInfo = CultureInfo.InvariantCulture;

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

//
builder.Services.ConfigureSession();

builder.Services.ReadConfiguration(builder.Configuration);

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Warning();
    lc.ReadFrom.Configuration(ctx.Configuration);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "preview",
    pattern: "Preview/{name}",
    defaults: new { controller = "Preview", action = "Viewer", name = "" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
