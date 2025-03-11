using WebTorrent.Api.Extensions;
using WebTorrent.Api.Models;
using WebTorrent.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureAutoMapper();
builder.Services.ConfigureHelpers();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureCors();
builder.Services.ConfigureMongoDb(builder.Configuration);
builder.Services.SetupCookieAuthentication(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/var/data-protection-keys"))
    .SetApplicationName("WebTorrent"); // Ensures all instances use the same key
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN"; // The cookie name
    options.HeaderName = "X-XSRF-TOKEN"; // The expected header name
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAnyOrigin");
}
else
{
    app.UseCors("AllowScotexTech");
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

var downloadDir = "/tmp/torrent_downloads";
var torrentDir = "/tmp/torrents";
var downloadProgress = new ConcurrentDictionary<string, int>();
Directory.CreateDirectory(downloadDir);
Directory.CreateDirectory(torrentDir);

var apiGroup = app.MapGroup("/api");

app.Use(async (context, next) =>
{
    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
        new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });

    await next();
});

apiGroup.MapGet("/google/login", (IGoogleOAuthRepository googleOAuthRepository) =>
{
    return Results.Redirect(googleOAuthRepository.GetGoogleOAuthUrl());
});

apiGroup.MapPost("/google/callback", async (OAuthRequest request, IGoogleOAuthRepository googleOAuthRepository, HttpContext httpContext) =>
{
    var code = request.Code;
    if (string.IsNullOrEmpty(code))
    {
        return Results.BadRequest("No OAuth code provided");
    }

    var result = await googleOAuthRepository.GetGoogleUser(code);

    var claims = new List<Claim>
    {
        new("user_id", result.Data.Id.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
    {
        IsPersistent = true,
        AllowRefresh = true
    });

    return Results.Ok(new { message = "Signed in successfully" });
});

apiGroup.MapPost("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync();
    return Results.Ok("Signed out successfully.");
});

apiGroup.MapPost("/upload", async (HttpContext context, IFormFile file, CancellationToken cancellationToken) =>
{
    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
    await antiforgery.ValidateRequestAsync(context);
    
    if (file == null || file.Length == 0)
        return Results.BadRequest("Invalid file");

    var filePath = Path.Combine(torrentDir, file.FileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // Start downloading using aria2
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "aria2c",
            Arguments = $"--dir={downloadDir} --input-file={filePath} --seed-time=0",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.OutputDataReceived += (sender, args) =>
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            if (int.TryParse(args.Data, out int progress))
            {
                downloadProgress[file.FileName] = progress;
            }
        }
    };
    
    process.Start();
    process.BeginOutputReadLine();
    
    return Results.Ok(new { message = "Download started" });
});//.RequireAuthorization();

apiGroup.MapGet("/status", async (string filename, CancellationToken cancellationToken) =>
{
    if (downloadProgress.TryGetValue(filename, out int progress))
    {
        return Results.Ok(new { filename, progress });
    }
    return Results.Ok(new { filename, progress = 0 });
});//.RequireAuthorization();

apiGroup.MapGet("/download", async (string filename, CancellationToken cancellationToken) =>
{
    var filePath = Path.Combine(downloadDir, filename);
    if (!File.Exists(filePath))
        return Results.NotFound();
    
    var stream = new FileStream(filePath, FileMode.Open);
    return Results.File(stream, "application/octet-stream", filename);
});//.RequireAuthorization();

app.Run();
