using CrazyEights.Cards;
using CrazyEights.Domain;

namespace CrazyEights.CardDeck;

/// <summary>
/// Represents the discard pile and active suit state.
/// </summary>
public class DiscardPile
{
    private readonly Stack<ICard> _discardedCards = [];

    public ICard TopCard
    {
        get
        {
            if (_discardedCards.Count == 0)
                throw new InvalidOperationException("Discard pile is empty.");

            return _discardedCards.Peek();
        }
    }

    public SuitType ActiveSuit => OverriddenSuit == SuitType.NotSet ? TopCard.Suit : OverriddenSuit;

    private SuitType OverriddenSuit { get; set; } = SuitType.NotSet;

    public void AddCard(ICard card)
    {
        _discardedCards.Push(card);
        SetTopCardAsActiveSuit();
    }

    private void SetTopCardAsActiveSuit()
    {
        OverriddenSuit = SuitType.NotSet;
    }

    public void OverrideTopCardSuit(SuitType newSuit)
    {
        if (TopCard.Suit == newSuit)
        {
            SetTopCardAsActiveSuit();
            return;
        }

        OverriddenSuit = newSuit;
    }
}
