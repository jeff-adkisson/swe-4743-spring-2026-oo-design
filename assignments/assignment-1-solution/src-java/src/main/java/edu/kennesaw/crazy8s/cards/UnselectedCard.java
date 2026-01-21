package edu.kennesaw.crazy8s.cards;

import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.SuitType;

/**
 * Null-object card used when no card is selected.
 */
public final class UnselectedCard implements Card {
    private static final UnselectedCard INSTANCE = new UnselectedCard();

    private UnselectedCard() {
    }

    public static UnselectedCard getInstance() {
        return INSTANCE;
    }

    @Override
    public RankType getRank() {
        return RankType.NOT_SET;
    }

    @Override
    public SuitType getSuit() {
        return SuitType.NOT_SET;
    }

    @Override
    public int getCardId() {
        return -1;
    }

    @Override
    public String getDescription() {
        return "Unselected Card";
    }

    @Override
    public boolean isSelectable() {
        return false;
    }
}
