using CrazyEights.Domain;

namespace CrazyEights.Cards;

public interface ICard
{
    public RankType Rank { get; }

    public SuitType Suit { get; }

    public int CardId { get; }

    public bool IsSelectable { get; }

    public string GetDescription();
}