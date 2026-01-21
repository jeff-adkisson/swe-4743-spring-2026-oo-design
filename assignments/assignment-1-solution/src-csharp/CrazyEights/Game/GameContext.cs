using CrazyEights.CardDeck;
using CrazyEights.Player;

namespace CrazyEights.Game;

/// <summary>
/// Holds shared game state and configuration values.
/// </summary>
public class GameContext
{
    private readonly ProgramContext _programContext;

    public GameContext(ProgramContext programContext, Deck deck, DiscardPile discardPile, Players players)
    {
        _programContext = programContext;
        Deck = deck;
        DiscardPile = discardPile;
        Players = players;
        ShowAllHands = programContext.ShowAllHands;
    }

    public Random RandomNumberGenerator => _programContext.RandomNumberGenerator;

    public bool ShowAllHands { get; }

    public string GameTitle { get; } = "Crazy Eights (Simplified)";

    public Deck Deck { get; }

    public DiscardPile DiscardPile { get; }

    public Players Players { get; }

    public int HandSize => _programContext.HandSize;

    public int TurnNumber { get; private set; }

    public int IncrementTurn()
    {
        return TurnNumber += 1;
    }
}
