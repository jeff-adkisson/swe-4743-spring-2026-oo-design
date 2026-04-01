namespace backend.Models;

public sealed record ChatEnvelope(
    string Id,
    string Kind,
    string Author,
    string Text,
    DateTime SentAtUtc);
