package edu.ksu.swe4743.smarthomescenes.domain;

import java.util.Objects;

public final class DeviceGroupTarget extends SceneTarget {
    private final String deviceType;
    private final String location;

    public DeviceGroupTarget(String deviceType, String location) {
        super(SceneTargetKind.DEVICE_GROUP);
        this.deviceType = Objects.requireNonNull(deviceType, "deviceType").trim();
        this.location = location == null || location.isBlank() ? null : location.trim();
    }

    public String getDeviceType() {
        return deviceType;
    }

    public String getLocation() {
        return location;
    }
}
