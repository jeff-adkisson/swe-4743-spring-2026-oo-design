using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;

namespace CrazyEights.Player;

public interface IPlayer
{
    public string Name { get; }

    public Hand Hand { get; }

    public int CardCount { get; }

    public bool ShowHand { get; }

    public ICard SelectCard(TurnContext context);

    public SuitType SelectSuit(GameContext gameContext, TurnContext turnContext);
}