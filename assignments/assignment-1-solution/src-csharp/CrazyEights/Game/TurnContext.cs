using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Player;

namespace CrazyEights.Game;

/// <summary>
/// Captures the state needed for a single player's turn.
/// </summary>
public class TurnContext
{
    public TurnContext(
        GameContext gameContext,
        Random randomNumberGenerator,
        IPlayer currentPlayer,
        SuitType currentSuit,
        ICard topCard)
    {
        GameContext = gameContext;
        RandomNumberGenerator = randomNumberGenerator;
        CurrentPlayer = currentPlayer;
        CurrentSuit = currentSuit;
        TopCard = topCard;
        CurrentRank = TopCard.Rank;
    }

    public GameContext GameContext { get; }

    public Random RandomNumberGenerator { get; set; }

    public IPlayer CurrentPlayer { get; }

    public SuitType CurrentSuit { get; }

    public ICard TopCard { get; }

    public RankType CurrentRank { get; }
}
