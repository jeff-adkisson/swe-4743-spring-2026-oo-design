package edu.kennesaw.crazy8s.cards;

import edu.kennesaw.crazy8s.domain.Rank;
import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.Suit;
import edu.kennesaw.crazy8s.domain.SuitType;

/**
 * Represents a standard playing card with rank and suit.
 */
public class StandardCard implements Card {
    private final RankType rank;
    private final SuitType suit;

    public StandardCard(RankType rank, SuitType suit) {
        this.rank = rank;
        this.suit = suit;
    }

    @Override
    public String getDescription() {
        String rankName = Rank.getRankName(rank);
        String suitName = Suit.getSuitName(suit);
        return String.format("%s of %s", rankName, suitName);
    }

    @Override
    public boolean isSelectable() {
        return true;
    }

    @Override
    public RankType getRank() {
        return rank;
    }

    @Override
    public SuitType getSuit() {
        return suit;
    }

    @Override
    public int getCardId() {
        return suit.getValue() * 100 + rank.getValue();
    }

    @Override
    public String toString() {
        return getDescription();
    }
}
