using System.Text.RegularExpressions;
using backend.Models;

namespace backend.Services;

public sealed class ChatCoordinator
{
    private static readonly Regex NamePattern = new("^[a-z0-9_-]+$", RegexOptions.Compiled);

    private readonly ChatSessionRegistry _registry;
    private readonly RabbitChatBus _bus;

    public ChatCoordinator(ChatSessionRegistry registry, RabbitChatBus bus)
    {
        _registry = registry;
        _bus = bus;
    }

    public async Task<JoinChatResponse> JoinAsync(string requestedName, CancellationToken cancellationToken)
    {
        var name = requestedName.Trim();

        if (!NamePattern.IsMatch(name))
        {
            throw new ChatInputException(
                "Usernames may only contain lowercase letters, numbers, dashes, and underscores.");
        }

        if (!_registry.TryRegister(name, out var session, out var error) || session is null)
        {
            throw new ChatInputException(error ?? "This username is unavailable.");
        }

        try
        {
            await _bus.EnsureSessionQueueAsync(session.QueueName, cancellationToken);

            await _bus.PublishAsync(
                CreateSystemMessage("system", $"{session.Name} has joined the chat..."),
                cancellationToken);

            return new JoinChatResponse(session.Name, session.QueueName, session.SessionId);
        }
        catch
        {
            _registry.Remove(session.SessionId);
            await _bus.DeleteQueueAsync(session.QueueName, cancellationToken);
            throw;
        }
    }

    public SessionStatusResponse GetSessionStatus(string sessionId)
    {
        var session = _registry.Get(sessionId);

        return session is null
            ? new SessionStatusResponse(false, null, null, null)
            : new SessionStatusResponse(true, session.Name, session.QueueName, session.SessionId);
    }

    public bool Touch(string sessionId) => _registry.Touch(sessionId);

    public async Task SendMessageAsync(string sessionId, string rawText, CancellationToken cancellationToken)
    {
        var session = _registry.Get(sessionId)
            ?? throw new ChatInputException("That chat session is no longer active.");

        _registry.Touch(sessionId);

        var text = rawText.Trim();
        if (text.Length == 0)
        {
            throw new ChatInputException("Messages cannot be empty.");
        }

        if (text.Length > 280)
        {
            throw new ChatInputException("Messages must be 280 characters or fewer.");
        }

        await _bus.PublishAsync(
            new ChatEnvelope(
                Guid.NewGuid().ToString("N"),
                "chat",
                session.Name,
                text,
                DateTime.UtcNow),
            cancellationToken);
    }

    public Task StreamAsync(string sessionId, HttpResponse response, CancellationToken cancellationToken)
    {
        var session = _registry.Get(sessionId)
            ?? throw new ChatInputException("That chat session is no longer active.");

        _registry.Touch(sessionId);
        return _bus.StreamQueueToSseAsync(session.QueueName, response, cancellationToken);
    }

    public async Task<bool> LeaveAsync(string sessionId, CancellationToken cancellationToken)
    {
        var session = _registry.Remove(sessionId);
        if (session is null)
        {
            return false;
        }

        await _bus.DeleteQueueAsync(session.QueueName, cancellationToken);
        await _bus.PublishAsync(
            CreateSystemMessage("system", $"{session.Name} has left the chat..."),
            cancellationToken);

        return true;
    }

    public async Task<int> RemoveStaleSessionsAsync(TimeSpan maxIdle, CancellationToken cancellationToken)
    {
        var staleSessions = _registry.GetStaleSessions(DateTime.UtcNow, maxIdle);
        var removed = 0;

        foreach (var session in staleSessions)
        {
            if (await LeaveAsync(session.SessionId, cancellationToken))
            {
                removed++;
            }
        }

        return removed;
    }

    private static ChatEnvelope CreateSystemMessage(string author, string text) =>
        new(
            Guid.NewGuid().ToString("N"),
            "system",
            author,
            text,
            DateTime.UtcNow);
}

public sealed class ChatInputException : Exception
{
    public ChatInputException(string message) : base(message)
    {
    }
}
