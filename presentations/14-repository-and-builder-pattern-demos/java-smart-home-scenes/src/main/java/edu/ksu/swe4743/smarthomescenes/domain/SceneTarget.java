package edu.ksu.swe4743.smarthomescenes.domain;

public abstract class SceneTarget {
    private final SceneTargetKind kind;

    protected SceneTarget(SceneTargetKind kind) {
        this.kind = kind;
    }

    public SceneTargetKind getKind() {
        return kind;
    }
}
