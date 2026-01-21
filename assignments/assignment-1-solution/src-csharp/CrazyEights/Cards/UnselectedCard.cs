using CrazyEights.Domain;

namespace CrazyEights.Cards;

/// <summary>
/// Null-object card used when no card is selected.
/// </summary>
public class UnselectedCard : ICard
{
    public static readonly UnselectedCard Instance = new();

    public RankType Rank => RankType.NotSet;

    public SuitType Suit => SuitType.NotSet;

    public int CardId => -1;

    public string GetDescription()
    {
        return "Unselected Card";
    }

    public bool IsSelectable => false;
}
