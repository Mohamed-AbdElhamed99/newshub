using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Infrastructure;
using NewsHub.Infrastructure.Data;
using NewsHub.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Generate lowercase URLs (e.g. /admin/account/login instead of
// /Admin/Account/Login) for every asp-controller/asp-action/Url.Action link.
// Incoming request matching was already case-insensitive either way — this
// only affects how links get generated.
builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Registers DbContext, Identity, and all Admin/Site repositories + AuthService.
// (Removed the separate manual AddDbContext<NewsHubDbContext> call above —
// AddInfrastructureServices already does this; having both was a duplicate
// registration.)
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/admin/account/login";
    options.AccessDeniedPath = "/admin/account/login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// UseAuthentication() MUST come before UseAuthorization(), and both MUST
// come after UseRouting() and before MapControllerRoute. Missing
// UseAuthentication() means the auth cookie is never read into
// HttpContext.User, so [Authorize] always sees an anonymous user.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Area route MUST be registered before the default route, or "/Admin/..."
// URLs get swallowed by the default route first (Controller=Admin,
// Action=... which doesn't exist) instead of matching the area.
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.Services.SeedAdminUserAsync();

app.Run();