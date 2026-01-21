package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.game.GameContext;
import edu.kennesaw.crazy8s.game.TurnContext;
import java.util.List;

/**
 * Represents a player in the Crazy Eights game.
 * Defines properties and methods required for player interactions,
 * such as managing the player's hand, selecting cards, and determining actions during a turn.
 */
public interface Player {
    /**
     * Gets the display name for the player.
     */
    String getName();

    /**
     * Gets the number of cards currently in the player's hand.
     */
    int getCardCount();

    /**
     * Gets a value indicating whether this player's hand should be shown.
     */
    boolean isShowHand();

    /**
     * Adds a card to the player's hand.
     */
    void addCard(Card card);

    /**
     * Removes a card from the player's hand.
     */
    void removeCard(Card card);

    /**
     * Returns a read-only view of the player's current hand.
     */
    List<Card> peekHand();

    /**
     * Executes the player's turn actions.
     */
    void takeTurn(TurnContext context);

    /**
     * Selects a card to play for the current turn.
     */
    Card selectCard(TurnContext context);

    /**
     * Selects a suit when a wildcard card is played.
     */
    SuitType selectSuit(GameContext gameContext, TurnContext turnContext);

    /**
     * Determines whether the player will immediately play a drawn card.
     */
    boolean willPlayDrawnCard(TurnContext turnContext, Card drawnCard);
}
