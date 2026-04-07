package edu.ksu.swe4743.smarthomescenes.repository;

import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import java.util.List;
import java.util.UUID;

public interface SceneRepository {
    void save(DeviceScene scene);

    DeviceScene getById(UUID id);

    List<DeviceScene> listAll();
}
