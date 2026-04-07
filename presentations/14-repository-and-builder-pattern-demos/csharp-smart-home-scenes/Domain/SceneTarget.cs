namespace SmartHomeScenesDemo.Domain;

public enum SceneTargetKind
{
    SpecificDevice,
    DeviceGroup
}

public abstract record SceneTarget(SceneTargetKind Kind);

public sealed record SpecificDeviceTarget(Guid DeviceId)
    : SceneTarget(SceneTargetKind.SpecificDevice);

public sealed record DeviceGroupTarget(string DeviceType, string? Location)
    : SceneTarget(SceneTargetKind.DeviceGroup);
