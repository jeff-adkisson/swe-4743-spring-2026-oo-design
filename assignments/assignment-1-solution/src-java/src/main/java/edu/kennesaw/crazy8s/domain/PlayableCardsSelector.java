package edu.kennesaw.crazy8s.domain;

import edu.kennesaw.crazy8s.cards.Card;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Determines which cards can be played given the current suit and rank.
 */
public final class PlayableCardsSelector {
    private PlayableCardsSelector() {
    }

    public static List<PlayableCard> get(List<Card> cards, SuitType currentSuit, RankType currentRank) {
        List<PlayableCard> playableCards = new ArrayList<>();
        for (Card card : cards) {
            IsPlayable outcome = canPlayCard(card, currentSuit, currentRank);
            if (!outcome.canPlay()) {
                continue;
            }

            PlayableCard playableCard = new PlayableCard(card, outcome.getReason());
            playableCards.add(playableCard);
        }

        return Collections.unmodifiableList(playableCards);
    }

    public static IsPlayable canPlayCard(Card card, SuitType currentSuitType, RankType currentRankType) {
        if (Rank.isWildcardRank(card.getRank())) {
            return new IsPlayable(true, "Wildcard!");
        }

        if (currentRankType == card.getRank()) {
            return new IsPlayable(true, "Matches Rank");
        }

        if (currentSuitType == card.getSuit()) {
            return new IsPlayable(true, "Matches Suit");
        }

        return new IsPlayable(false, "Not Playable");
    }
}
