using backend.Models;

namespace backend.Services;

public sealed class ChatSessionRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, ChatSession> _sessionsById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _sessionIdsByName = new(StringComparer.Ordinal);

    public bool TryRegister(string name, out ChatSession? session, out string? error)
    {
        lock (_sync)
        {
            if (_sessionIdsByName.ContainsKey(name))
            {
                session = null;
                error = $"The name '{name}' is already connected.";
                return false;
            }

            var sessionId = Guid.NewGuid().ToString("N");
            var queueName = $"wacky-chat.visitor.{name}.{sessionId[..8]}";
            session = new ChatSession(sessionId, name, queueName, DateTime.UtcNow);

            _sessionsById[session.SessionId] = session;
            _sessionIdsByName[name] = session.SessionId;
            error = null;
            return true;
        }
    }

    public ChatSession? Get(string sessionId)
    {
        lock (_sync)
        {
            return _sessionsById.GetValueOrDefault(sessionId);
        }
    }

    public bool Touch(string sessionId)
    {
        lock (_sync)
        {
            if (!_sessionsById.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            session.LastSeenUtc = DateTime.UtcNow;
            return true;
        }
    }

    public ChatSession? Remove(string sessionId)
    {
        lock (_sync)
        {
            if (!_sessionsById.Remove(sessionId, out var session))
            {
                return null;
            }

            _sessionIdsByName.Remove(session.Name);
            return session;
        }
    }

    public IReadOnlyList<ChatSession> GetStaleSessions(DateTime nowUtc, TimeSpan maxIdle)
    {
        lock (_sync)
        {
            return _sessionsById.Values
                .Where(session => nowUtc - session.LastSeenUtc > maxIdle)
                .ToList();
        }
    }
}
