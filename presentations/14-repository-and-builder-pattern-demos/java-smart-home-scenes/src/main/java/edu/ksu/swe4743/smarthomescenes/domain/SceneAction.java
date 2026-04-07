package edu.ksu.swe4743.smarthomescenes.domain;

import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Objects;
import java.util.UUID;

public final class SceneAction {
    private final UUID id;
    private final int orderIndex;
    private final SceneTarget target;
    private final String operation;
    private final Map<String, String> parameters;

    public SceneAction(
        UUID id,
        int orderIndex,
        SceneTarget target,
        String operation,
        Map<String, String> parameters
    ) {
        this.id = Objects.requireNonNull(id, "id");
        this.orderIndex = orderIndex;
        this.target = Objects.requireNonNull(target, "target");
        this.operation = Objects.requireNonNull(operation, "operation").trim();
        this.parameters = Map.copyOf(new LinkedHashMap<>(parameters));
    }

    public UUID getId() {
        return id;
    }

    public int getOrderIndex() {
        return orderIndex;
    }

    public SceneTarget getTarget() {
        return target;
    }

    public String getOperation() {
        return operation;
    }

    public Map<String, String> getParameters() {
        return parameters;
    }
}
