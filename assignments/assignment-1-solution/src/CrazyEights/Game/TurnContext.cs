using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Player;

namespace CrazyEights.Game;

public class TurnContext
{
    public TurnContext(
        Random randomNumberGenerator,
        IPlayer currentPlayer,
        SuitType currentSuit,
        ICard topCard)
    {
        RandomNumberGenerator = randomNumberGenerator;
        CurrentPlayer = currentPlayer;
        CurrentSuit = currentSuit;
        TopCard = topCard;
        CurrentRank = TopCard.Rank;
    }

    public Random RandomNumberGenerator { get; set; }

    public IPlayer CurrentPlayer { get; }

    public Hand Hand => CurrentPlayer.Hand;

    public SuitType CurrentSuit { get; }

    public ICard TopCard { get; }

    public RankType CurrentRank { get; }
}