package edu.ksu.swe4743.smarthomescenes.domain;

import java.util.Objects;
import java.util.UUID;

public final class SpecificDeviceTarget extends SceneTarget {
    private final UUID deviceId;

    public SpecificDeviceTarget(UUID deviceId) {
        super(SceneTargetKind.SPECIFIC_DEVICE);
        this.deviceId = Objects.requireNonNull(deviceId, "deviceId");
    }

    public UUID getDeviceId() {
        return deviceId;
    }
}
