namespace backend.Models;

public sealed class ChatSession
{
    public ChatSession(string sessionId, string name, string queueName, DateTime lastSeenUtc)
    {
        SessionId = sessionId;
        Name = name;
        QueueName = queueName;
        LastSeenUtc = lastSeenUtc;
    }

    public string SessionId { get; }

    public string Name { get; }

    public string QueueName { get; }

    public DateTime LastSeenUtc { get; set; }
}
