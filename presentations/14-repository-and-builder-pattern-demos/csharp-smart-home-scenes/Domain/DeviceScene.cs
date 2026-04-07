namespace SmartHomeScenesDemo.Domain;

public sealed class DeviceScene
{
    public DeviceScene(Guid id, string name, IReadOnlyList<SceneAction> actions)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Scene id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Scene name is required.", nameof(name));
        }

        if (actions.Count == 0)
        {
            throw new ArgumentException("A scene must contain at least one action.", nameof(actions));
        }

        Id = id;
        Name = name;
        Actions = actions
            .OrderBy(action => action.OrderIndex)
            .ToArray();
    }

    public Guid Id { get; }

    public string Name { get; }

    public IReadOnlyList<SceneAction> Actions { get; }
}
