using SmartHomeScenesDemo.Domain;

namespace SmartHomeScenesDemo.Repository;

public interface ISceneRepository
{
    void Save(DeviceScene scene);

    DeviceScene? GetById(Guid id);

    IReadOnlyList<DeviceScene> ListAll();
}
