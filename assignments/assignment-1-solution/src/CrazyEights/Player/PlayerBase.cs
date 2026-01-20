using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;
using System.Collections.Generic;

namespace CrazyEights.Player;

public abstract class PlayerBase : IPlayer
{
    protected PlayerBase(string name, Hand hand, bool showHand)
    {
        Name = name;
        Hand = hand;
        ShowHand = showHand;
    }

    public string Name { get; }

    protected Hand Hand { get; }

    public int CardCount => Hand.CardCount;

    public bool ShowHand { get; }

    public void AddCard(ICard card)
    {
        Hand.AddCard(card);
    }

    public void RemoveCard(ICard card)
    {
        Hand.RemoveCard(card);
    }

    public IReadOnlyList<ICard> PeekHand()
    {
        return Hand.CardList;
    }

    public void TakeTurn(TurnContext context)
    {
        TurnAction.StartTurn(context.GameContext, context);
    }

    public abstract ICard SelectCard(TurnContext context);

    public abstract SuitType SelectSuit(GameContext gameContext, TurnContext turnContext);

    public abstract bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard);
}
