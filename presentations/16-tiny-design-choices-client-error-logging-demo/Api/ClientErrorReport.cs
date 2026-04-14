namespace Api;

public sealed record ClientErrorReport(
    string Message,
    string? Stack,
    string Url,
    string? User,
    string Timestamp,
    string UserAgent,
    string? Component
);
