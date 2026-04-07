package edu.ksu.swe4743.smarthomescenes.domain;

import java.util.Comparator;
import java.util.List;
import java.util.Objects;
import java.util.UUID;

public final class DeviceScene {
    private final UUID id;
    private final String name;
    private final List<SceneAction> actions;

    public DeviceScene(UUID id, String name, List<SceneAction> actions) {
        this.id = Objects.requireNonNull(id, "id");
        this.name = Objects.requireNonNull(name, "name").trim();

        if (this.name.isBlank()) {
            throw new IllegalArgumentException("Scene name is required.");
        }

        if (actions.isEmpty()) {
            throw new IllegalArgumentException("A scene must contain at least one action.");
        }

        this.actions = actions.stream()
            .sorted(Comparator.comparingInt(SceneAction::getOrderIndex))
            .toList();
    }

    public UUID getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public List<SceneAction> getActions() {
        return actions;
    }
}
