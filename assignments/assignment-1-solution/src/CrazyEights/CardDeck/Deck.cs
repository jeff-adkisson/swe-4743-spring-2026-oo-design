using CrazyEights.Cards;

namespace CrazyEights.CardDeck;

public class Deck
{
    private readonly Stack<ICard> _cards;

    public Deck(IEnumerable<ICard> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);
        _cards = new Stack<ICard>(cards);
    }

    public int CardCount => _cards.Count;

    public bool IsEmpty => _cards.Count == 0;

    public ICard DrawCard()
    {
        if (IsEmpty) throw new InvalidOperationException("Deck is empty.");

        return _cards.Pop();
    }
}