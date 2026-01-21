package edu.kennesaw.crazy8s.carddeck;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.SuitType;
import java.util.ArrayDeque;
import java.util.Deque;

/**
 * Represents the discard pile and active suit state.
 */
public class DiscardPile {
    private final Deque<Card> discardedCards = new ArrayDeque<>();
    private SuitType overriddenSuit = SuitType.NOT_SET;

    public Card getTopCard() {
        if (discardedCards.isEmpty()) {
            throw new IllegalStateException("Discard pile is empty.");
        }

        return discardedCards.peek();
    }

    public SuitType getActiveSuit() {
        return overriddenSuit == SuitType.NOT_SET ? getTopCard().getSuit() : overriddenSuit;
    }

    public void addCard(Card card) {
        discardedCards.push(card);
        setTopCardAsActiveSuit();
    }

    private void setTopCardAsActiveSuit() {
        overriddenSuit = SuitType.NOT_SET;
    }

    public void overrideTopCardSuit(SuitType newSuit) {
        if (getTopCard().getSuit() == newSuit) {
            setTopCardAsActiveSuit();
            return;
        }

        overriddenSuit = newSuit;
    }
}
