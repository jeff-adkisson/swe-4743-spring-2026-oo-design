using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;
using System.Collections.Generic;

namespace CrazyEights.Player;

public interface IPlayer
{
    public string Name { get; }

    public int CardCount { get; }

    public bool ShowHand { get; }

    public void AddCard(ICard card);

    public void RemoveCard(ICard card);

    public IReadOnlyList<ICard> PeekHand();

    public void TakeTurn(TurnContext context);

    public ICard SelectCard(TurnContext context);

    public SuitType SelectSuit(GameContext gameContext, TurnContext turnContext);

    public bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard);
}
