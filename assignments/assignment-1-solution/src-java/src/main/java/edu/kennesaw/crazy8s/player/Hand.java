package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.PlayableCard;
import edu.kennesaw.crazy8s.domain.PlayableCardsSelector;
import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.SuitType;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Represents a player's hand of cards.
 */
public class Hand {
    private final List<Card> cards = new ArrayList<>();

    public int getCardCount() {
        return cards.size();
    }

    public List<Card> getCardList() {
        return Collections.unmodifiableList(cards);
    }

    public void addCard(Card card) {
        cards.add(card);
    }

    public void removeCard(Card card) {
        if (!cards.remove(card)) {
            throw new IllegalStateException("Card not found in hand.");
        }
    }

    public List<PlayableCard> getPlayableCards(SuitType currentSuit, RankType currentRank) {
        return PlayableCardsSelector.get(getCardList(), currentSuit, currentRank);
    }
}
