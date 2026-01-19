using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;

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

    public Hand Hand { get; }

    public int CardCount => Hand.CardCount;

    public bool ShowHand { get; }

    public abstract ICard SelectCard(TurnContext context);

    public abstract SuitType SelectSuit(GameContext gameContext, TurnContext turnContext);
}