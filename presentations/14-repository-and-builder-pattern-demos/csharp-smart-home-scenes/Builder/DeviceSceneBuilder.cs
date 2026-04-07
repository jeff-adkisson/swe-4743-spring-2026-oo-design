using SmartHomeScenesDemo.Domain;

namespace SmartHomeScenesDemo.Builder;

public sealed class DeviceSceneBuilder : ISceneBuilder
{
    private string? _name;
    private readonly List<SceneAction> _actions = new();

    public void Reset()
    {
        _name = null;
        _actions.Clear();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Scene name is required.", nameof(name));
        }

        _name = name.Trim();
    }

    public void AddAction(SceneAction action)
    {
        _actions.Add(action);
    }

    public DeviceSceneBuilder Named(string name)
    {
        SetName(name);
        return this;
    }

    public DeviceSceneBuilder AddDeviceAction(
        Guid deviceId,
        string operation,
        IReadOnlyDictionary<string, string>? parameters = null)
    {
        return AddActionInternal(
            new SpecificDeviceTarget(deviceId),
            operation,
            parameters);
    }

    public DeviceSceneBuilder AddGroupAction(
        string deviceType,
        string? location,
        string operation,
        IReadOnlyDictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(deviceType))
        {
            throw new ArgumentException("Device type is required.", nameof(deviceType));
        }

        return AddActionInternal(
            new DeviceGroupTarget(deviceType.Trim(), string.IsNullOrWhiteSpace(location) ? null : location.Trim()),
            operation,
            parameters);
    }

    public DeviceScene Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            throw new InvalidOperationException("Cannot build a DeviceScene without a name.");
        }

        if (_actions.Count == 0)
        {
            throw new InvalidOperationException("Cannot build a DeviceScene without at least one action.");
        }

        var scene = new DeviceScene(
            Guid.NewGuid(),
            _name,
            _actions
                .OrderBy(action => action.OrderIndex)
                .Select(action => new SceneAction(
                    action.Id,
                    action.OrderIndex,
                    action.Target,
                    action.Operation,
                    new Dictionary<string, string>(action.Parameters)))
                .ToArray());

        return scene;
    }

    private DeviceSceneBuilder AddActionInternal(
        SceneTarget target,
        string operation,
        IReadOnlyDictionary<string, string>? parameters)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            throw new ArgumentException("Operation is required.", nameof(operation));
        }

        var orderIndex = _actions.Count;
        var safeParameters = parameters is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(parameters);

        _actions.Add(new SceneAction(
            Guid.NewGuid(),
            orderIndex,
            target,
            operation.Trim(),
            safeParameters));

        return this;
    }
}
