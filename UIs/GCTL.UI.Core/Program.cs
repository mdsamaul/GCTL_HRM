using GCTL.UI.Core.BackgroundServices;
using GCTL.UI.Core.Extensions;
using GCTL.UI.Core.Hubs;
using QuestPDF.Infrastructure;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Services Registration
// =====================

// DB + Services + AutoMapper
builder.Services.ConfigureContext(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureMapper();

// MVC
builder.Services.AddControllersWithViews();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddHostedService<AttendanceSqlWatcher>();

// Session
builder.Services.ConfigureSession();

// App Config
builder.Services.ReadConfiguration(builder.Configuration);

// QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// Culture Setting (Invariant)
var cultureInfo = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Serilog Bootstrap
Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Warning();
    lc.ReadFrom.Configuration(ctx.Configuration);
});

var app = builder.Build();

// =====================
// Middleware Pipeline
// =====================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// =====================
// SignalR Hub Mapping
// =====================
app.MapHub<AttendanceHub>("/attendanceHub");

// =====================
// Custom Routes
// =====================

app.MapControllerRoute(
    name: "preview",
    pattern: "Preview/{name}",
    defaults: new { controller = "Preview", action = "Viewer", name = "" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();