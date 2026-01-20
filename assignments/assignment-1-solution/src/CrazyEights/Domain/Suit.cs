using System.Collections.Immutable;

namespace CrazyEights.Domain;

/// <summary>
/// Provides helpers for suit names, symbols, and lists.
/// </summary>
public static class Suit
{
    public static string GetSuitSymbol(SuitType suit)
    {
        return suit switch
        {
            SuitType.Hearts => "♥",
            SuitType.Diamonds => "♦",
            SuitType.Clubs => "♣",
            SuitType.Spades => "♠",
            _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
        };
    }

    public static string GetSuitName(SuitType suit)
    {
        return $"{suit} {GetSuitSymbol(suit)}";
    }

    public static IReadOnlyList<SuitType> GetSuits()
    {
        return Enum.GetValues<SuitType>()
            .Where(suit => suit != SuitType.NotSet)
            .ToImmutableList();
    }
}
