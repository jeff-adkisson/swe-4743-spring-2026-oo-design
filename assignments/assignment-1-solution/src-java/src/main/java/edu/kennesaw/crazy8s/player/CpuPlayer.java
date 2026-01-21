package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.cards.UnselectedCard;
import edu.kennesaw.crazy8s.domain.PlayableCard;
import edu.kennesaw.crazy8s.domain.Suit;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.game.GameContext;
import edu.kennesaw.crazy8s.game.TurnContext;
import java.util.List;
import java.util.Random;

/**
 * CPU-controlled player that makes random selections.
 */
public class CpuPlayer extends PlayerBase {
    private static final String CPU_PLAYER_NAME = "CPU";

    public CpuPlayer(Hand hand, boolean showHand) {
        super(CPU_PLAYER_NAME, hand, showHand);
    }

    @Override
    public Card selectCard(TurnContext turnContext) {
        Random rng = turnContext.getRandomNumberGenerator();
        List<PlayableCard> playableCards =
                getHand().getPlayableCards(turnContext.getCurrentSuit(), turnContext.getCurrentRank());

        if (playableCards.isEmpty()) {
            return UnselectedCard.getInstance();
        }

        int randomCardIndex = rng.nextInt(playableCards.size());
        return playableCards.get(randomCardIndex).getCard();
    }

    @Override
    public SuitType selectSuit(GameContext gameContext, TurnContext turnContext) {
        Random rng = turnContext.getRandomNumberGenerator();
        List<SuitType> suitValues = Suit.getSuits();
        int randomSuitIndex = rng.nextInt(suitValues.size());
        return suitValues.get(randomSuitIndex);
    }

    @Override
    public boolean willPlayDrawnCard(TurnContext turnContext, Card drawnCard) {
        Random rng = turnContext.getRandomNumberGenerator();
        return rng.nextDouble() > 0.5;
    }
}
