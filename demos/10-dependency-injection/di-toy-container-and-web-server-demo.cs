// Tiny DI-powered web server demo
//
// dotnet run
//
// Then open:
//   http://localhost:5005/users/create
//   http://localhost:5005/users/hello
//   http://localhost:5005/users/hello?name=Jeff
//
// Note: HttpListener may require permission on some systems. If you get an access error,
// try running as admin, or change the prefix/port.

using System.Net;
using System.Text;

// ===== Mini DI container (auto constructor injection) =====
public sealed class MiniContainer
{
    private readonly Dictionary<Type, Type> _registrations = new();

    public void Register<TService, TImplementation>() where TImplementation : TService
        => _registrations[typeof(TService)] = typeof(TImplementation);

    public T Resolve<T>() => (T)Resolve(typeof(T));

    public object Resolve(Type serviceType)
    {
        if (!_registrations.TryGetValue(serviceType, out var implType))
            implType = serviceType; // allow concretes without registration

        var ctor = implType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor is null)
            throw new InvalidOperationException($"Type {implType.Name} has no public constructors.");

        var args = ctor.GetParameters()
            .Select(p => Resolve(p.ParameterType))
            .ToArray();

        return Activator.CreateInstance(implType, args)!;
    }
}

// ===== "Framework": routing + controller activation via DI =====
public sealed record Request(
    string Path,
    string Method,
    IReadOnlyDictionary<string, string?> Query);

public sealed class ToyWebApp
{
    private readonly MiniContainer _container;

    // key: "GET /path"
    private readonly Dictionary<string, (Type ControllerType, string ActionName)> _routes = new(StringComparer.OrdinalIgnoreCase);

    public ToyWebApp(MiniContainer container) => _container = container;

    public void MapGet<TController>(string path, string actionName)
        => _routes[$"GET {path}"] = (typeof(TController), actionName);

    public string Handle(Request request)
    {
        if (!_routes.TryGetValue($"{request.Method} {request.Path}", out var route))
            return "404 Not Found";

        // Controller activation via DI
        var controller = _container.Resolve(route.ControllerType);

        // Toy action invocation:
        // - parameterless action, or
        // - single optional "name" string parameter from query string
        var action = route.ControllerType.GetMethod(route.ActionName);
        if (action is null)
            return "500 Missing action";

        var parameters = action.GetParameters();

        object?[] args;
        if (parameters.Length == 0)
        {
            args = Array.Empty<object>();
        }
        else if (parameters.Length == 1 &&
                 parameters[0].ParameterType == typeof(string) &&
                 string.Equals(parameters[0].Name, "name", StringComparison.OrdinalIgnoreCase))
        {
            request.Query.TryGetValue("name", out var name);
            args = new object?[] { name };
        }
        else
        {
            return "500 Unsupported action signature";
        }

        var result = action.Invoke(controller, args);
        return result?.ToString() ?? "";
    }
}

// ===== App code: services + controller =====
public interface IMessageSender
{
    void Send(string to, string message);
}

public sealed class ConsoleMessageSender : IMessageSender
{
    public void Send(string to, string message) =>
        Console.WriteLine($"[Email] To: {to} | {message}");
}

public abstract class OnboardingService
{
    public abstract void Onboard(string email);
}

public sealed class EmailOnboardingService : OnboardingService
{
    private readonly IMessageSender _sender;

    public EmailOnboardingService(IMessageSender sender) => _sender = sender;

    public override void Onboard(string email) => _sender.Send(email, "Welcome!");
}

public abstract class WelcomeService
{
    public abstract string SayHello(string? name = null);
}

public sealed class ConsoleWelcomeService : WelcomeService
{
    public override string SayHello(string? name = null)
    {
        var message = string.IsNullOrWhiteSpace(name) ? "Hello!" : $"Hello, {name}!";
        Console.WriteLine(message);
        return message;
    }
}

public sealed class UsersController
{
    private readonly OnboardingService _onboarding;
    private readonly WelcomeService _welcome;

    public UsersController(OnboardingService onboarding, WelcomeService welcome)
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

// ===== Tiny web server using HttpListener =====
public sealed class TinyServer
{
    private readonly HttpListener _listener = new();
    private readonly ToyWebApp _app;

    public TinyServer(string prefix, ToyWebApp app)
    {
        _app = app;
        _listener.Prefixes.Add(prefix);
    }

    public async Task RunAsync()
    {
        _listener.Start();
        Console.WriteLine("Listening...");
        while (_listener.IsListening)
        {
            var ctx = await _listener.GetContextAsync();
            _ = Task.Run(() => HandleOneAsync(ctx));
        }
    }

    private async Task HandleOneAsync(HttpListenerContext ctx)
    {
        try
        {
            var path = ctx.Request.Url?.AbsolutePath ?? "/";
            var method = ctx.Request.HttpMethod ?? "GET";
            var query = (ctx.Request.QueryString.AllKeys ?? Array.Empty<string?>())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToDictionary(
                    k => k!,
                    k => ctx.Request.QueryString[k!],
                    StringComparer.OrdinalIgnoreCase);

            var responseText = _app.Handle(new Request(path, method, query));

            ctx.Response.StatusCode = responseText.StartsWith("404") ? 404 :
                                     responseText.StartsWith("500") ? 500 : 200;

            ctx.Response.ContentType = "text/plain; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(responseText);
            ctx.Response.ContentLength64 = bytes.Length;

            await using var output = ctx.Response.OutputStream;
            await output.WriteAsync(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = 500;
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes("500 Internal Server Error\n\n" + ex);
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }
        finally
        {
            try { ctx.Response.Close(); } catch { /* ignore */ }
        }
    }
}

// ===== Composition root =====
public static class Program
{
    public static async Task Main()
    {
        // DI setup (stable abstractions -> volatile details)
        var container = new MiniContainer();
        container.Register<IMessageSender, ConsoleMessageSender>();
        container.Register<OnboardingService, EmailOnboardingService>();
        container.Register<WelcomeService, ConsoleWelcomeService>();

        // App + routes
        var app = new ToyWebApp(container);
        app.MapGet<UsersController>("/users/create", nameof(UsersController.Create));
        app.MapGet<UsersController>("/users/hello", nameof(UsersController.Hello));

        // Server
        var prefix = "http://localhost:5005/";
        Console.WriteLine($"Try:\n  {prefix}users/create\n  {prefix}users/hello\n  {prefix}users/hello?name=Jeff\n");

        var server = new TinyServer(prefix, app);
        await server.RunAsync();
    }
}
