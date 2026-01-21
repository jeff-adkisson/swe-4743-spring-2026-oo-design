package edu.kennesaw.crazy8s.game;

import edu.kennesaw.crazy8s.ProgramContext;
import edu.kennesaw.crazy8s.carddeck.Deck;
import edu.kennesaw.crazy8s.carddeck.DeckInitializer;
import edu.kennesaw.crazy8s.carddeck.DiscardPile;
import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.player.Player;
import edu.kennesaw.crazy8s.player.PlayerRegistration;
import edu.kennesaw.crazy8s.player.Players;
import java.util.List;
import java.util.Random;

/**
 * Runs a single game session from setup through winner determination.
 */
public class GameEngine {
    private final GameContext gameContext;

    public GameEngine(ProgramContext programContext) {
        Deck deck = DeckInitializer.createCardDeck(programContext);

        this.gameContext = new GameContext(
                programContext,
                deck,
                new DiscardPile(),
                new Players());
    }

    public void startGame() {
        PlayerRegistration.register(gameContext);
        GameConsole.writeSeparator();
        GameConsole.writeLine(gameContext.getGameTitle());
        GameConsole.writeSeparator();

        Deck deck = gameContext.getDeck();
        DiscardPile discardPile = initializeDiscardPile(deck);

        Players players = gameContext.getPlayers();
        Player currentPlayer = players.getCurrentPlayer();

        executeGameLoop(deck, discardPile, currentPlayer, players);

        showWinners(players);
    }

    private static void showWinners(Players players) {
        GameConsole.writeLine();

        List<Player> winners = players.getPlayersWithLeastCards();
        if (winners.size() == 1) {
            Player winner = winners.get(0);
            GameConsole.writeLine("***** " + winner.getName() + " wins the game! *****");
            return;
        }

        String tieWinnerNames = winners.stream()
                .map(Player::getName)
                .reduce((left, right) -> left + ", " + right)
                .orElse("");
        GameConsole.writeLine("***** It's a tie between: " + tieWinnerNames + "! *****");
    }

    private void executeGameLoop(Deck deck, DiscardPile discardPile, Player currentPlayer, Players players) {
        while (!deck.isEmpty() && players.getSmallestHandCardCount() > 0) {
            gameContext.incrementTurn();
            TurnContext turnContext = getTurnContext(gameContext, discardPile, currentPlayer);
            GameConsole.writeLine();
            TurnAction.showTurn(gameContext, turnContext);
            currentPlayer.takeTurn(turnContext);
            currentPlayer = players.moveToNextPlayer();
        }
    }

    private static TurnContext getTurnContext(GameContext gameContext, DiscardPile discardPile, Player currentPlayer) {
        Random rng = gameContext.getRandomNumberGenerator();
        SuitType currentSuit = discardPile.getActiveSuit();
        Card topCard = discardPile.getTopCard();

        return new TurnContext(gameContext, rng, currentPlayer, currentSuit, topCard);
    }

    private DiscardPile initializeDiscardPile(Deck deck) {
        DiscardPile discardPile = gameContext.getDiscardPile();
        discardPile.addCard(deck.drawCard());

        Card topCard = discardPile.getTopCard();
        TurnAction.showTopCard(topCard, topCard.getSuit(), "Starting discard: ");
        return discardPile;
    }
}
