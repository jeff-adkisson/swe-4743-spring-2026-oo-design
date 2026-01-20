using System.Collections.Immutable;

namespace CrazyEights.Player;

public class Players
{
    private readonly List<IPlayer> _players = [];
    private int _currentPlayerIndex;

    public IReadOnlyList<IPlayer> List => _players.AsReadOnly();

    public IPlayer CurrentPlayer
    {
        get
        {
            if (_players.Count == 0)
                throw new InvalidOperationException("No players are registered.");

            return _players[_currentPlayerIndex];
        }
    }

    public void Add(IPlayer player)
    {
        _players.Add(player);
    }

    public IPlayer MoveToNextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        return CurrentPlayer;
    }

    public int GetSmallestHandCardCount()
    {
        return _players.Min(p => p.Hand.CardCount);
    }

    public IReadOnlyList<IPlayer> GetPlayersWithLeastCards()
    {
        var smallestHandCount = GetSmallestHandCardCount();
        return _players
            .Where(p => p.Hand.CardCount == smallestHandCount)
            .OrderBy(player => player.Name)
            .ToImmutableList();
    }
}