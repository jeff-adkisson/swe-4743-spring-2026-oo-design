using CrazyEights.Cards;
using CrazyEights.Domain;

namespace CrazyEights.CardDeck;

public static class DeckInitializer
{
    public static Deck CreateCardDeck(ProgramContext programContext)
    {
        var cards = CreateOrderedCardList();
        var shuffledCards = Shuffle(programContext, cards);
        return new Deck(shuffledCards);
    }

    private static List<ICard> CreateOrderedCardList()
    {
        var cards = new List<ICard>();
        foreach (var suit in Suit.GetSuits())
        foreach (var rank in Rank.GetRanks())
            cards.Add(new Card(rank, suit));

        return cards;
    }

    private static List<ICard> Shuffle(ProgramContext programContext, List<ICard> cards)
    {
        var random = programContext.RandomNumberGenerator;
        var shuffledCards = cards.OrderBy(c => random.Next()).ToList();
        return shuffledCards;
    }
}