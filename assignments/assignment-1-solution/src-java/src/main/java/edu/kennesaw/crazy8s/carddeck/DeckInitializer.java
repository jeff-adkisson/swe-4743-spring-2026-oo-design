package edu.kennesaw.crazy8s.carddeck;

import edu.kennesaw.crazy8s.ProgramContext;
import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.cards.StandardCard;
import edu.kennesaw.crazy8s.domain.Rank;
import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.Suit;
import edu.kennesaw.crazy8s.domain.SuitType;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Creates and shuffles a new deck for the game.
 */
public final class DeckInitializer {
    private DeckInitializer() {
    }

    public static Deck createCardDeck(ProgramContext programContext) {
        List<Card> cards = createOrderedCardList();
        List<Card> shuffledCards = shuffle(programContext, cards);
        return new Deck(shuffledCards);
    }

    private static List<Card> createOrderedCardList() {
        List<Card> cards = new ArrayList<>();
        for (SuitType suit : Suit.getSuits()) {
            for (RankType rank : Rank.getRanks()) {
                cards.add(new StandardCard(rank, suit));
            }
        }

        return cards;
    }

    private static List<Card> shuffle(ProgramContext programContext, List<Card> cards) {
        List<Card> shuffledCards = new ArrayList<>(cards);
        Collections.shuffle(shuffledCards, programContext.getRandomNumberGenerator());
        return shuffledCards;
    }
}
