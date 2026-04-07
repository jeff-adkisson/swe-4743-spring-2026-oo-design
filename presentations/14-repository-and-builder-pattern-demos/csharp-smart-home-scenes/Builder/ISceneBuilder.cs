using SmartHomeScenesDemo.Domain;

namespace SmartHomeScenesDemo.Builder;

public interface ISceneBuilder
{
    void Reset();

    void SetName(string name);

    void AddAction(SceneAction action);

    DeviceScene Build();
}
