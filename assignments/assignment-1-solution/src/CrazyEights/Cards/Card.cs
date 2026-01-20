using CrazyEights.Domain;

namespace CrazyEights.Cards;

public class Card : ICard
{
    public Card(RankType rankType, SuitType suitType)
    {
        Rank = rankType;
        Suit = suitType;
    }

    public string GetDescription()
    {
        var rankName = Domain.Rank.GetRankName(Rank);
        var suitName = Domain.Suit.GetSuitName(Suit);
        return $"{rankName} of {suitName}";
    }

    public bool IsSelectable => true;

    public RankType Rank { get; }

    public SuitType Suit { get; }

    public int CardId => (int)Suit * 100 + (int)Rank;

    public override string ToString()
    {
        return GetDescription();
    }
}