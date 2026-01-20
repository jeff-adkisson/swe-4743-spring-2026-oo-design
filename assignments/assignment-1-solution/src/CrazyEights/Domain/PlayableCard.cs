using CrazyEights.Cards;

namespace CrazyEights.Domain;

public class PlayableCard
{
    public PlayableCard(ICard card, string playableReason)
    {
        Card = card;
        PlayableReason = playableReason;
    }

    public ICard Card { get; }

    public string PlayableReason { get; }
}