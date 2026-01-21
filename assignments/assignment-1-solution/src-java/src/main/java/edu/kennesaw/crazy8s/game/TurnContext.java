package edu.kennesaw.crazy8s.game;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.RankType;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.player.Player;
import java.util.Random;

/**
 * Captures the state needed for a single player's turn.
 */
public class TurnContext {
    private final GameContext gameContext;
    private final Random randomNumberGenerator;
    private final Player currentPlayer;
    private final SuitType currentSuit;
    private final Card topCard;
    private final RankType currentRank;

    public TurnContext(
            GameContext gameContext,
            Random randomNumberGenerator,
            Player currentPlayer,
            SuitType currentSuit,
            Card topCard) {
        this.gameContext = gameContext;
        this.randomNumberGenerator = randomNumberGenerator;
        this.currentPlayer = currentPlayer;
        this.currentSuit = currentSuit;
        this.topCard = topCard;
        this.currentRank = topCard.getRank();
    }

    public GameContext getGameContext() {
        return gameContext;
    }

    public Random getRandomNumberGenerator() {
        return randomNumberGenerator;
    }

    public Player getCurrentPlayer() {
        return currentPlayer;
    }

    public SuitType getCurrentSuit() {
        return currentSuit;
    }

    public Card getTopCard() {
        return topCard;
    }

    public RankType getCurrentRank() {
        return currentRank;
    }
}
