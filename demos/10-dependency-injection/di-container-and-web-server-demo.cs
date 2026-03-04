#!/usr/bin/dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:property Nullable=enable

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5005");

// DI setup (stable abstractions -> volatile details)
builder.Services.AddTransient<IMessageSender, ConsoleMessageSender>();
builder.Services.AddTransient<IOnboardingService, EmailOnboardingService>();
builder.Services.AddTransient<IWelcomeService, ConsoleWelcomeService>();
builder.Services.AddTransient<UsersController>();

var app = builder.Build();

app.MapGet(
    "/users/create",
    ([FromServices] UsersController controller) => Results.Text(controller.Create(), "text/plain; charset=utf-8"));

app.MapGet(
    "/users/hello",
    ([FromServices] UsersController controller, string? name) =>
        Results.Text(controller.Hello(name), "text/plain; charset=utf-8"));

Console.WriteLine("Try:");
Console.WriteLine("  http://localhost:5005/users/create");
Console.WriteLine("  http://localhost:5005/users/hello");
Console.WriteLine("  http://localhost:5005/users/hello?name=Jeff");
Console.WriteLine();

app.Run();

public interface IMessageSender
{
    void Send(string to, string message);
}

public sealed class ConsoleMessageSender : IMessageSender
{
    public void Send(string to, string message)
        => Console.WriteLine($"[Email] To: {to} | {message}");
}

public interface IOnboardingService
{
    void Onboard(string email);
}

public sealed class EmailOnboardingService : IOnboardingService
{
    private readonly IMessageSender _sender;

    public EmailOnboardingService(IMessageSender sender)
    {
        _sender = sender;
    }

    public void Onboard(string email)
    {
        _sender.Send(email, "Welcome!");
    }
}

public interface IWelcomeService
{
    string SayHello(string? name = null);
}


public sealed class ConsoleWelcomeService : IWelcomeService
{
    public string SayHello(string? name = null)
    {
        var message = string.IsNullOrWhiteSpace(name) ? "Hello!" : $"Hello, {name}!";
        Console.WriteLine(message);
        return message;
    }
}

public sealed class UsersController
{
    private readonly IOnboardingService _onboarding;
    private readonly IWelcomeService _welcome;

    public UsersController(IOnboardingService onboarding, IWelcomeService welcome)
    {
        _onboarding = onboarding;
        _welcome = welcome;
    }

    public string Create()
    {
        _onboarding.Onboard("user@example.com");
        return "201 Created (onboarded user@example.com)";
    }

    public string Hello(string? name = null)
    {
        return _welcome.SayHello(name);
    }
}
