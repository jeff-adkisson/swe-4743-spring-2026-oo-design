package edu.kennesaw.crazy8s.domain;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Provides helpers for suit names, symbols, and lists.
 */
public final class Suit {
    private Suit() {
    }

    public static String getSuitSymbol(SuitType suit) {
        switch (suit) {
            case HEARTS:
                return "♥";
            case DIAMONDS:
                return "♦";
            case CLUBS:
                return "♣";
            case SPADES:
                return "♠";
            default:
                throw new IllegalArgumentException("Unknown suit: " + suit);
        }
    }

    public static String getSuitName(SuitType suit) {
        return String.format("%s %s", suit.getDisplayName(), getSuitSymbol(suit));
    }

    public static List<SuitType> getSuits() {
        List<SuitType> suits = new ArrayList<>();
        for (SuitType suit : SuitType.values()) {
            if (suit != SuitType.NOT_SET) {
                suits.add(suit);
            }
        }

        return Collections.unmodifiableList(suits);
    }
}
