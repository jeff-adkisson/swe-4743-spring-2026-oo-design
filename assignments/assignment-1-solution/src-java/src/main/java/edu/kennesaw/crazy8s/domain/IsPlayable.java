package edu.kennesaw.crazy8s.domain;

/**
 * Represents the outcome of a playability check.
 */
public class IsPlayable {
    private final boolean canPlay;
    private final String reason;

    public IsPlayable(boolean canPlay, String reason) {
        this.canPlay = canPlay;
        this.reason = reason;
    }

    public boolean canPlay() {
        return canPlay;
    }

    public String getReason() {
        return reason;
    }
}
