package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.cards.Card;
import edu.kennesaw.crazy8s.domain.SuitType;
import edu.kennesaw.crazy8s.game.GameContext;
import edu.kennesaw.crazy8s.game.TurnAction;
import edu.kennesaw.crazy8s.game.TurnContext;
import java.util.List;

/**
 * Base class that implements shared player behavior and hand management.
 */
public abstract class PlayerBase implements Player {
    private final String name;
    private final Hand hand;
    private final boolean showHand;

    protected PlayerBase(String name, Hand hand, boolean showHand) {
        this.name = name;
        this.hand = hand;
        this.showHand = showHand;
    }

    @Override
    public String getName() {
        return name;
    }

    protected Hand getHand() {
        return hand;
    }

    @Override
    public int getCardCount() {
        return hand.getCardCount();
    }

    @Override
    public boolean isShowHand() {
        return showHand;
    }

    @Override
    public void addCard(Card card) {
        hand.addCard(card);
    }

    @Override
    public void removeCard(Card card) {
        hand.removeCard(card);
    }

    @Override
    public List<Card> peekHand() {
        return hand.getCardList();
    }

    @Override
    public void takeTurn(TurnContext context) {
        TurnAction.startTurn(context.getGameContext(), context);
    }

    @Override
    public abstract Card selectCard(TurnContext context);

    @Override
    public abstract SuitType selectSuit(GameContext gameContext, TurnContext turnContext);

    @Override
    public abstract boolean willPlayDrawnCard(TurnContext turnContext, Card drawnCard);
}
