using CrazyEights.Cards;

namespace CrazyEights.Domain;

/// <summary>
/// Wraps a card with the reason it is playable.
/// </summary>
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
