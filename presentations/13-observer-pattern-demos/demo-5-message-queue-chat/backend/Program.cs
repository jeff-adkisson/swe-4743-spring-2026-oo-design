using backend.Models;
using backend.Options;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<ChatSessionRegistry>();
builder.Services.AddSingleton<RabbitChatBus>();
builder.Services.AddSingleton<ChatCoordinator>();
builder.Services.AddHostedService<StaleSessionCleanupService>();

var app = builder.Build();

await app.Services.GetRequiredService<RabbitChatBus>().InitializeAsync(app.Lifetime.ApplicationStopping);

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.MapGet("/api/chat/session/{sessionId}", (string sessionId, ChatCoordinator coordinator) =>
{
    return Results.Ok(coordinator.GetSessionStatus(sessionId));
});

app.MapPost("/api/chat/join", async (
    JoinChatRequest request,
    ChatCoordinator coordinator,
    CancellationToken cancellationToken) =>
{
    try
    {
        var response = await coordinator.JoinAsync(request.Name, cancellationToken);
        return Results.Ok(response);
    }
    catch (ChatInputException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/api/chat/heartbeat", (SessionRequest request, ChatCoordinator coordinator) =>
{
    return coordinator.Touch(request.SessionId)
        ? Results.Ok()
        : Results.NotFound("That chat session is no longer active.");
});

app.MapPost("/api/chat/messages", async (
    SendMessageRequest request,
    ChatCoordinator coordinator,
    CancellationToken cancellationToken) =>
{
    try
    {
        await coordinator.SendMessageAsync(request.SessionId, request.Text, cancellationToken);
        return Results.Ok();
    }
    catch (ChatInputException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/api/chat/leave", async (
    SessionRequest request,
    ChatCoordinator coordinator,
    CancellationToken cancellationToken) =>
{
    await coordinator.LeaveAsync(request.SessionId, cancellationToken);
    return Results.Ok();
});

app.MapGet("/api/chat/stream/{sessionId}", async (
    string sessionId,
    HttpContext httpContext,
    ChatCoordinator coordinator,
    CancellationToken cancellationToken) =>
{
    try
    {
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";
        httpContext.Response.Headers["X-Accel-Buffering"] = "no";
        httpContext.Response.ContentType = "text/event-stream";

        await coordinator.StreamAsync(sessionId, httpContext.Response, cancellationToken);
        return Results.Empty;
    }
    catch (ChatInputException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/health", () => Results.Ok(new { ok = true }));

app.MapFallbackToFile("index.html");

app.Run();
