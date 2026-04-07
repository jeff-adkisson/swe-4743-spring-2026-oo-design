package edu.ksu.swe4743.smarthomescenes.builder;

import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import java.util.Map;
import java.util.UUID;

public final class SceneDirector {
    private final DeviceSceneBuilder builder;

    public SceneDirector(DeviceSceneBuilder builder) {
        this.builder = builder;
    }

    public DeviceScene buildEveningArrival(UUID porchLightId) {
        builder.reset();

        return builder
            .named("Evening Arrival")
            .addDeviceAction(porchLightId, "TurnOn", Map.of())
            .addGroupAction("Light", "Living Room", "TurnOn", Map.of())
            .addGroupAction("Light", "Living Room", "SetBrightness", Map.of("brightness", "40"))
            .build();
    }
}
