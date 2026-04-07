using SmartHomeScenesDemo.Domain;

namespace SmartHomeScenesDemo.Builder;

public sealed class SceneDirector
{
    private readonly DeviceSceneBuilder _builder;

    public SceneDirector(DeviceSceneBuilder builder)
    {
        _builder = builder;
    }

    public DeviceScene BuildEveningArrival(Guid porchLightId)
    {
        _builder.Reset();

        return _builder
            .Named("Evening Arrival")
            .AddDeviceAction(porchLightId, "TurnOn")
            .AddGroupAction("Light", "Living Room", "TurnOn")
            .AddGroupAction(
                "Light",
                "Living Room",
                "SetBrightness",
                new Dictionary<string, string>
                {
                    ["brightness"] = "40"
                })
            .Build();
    }
}
