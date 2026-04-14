using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// CORS — allow the Angular dev server (ng serve) and the Dockerized client (nginx)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Rate limiting — protect the client-log endpoint
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("client-log", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

app.UseCors();
app.UseRateLimiter();
app.MapControllers();

app.Run();
