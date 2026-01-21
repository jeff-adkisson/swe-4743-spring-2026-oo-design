package edu.kennesaw.crazy8s.cards;

import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.SuitType;

/**
 * Defines the core properties and behaviors of a card.
 */
public interface Card {
    /**
     * Gets the rank of the card.
     */
    RankType getRank();

    /**
     * Gets the suit of the card.
     */
    SuitType getSuit();

    /**
     * Gets a unique identifier for the card.
     */
    int getCardId();

    /**
     * Gets a value indicating whether the card can be selected to play.
     */
    boolean isSelectable();

    /**
     * Returns a human-readable description of the card.
     */
    String getDescription();
}
