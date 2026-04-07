package edu.ksu.swe4743.smarthomescenes.builder;

import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import edu.ksu.swe4743.smarthomescenes.domain.SceneAction;

public interface SceneBuilder {
    void reset();

    void setName(String name);

    void addAction(SceneAction action);

    DeviceScene build();
}
