using CrazyEights.Cards;
using CrazyEights.Domain;

namespace CrazyEights.Player;

/// <summary>
/// Represents a player's hand of cards.
/// </summary>
public class Hand
{
    private readonly List<ICard> _cards = new();

    public int CardCount => _cards.Count;

    public IReadOnlyList<ICard> CardList => _cards.AsReadOnly();

    public void AddCard(ICard card)
    {
        _cards.Add(card);
    }

    public void RemoveCard(ICard card)
    {
        if (!_cards.Contains(card))
            throw new InvalidOperationException("Card not found in hand.");
        _cards.Remove(card);
    }

    public IReadOnlyList<PlayableCard> GetPlayableCards(SuitType currentSuit, RankType currentRank)
    {
        return PlayableCardsSelector.Get(CardList, currentSuit, currentRank);
    }
}
