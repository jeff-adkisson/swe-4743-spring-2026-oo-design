namespace SmartHomeScenesDemo.Domain;

public sealed record SceneAction(
    Guid Id,
    int OrderIndex,
    SceneTarget Target,
    string Operation,
    IReadOnlyDictionary<string, string> Parameters);
