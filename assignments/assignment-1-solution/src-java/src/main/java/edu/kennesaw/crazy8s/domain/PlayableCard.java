package edu.kennesaw.crazy8s.domain;

import edu.kennesaw.crazy8s.cards.Card;

/**
 * Wraps a card with the reason it is playable.
 */
public class PlayableCard {
    private final Card card;
    private final String playableReason;

    public PlayableCard(Card card, String playableReason) {
        this.card = card;
        this.playableReason = playableReason;
    }

    public Card getCard() {
        return card;
    }

    public String getPlayableReason() {
        return playableReason;
    }
}
