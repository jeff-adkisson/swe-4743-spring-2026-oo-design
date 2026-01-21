package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.carddeck.Deck;
import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.game.GameContext;

/**
 * Deals starting hands to players from the deck.
 */
public final class HandDealer {
    private HandDealer() {
    }

    public static Hand[] deal(GameContext gameContext, int numberOfPlayers) {
        Hand[] hands = new Hand[numberOfPlayers];
        Deck deck = gameContext.getDeck();
        int handSize = gameContext.getHandSize();

        if (deck.getCardCount() < numberOfPlayers * handSize) {
            throw new IllegalStateException("Not enough cards in the deck to deal hands to all players.");
        }

        for (int i = 0; i < numberOfPlayers; i++) {
            Hand hand = new Hand();
            for (int j = 0; j < handSize; j++) {
                Card card = deck.drawCard();
                hand.addCard(card);
            }

            hands[i] = hand;
        }

        return hands;
    }
}
