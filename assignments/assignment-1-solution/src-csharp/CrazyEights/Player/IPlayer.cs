using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;
using System.Collections.Generic;

namespace CrazyEights.Player;

/// <summary>
/// Represents a player in the Crazy Eights game.
/// Defines properties and methods required for player interactions,
/// such as managing the player's hand, selecting cards, and determining actions during a turn.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Gets the display name for the player.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the number of cards currently in the player's hand.
    /// </summary>
    public int CardCount { get; }

    /// <summary>
    /// Gets a value indicating whether this player's hand should be shown.
    /// </summary>
    public bool ShowHand { get; }

    /// <summary>
    /// Adds a card to the player's hand.
    /// </summary>
    /// <param name="card">The card to add.</param>
    public void AddCard(ICard card);

    /// <summary>
    /// Removes a card from the player's hand.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    public void RemoveCard(ICard card);

    /// <summary>
    /// Returns a read-only view of the player's current hand.
    /// </summary>
    /// <returns>The cards currently in the player's hand.</returns>
    public IReadOnlyList<ICard> PeekHand();

    /// <summary>
    /// Executes the player's turn actions.
    /// </summary>
    /// <param name="context">The context for the current turn.</param>
    public void TakeTurn(TurnContext context);

    /// <summary>
    /// Selects a card to play for the current turn.
    /// </summary>
    /// <param name="context">The context for the current turn.</param>
    /// <returns>The selected card, or an unselectable card if none is chosen.</returns>
    public ICard SelectCard(TurnContext context);

    /// <summary>
    /// Selects a suit when a wildcard card is played.
    /// </summary>
    /// <param name="gameContext">The overall game context.</param>
    /// <param name="turnContext">The current turn context.</param>
    /// <returns>The suit the player wants to set.</returns>
    public SuitType SelectSuit(GameContext gameContext, TurnContext turnContext);

    /// <summary>
    /// Determines whether the player will immediately play a drawn card.
    /// </summary>
    /// <param name="turnContext">The current turn context.</param>
    /// <param name="drawnCard">The card that was drawn.</param>
    /// <returns><c>true</c> if the player will play the drawn card; otherwise, <c>false</c>.</returns>
    public bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard);
}
