package edu.kennesaw.crazy8s.game;

import edu.kennesaw.crazy8s.carddeck.Deck;
import edu.kennesaw.crazy8s.carddeck.DiscardPile;
import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.cards.UnselectedCard;
import edu.kennesaw.crazy8s.domain.IsPlayable;
import edu.kennesaw.crazy8s.domain.PlayableCard;
import edu.kennesaw.crazy8s.domain.PlayableCardsSelector;
import edu.kennesaw.crazy8s.domain.Rank;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.player.Player;
import edu.kennesaw.crazy8s.player.Players;
import java.util.List;

/**
 * Implements turn execution steps and game UI prompts.
 */
public final class TurnAction {
    private TurnAction() {
    }

    public static void showTurn(GameContext gameContext, TurnContext turnContext) {
        int turnCount = gameContext.getTurnNumber();
        Players players = gameContext.getPlayers();

        GameConsole.writeLine(String.format("----- TURN %d -----", turnCount));
        showTopCard(turnContext.getTopCard(), turnContext.getCurrentSuit());
        showRemainingDeckCount(gameContext.getDeck());
        showCardCounts(players);

        showMessage(turnContext.getCurrentPlayer().getName().toUpperCase() + "'s turn", true);
    }

    public static void startTurn(GameContext gameContext, TurnContext turnContext) {
        Player player = turnContext.getCurrentPlayer();

        if (player.isShowHand()) {
            showAllCardsInHand(turnContext);
        }

        selectCard(gameContext, turnContext);
    }

    private static Card drawCard(GameContext gameContext, TurnContext turnContext) {
        Player player = turnContext.getCurrentPlayer();
        String name = player.getName();
        showMessage(name + " has no playable cards. Drawing one card...", true);

        Card drawnCard = gameContext.getDeck().drawCard();
        player.addCard(drawnCard);

        String cardDescription = player.isShowHand() ? drawnCard.getDescription() : "a card";
        showMessage(name + " drew " + cardDescription);

        IsPlayable canPlayDrawnCard = PlayableCardsSelector.canPlayCard(
                drawnCard,
                turnContext.getCurrentSuit(),
                turnContext.getCurrentRank());
        if (canPlayDrawnCard.canPlay() && player.willPlayDrawnCard(turnContext, drawnCard)) {
            return drawnCard;
        }

        GameConsole.readLine("Press Enter to continue...");
        return UnselectedCard.getInstance();
    }

    private static void selectCard(GameContext gameContext, TurnContext turnContext) {
        Player currentPlayer = turnContext.getCurrentPlayer();
        Card selectedCard = currentPlayer.selectCard(turnContext);

        if (!selectedCard.isSelectable()) {
            Card drawnCard = drawCard(gameContext, turnContext);
            if (!drawnCard.isSelectable()) {
                return;
            }
            selectedCard = drawnCard;
        }

        currentPlayer.removeCard(selectedCard);
        gameContext.getDiscardPile().addCard(selectedCard);
        String name = turnContext.getCurrentPlayer().getName();
        showMessage(name + " selected " + selectedCard.getDescription());

        if (Rank.isWildcardRank(selectedCard.getRank())) {
            chooseSuit(gameContext, turnContext);
        }
    }

    private static void chooseSuit(GameContext gameContext, TurnContext turnContext) {
        Player currentPlayer = turnContext.getCurrentPlayer();
        SuitType chosenSuit = currentPlayer.selectSuit(gameContext, turnContext);
        String name = turnContext.getCurrentPlayer().getName();

        DiscardPile discardPile = gameContext.getDiscardPile();
        if (discardPile.getActiveSuit() != chosenSuit) {
            discardPile.overrideTopCardSuit(chosenSuit);
            showMessage(name + " changed suit to " + chosenSuit);
            return;
        }

        showMessage(name + " left the suit as " + chosenSuit);
    }

    private static void showMessage(String action) {
        showMessage(action, false);
    }

    private static void showMessage(String action, boolean showBlankLineBefore) {
        if (showBlankLineBefore) {
            GameConsole.writeLine();
        }
        GameConsole.writeLine("** " + action);
    }

    private static void showAllCardsInHand(TurnContext turnContext) {
        String name = turnContext.getCurrentPlayer().getName();
        List<Card> hand = turnContext.getCurrentPlayer().peekHand();

        GameConsole.writeLine();
        GameConsole.writeLine(name + "'s hand");

        for (Card card : hand) {
            GameConsole.writeLine("  - " + card.getDescription());
        }
    }

    private static void showRemainingDeckCount(Deck deck) {
        int cardCount = deck.getCardCount();
        GameConsole.writeLine("Deck remaining: " + cardCount + " " + getPluralCardLabel(cardCount));
    }

    public static void showTopCard(Card topCard, SuitType currentSuit) {
        showTopCard(topCard, currentSuit, "Top discard: ");
    }

    public static void showTopCard(Card topCard, SuitType currentSuit, String prefix) {
        boolean isSuitChanged = topCard.getSuit() != currentSuit;
        String suitToMatch = isSuitChanged
                ? " (Suit to match: " + currentSuit + ")"
                : "";

        GameConsole.writeLine(prefix + topCard.getDescription() + suitToMatch);
    }

    private static void showCardCounts(Players players) {
        List<Player> playerList = players.getList();
        for (int i = 0; i < playerList.size(); i++) {
            if (i > 0) {
                GameConsole.write(" | ");
            }
            Player player = playerList.get(i);
            String name = player.getName();
            int cardCount = player.getCardCount();
            GameConsole.write(name + ": " + cardCount + " " + getPluralCardLabel(cardCount));
        }

        GameConsole.writeLine();
    }

    private static String getPluralCardLabel(int count) {
        return count == 1 ? "card" : "cards";
    }
}
