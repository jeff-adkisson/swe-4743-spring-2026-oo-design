namespace backend.Models;

public sealed record JoinChatRequest(string Name);

public sealed record JoinChatResponse(string Name, string QueueName, string SessionId);

public sealed record SessionStatusResponse(bool Active, string? Name, string? QueueName, string? SessionId);

public sealed record SendMessageRequest(string SessionId, string Text);

public sealed record SessionRequest(string SessionId);
