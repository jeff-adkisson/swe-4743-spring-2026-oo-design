package edu.kennesaw.crazy8s.domain;

/**
 * Defines the possible suits for a standard playing card.
 */
public enum SuitType {
    NOT_SET(0, "NotSet"),
    HEARTS(1, "Hearts"),
    DIAMONDS(2, "Diamonds"),
    CLUBS(3, "Clubs"),
    SPADES(4, "Spades");

    private final int value;
    private final String displayName;

    SuitType(int value, String displayName) {
        this.value = value;
        this.displayName = displayName;
    }

    public int getValue() {
        return value;
    }

    public String getDisplayName() {
        return displayName;
    }

    @Override
    public String toString() {
        return displayName;
    }
}
