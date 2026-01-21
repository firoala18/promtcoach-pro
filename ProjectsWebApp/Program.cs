

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Classes;
using ProjectsWebApp.DataAccsess.Services.Calsses;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System;
using System.Globalization;
using System.Net;
using ProjectsWebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ─────────── Session ───────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ─────────── MVC, Razor, Localization ───────────
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();


// ─────────── Form-Limits ───────────
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500_000_000; // 500 MB
});

// ─────────── DI ───────────
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IContactEmailService, ContactEmailService>();
builder.Services.AddScoped<IPromptFilterAiService, PromptFilterAiService>();
builder.Services.AddScoped<IPromptSmartSelectionService, PromptSmartSelectionService>();
builder.Services.AddSingleton<ISmartSelectionProgressStore, InMemorySmartSelectionProgressStore>();
builder.Services.AddSingleton<ISmartSelectionProgressNotifier, SmartSelectionProgressNotifier>();
builder.Services.AddScoped<ISemanticIndexService, SemanticIndexService>();
builder.Services.AddScoped<IAiProviderResolver, AiProviderResolver>();
builder.Services.AddScoped<IUserActivityLogger, UserActivityLogger>();
builder.Services.AddScoped<IUserActivityAnalyticsService, UserActivityAnalyticsService>();
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddScoped<IPdfChunkingService, PdfChunkingService>();

// ─────────── EF Core (MariaDB via Pomelo) ───────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))

);


// ─────────── Identity with Roles ───────────
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5; // after 5 failed attempts the account is temporarily locked
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Login/Logout routes + secure cookies behind TLS proxy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ─────────── Policies ───────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));
});

// ─────────── Localization cultures ───────────
var supported = new[] { new CultureInfo("de-DE"), new CultureInfo("en-US") };
builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();   // <- important

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("de-DE"),
    SupportedCultures = supported,
    SupportedUICultures = supported
});

// ─────────── Middleware ───────────
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Respect reverse proxy (Apache) for scheme/host so redirects are correct
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    // Accept from local reverse proxy:
    KnownProxies = { IPAddress.Parse("127.0.0.1"), IPAddress.IPv6Loopback }
});



// Path base from environment (systemd sets ASPNETCORE_PATHBASE=/apps/promptcoach)
var pathBase = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
if (!string.IsNullOrEmpty(pathBase))
    app.UsePathBase(pathBase);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Basic per-IP rate limiting for login endpoint
app.UseMiddleware<ProjectsWebApp.Utility.LoginRateLimitingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=user}/{controller=Home}/{action=Home}/{id?}"
);

// SignalR hubs
app.MapHub<AdminImportHub>("/hubs/admin-import");

// ─────────── Seed roles & default accounts ───────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleM = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userM = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1) Rollen anlegen
    string[] roles = { "User", "Admin", "SuperAdmin", "Dozent", "ApiManager" };
    foreach (var r in roles)
    {
        if (!await roleM.RoleExistsAsync(r))
            await roleM.CreateAsync(new IdentityRole(r));
    }

    // 2) SuperAdmin(s) anlegen
    var superAdmins = new[]
    {
        (Email: "h.seehagen-marx@uni-wuppertal.de",
         Pwd:   "Medialab25!!",
         Name:  "Dr. Heike Seehagen-Marx")
        // weitere vordefinierte SuperAdmins …
    };

    foreach (var sa in superAdmins)
    {
        var u = await userM.FindByEmailAsync(sa.Email);
        if (u == null)
        {
            u = new IdentityUser
            {
                UserName = sa.Email,
                Email = sa.Email,
                EmailConfirmed = true
            };
            await userM.CreateAsync(u, sa.Pwd);
        }

        if (!await userM.IsInRoleAsync(u, "Admin"))
            await userM.AddToRoleAsync(u, "Admin");
        if (!await userM.IsInRoleAsync(u, "SuperAdmin"))
            await userM.AddToRoleAsync(u, "SuperAdmin");
    }
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Load persisted feature flags (best-effort)
    try
    {
        var rec = db.PromptFeatureSettings
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefault();
        if (rec != null)
        {
            ProjectsWebApp.Utility.FeatureFlags.EnableFilterGeneration = rec.EnableFilterGeneration;
            ProjectsWebApp.Utility.FeatureFlags.EnableSmartSelection   = rec.EnableSmartSelection;
            ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics        = rec.EnableAnalytics;
        }
    }
    catch { /* ignore if table not present yet */ }

    if (!db.RegistrationCodes.Any())
    {
        db.RegistrationCodes.AddRange(
            new RegistrationCode { Code = "ABC123" },
            new RegistrationCode { Code = "XYZ789" },
            new RegistrationCode { Code = "WELCOME2025" }
        );
        await db.SaveChangesAsync();
    }
}

app.Run();
