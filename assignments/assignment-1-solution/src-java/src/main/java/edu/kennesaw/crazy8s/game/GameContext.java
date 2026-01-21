package edu.kennesaw.crazy8s.game;

import edu.kennesaw.crazy8s.ProgramContext;
import edu.kennesaw.crazy8s.carddeck.Deck;
import edu.kennesaw.crazy8s.carddeck.DiscardPile;
import edu.kennesaw.crazy8s.player.Players;
import java.util.Random;

/**
 * Holds shared game state and configuration values.
 */
public class GameContext {
    private final ProgramContext programContext;
    private final Deck deck;
    private final DiscardPile discardPile;
    private final Players players;
    private final boolean showAllHands;
    private int turnNumber;

    public GameContext(ProgramContext programContext, Deck deck, DiscardPile discardPile, Players players) {
        this.programContext = programContext;
        this.deck = deck;
        this.discardPile = discardPile;
        this.players = players;
        this.showAllHands = programContext.isShowAllHands();
    }

    public Random getRandomNumberGenerator() {
        return programContext.getRandomNumberGenerator();
    }

    public boolean isShowAllHands() {
        return showAllHands;
    }

    public String getGameTitle() {
        return "Crazy Eights (Simplified)";
    }

    public Deck getDeck() {
        return deck;
    }

    public DiscardPile getDiscardPile() {
        return discardPile;
    }

    public Players getPlayers() {
        return players;
    }

    public int getHandSize() {
        return programContext.getHandSize();
    }

    public int getTurnNumber() {
        return turnNumber;
    }

    public int incrementTurn() {
        turnNumber += 1;
        return turnNumber;
    }
}
