package edu.kennesaw.crazy8s.carddeck;

import edu.kennesaw.crazy8s.cards.Card;
import java.util.ArrayDeque;
import java.util.Deque;
import java.util.List;

/**
 * Represents a stack-based deck of cards to draw from.
 */
public class Deck {
    private final Deque<Card> cards = new ArrayDeque<>();

    public Deck(List<Card> cards) {
        if (cards == null) {
            throw new IllegalArgumentException("Cards cannot be null.");
        }
        for (Card card : cards) {
            this.cards.push(card);
        }
    }

    public int getCardCount() {
        return cards.size();
    }

    public boolean isEmpty() {
        return cards.isEmpty();
    }

    public Card drawCard() {
        if (isEmpty()) {
            throw new IllegalStateException("Deck is empty.");
        }

        return cards.pop();
    }
}
