using CrazyEights.Domain;

namespace CrazyEights.Cards;

/// <summary>
/// Defines the core properties and behaviors of a card.
/// </summary>
public interface ICard
{
    /// <summary>
    /// Gets the rank of the card.
    /// </summary>
    public RankType Rank { get; }

    /// <summary>
    /// Gets the suit of the card.
    /// </summary>
    public SuitType Suit { get; }

    /// <summary>
    /// Gets a unique identifier for the card.
    /// </summary>
    public int CardId { get; }

    /// <summary>
    /// Gets a value indicating whether the card can be selected to play.
    /// </summary>
    public bool IsSelectable { get; }

    /// <summary>
    /// Returns a human-readable description of the card.
    /// </summary>
    /// <returns>The card description.</returns>
    public string GetDescription();
}
