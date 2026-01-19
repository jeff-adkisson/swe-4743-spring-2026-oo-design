using CrazyEights.Cards;

namespace CrazyEights.Player;

public class PlayableCard
{
    public PlayableCard(ICard card, int selector, string playableReason)
    {
        Card = card;
        Selector = selector;
        PlayableReason = playableReason;
    }

    public ICard Card { get; }

    public int Selector { get; }

    public string PlayableReason { get; }
}