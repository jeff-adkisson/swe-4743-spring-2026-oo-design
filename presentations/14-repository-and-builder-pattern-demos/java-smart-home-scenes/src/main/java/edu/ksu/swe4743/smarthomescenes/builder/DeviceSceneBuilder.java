package edu.ksu.swe4743.smarthomescenes.builder;

import edu.ksu.swe4743.smarthomescenes.domain.DeviceGroupTarget;
import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import edu.ksu.swe4743.smarthomescenes.domain.SceneAction;
import edu.ksu.swe4743.smarthomescenes.domain.SceneTarget;
import edu.ksu.swe4743.smarthomescenes.domain.SpecificDeviceTarget;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

public final class DeviceSceneBuilder implements SceneBuilder {
    private String name;
    private final List<SceneAction> actions = new ArrayList<>();

    @Override
    public void reset() {
        name = null;
        actions.clear();
    }

    @Override
    public void setName(String name) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("Scene name is required.");
        }

        this.name = name.trim();
    }

    @Override
    public void addAction(SceneAction action) {
        actions.add(action);
    }

    public DeviceSceneBuilder named(String name) {
        setName(name);
        return this;
    }

    public DeviceSceneBuilder addDeviceAction(
        UUID deviceId,
        String operation,
        Map<String, String> parameters
    ) {
        return addActionInternal(new SpecificDeviceTarget(deviceId), operation, parameters);
    }

    public DeviceSceneBuilder addGroupAction(
        String deviceType,
        String location,
        String operation,
        Map<String, String> parameters
    ) {
        if (deviceType == null || deviceType.isBlank()) {
            throw new IllegalArgumentException("Device type is required.");
        }

        return addActionInternal(new DeviceGroupTarget(deviceType, location), operation, parameters);
    }

    @Override
    public DeviceScene build() {
        if (name == null || name.isBlank()) {
            throw new IllegalStateException("Cannot build a DeviceScene without a name.");
        }

        if (actions.isEmpty()) {
            throw new IllegalStateException("Cannot build a DeviceScene without at least one action.");
        }

        var copiedActions = actions.stream()
            .map(action -> new SceneAction(
                action.getId(),
                action.getOrderIndex(),
                action.getTarget(),
                action.getOperation(),
                new LinkedHashMap<>(action.getParameters())))
            .toList();

        return new DeviceScene(UUID.randomUUID(), name, copiedActions);
    }

    private DeviceSceneBuilder addActionInternal(
        SceneTarget target,
        String operation,
        Map<String, String> parameters
    ) {
        if (operation == null || operation.isBlank()) {
            throw new IllegalArgumentException("Operation is required.");
        }

        var safeParameters = parameters == null
            ? Map.<String, String>of()
            : Map.copyOf(new LinkedHashMap<>(parameters));

        actions.add(new SceneAction(
            UUID.randomUUID(),
            actions.size(),
            target,
            operation.trim(),
            safeParameters));

        return this;
    }
}
