package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.cards.UnselectedCard;
import edu.kennesaw.crazy8s.domain.PlayableCard;
import edu.kennesaw.crazy8s.domain.Suit;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.game.GameConsole;
import edu.kennesaw.crazy8s.game.GameContext;
import edu.kennesaw.crazy8s.game.TurnContext;
import java.util.List;

/**
 * Human-controlled player that chooses cards via console input.
 */
public class HumanPlayer extends PlayerBase {
    public static final String DEFAULT_NAME = "Player";

    private static final boolean SHOW_MY_HAND = true;

    public HumanPlayer(String name, Hand hand) {
        super(name, hand, SHOW_MY_HAND);
    }

    @Override
    public Card selectCard(TurnContext turnContext) {
        List<PlayableCard> playableCards =
                getHand().getPlayableCards(turnContext.getCurrentSuit(), turnContext.getCurrentRank());

        if (playableCards.isEmpty()) {
            return UnselectedCard.getInstance();
        }

        showPlayableCardsInHand(turnContext, playableCards);

        boolean isValidInput = false;
        Card selectedCard = UnselectedCard.getInstance();
        while (!isValidInput) {
            String input = GameConsole.readLine("Choose a card number to play: ");
            Integer selectedNumber = tryParseInt(input);
            if (selectedNumber == null || selectedNumber < 1 || selectedNumber > playableCards.size()) {
                GameConsole.writeLine("- Invalid choice! Please try again.");
                continue;
            }

            selectedCard = playableCards.get(selectedNumber - 1).getCard();
            isValidInput = true;
        }

        return selectedCard;
    }

    private static void showPlayableCardsInHand(TurnContext turnContext, List<PlayableCard> playableCards) {
        Player currentPlayer = turnContext.getCurrentPlayer();
        String name = currentPlayer.getName();

        GameConsole.writeLine();
        GameConsole.writeLine(name + "'s playable cards");

        for (int i = 0; i < playableCards.size(); i++) {
            PlayableCard playableCard = playableCards.get(i);
            Card card = playableCard.getCard();
            int selector = i + 1;
            String cardDescription = card.getDescription();
            String playableReason = playableCard.getPlayableReason();
            GameConsole.writeLine(String.format("  [%d] %s (%s)", selector, cardDescription, playableReason));
        }
    }

    @Override
    public SuitType selectSuit(GameContext gameContext, TurnContext turnContext) {
        SuitType currentSuit = gameContext.getDiscardPile().getActiveSuit();
        List<SuitType> suits = Suit.getSuits();
        String name = turnContext.getCurrentPlayer().getName();

        boolean isValidInput = false;
        SuitType selectedSuit = SuitType.NOT_SET;

        while (!isValidInput) {
            GameConsole.writeLine();
            GameConsole.writeLine(name + ", you played a wildcard! Choose a suit:");
            for (SuitType suit : suits) {
                String suitName = Suit.getSuitName(suit);
                String letter = suitName.substring(0, 1).toUpperCase();
                String isCurrentSuitNote = currentSuit == suit ? " (current suit)" : "";
                GameConsole.writeLine(String.format("  [%s] %s%s", letter, suit, isCurrentSuitNote));
            }

            String input = GameConsole.readLine("Enter the letter of your chosen suit: ");
            if (input == null || input.length() != 1) {
                GameConsole.writeLine("- Invalid choice! Please try again.");
                continue;
            }

            char selectedLetter = Character.toUpperCase(input.charAt(0));
            switch (selectedLetter) {
                case 'H':
                    selectedSuit = SuitType.HEARTS;
                    break;
                case 'D':
                    selectedSuit = SuitType.DIAMONDS;
                    break;
                case 'C':
                    selectedSuit = SuitType.CLUBS;
                    break;
                case 'S':
                    selectedSuit = SuitType.SPADES;
                    break;
                default:
                    GameConsole.writeLine("- Invalid choice! Please try again.");
                    continue;
            }

            isValidInput = true;
        }

        return selectedSuit;
    }

    @Override
    public boolean willPlayDrawnCard(TurnContext turnContext, Card drawnCard) {
        GameConsole.writeLine();
        return GameConsole.promptYesNo("You drew a playable card. Play it now (Y/N): ");
    }

    private static Integer tryParseInt(String value) {
        try {
            return Integer.parseInt(value);
        } catch (NumberFormatException ex) {
            return null;
        }
    }
}
