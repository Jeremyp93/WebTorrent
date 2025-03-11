using LouisManager.Api.Extensions;
using LouisManager.Api.Models;
using LouisManager.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureAutoMapper();
builder.Services.ConfigureHelpers();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureCors();
builder.Services.ConfigureMongoDb(builder.Configuration);
builder.Services.SetupCookieAuthentication(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddAuthorization();
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

private readonly string _downloadDir = "/tmp/torrent_downloads";
private readonly string _torrentDir = "/tmp/torrents";

Directory.CreateDirectory(_downloadDir);
Directory.CreateDirectory(_torrentDir);

var apiGroup = app.MapGroup("/api");

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

apiGroup.MapPost("/upload", async (IFormFile file, CancellationToken cancellationToken) =>
{
    if (file == null || file.Length == 0)
            return BadRequest("Invalid file");

        var filePath = Path.Combine(_torrentDir, file.FileName);
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
                Arguments = $"--dir={_downloadDir} --input-file={filePath} --seed-time=0",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        return Results.Ok(new { message = "Download started" });
}).RequireAuthorization();

apiGroup.MapGet("/status", async (CancellationToken cancellationToken) =>
{
    return Results.Ok(new { status = "Downloading..." })
}).RequireAuthorization();

apiGroup.MapGet("/download", async (CancellationToken cancellationToken) =>
{
    var filePath = Path.Combine(_downloadDir, filename);
    if (!System.IO.File.Exists(filePath))
        return NotFound();
    
    var stream = new FileStream(filePath, FileMode.Open);
    return File(stream, "application/octet-stream", filename);
}).RequireAuthorization();

app.Run();
