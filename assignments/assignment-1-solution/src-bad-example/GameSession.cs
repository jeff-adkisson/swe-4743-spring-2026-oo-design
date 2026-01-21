namespace CrazyEightsBadExample;

/// <summary>
/// Runs a full game session and keeps all game state.
/// </summary>
internal class GameSession
{
    private readonly List<Card> _deck = new();
    private readonly List<Card> _discard = new();
    private readonly int _handSize;
    private readonly List<object> _players = new();
    private readonly Random _r;
    private readonly bool _showAllHands;
    private string _currentRank = "";
    private string _currentSuit = "";
    private bool _gameOver;
    private int _turnIndex;

    public GameSession(int seed, int handSize, bool showAllHands)
    {
        _r = seed == 0 ? new Random() : new Random(seed);
        _handSize = handSize;
        _showAllHands = showAllHands;
        _players.Add(new HumanPlayer { Name = "You", Id = 1 });
        _players.Add(new CpuPlayer { Name = "CPU-1", Id = 2 });
    }

    public string RunSession()
    {
        MakeDeck();
        ShuffleDeck();
        Deal();
        StartDiscard();
        _gameOver = false;
        _turnIndex = 0;

        while (!_gameOver)
        {
            if (_turnIndex >= _players.Count) _turnIndex = 0;
            var p = _players[_turnIndex];

            Console.Clear();
            Console.WriteLine("Top Card: " + _currentRank + " of " + _currentSuit);
            Console.WriteLine("Deck: " + _deck.Count + " cards");
            Console.WriteLine("Discard: " + _discard.Count + " cards");
            Console.WriteLine();
            foreach (var other in _players)
                if (other is PlayerData od)
                {
                    if (_showAllHands || other == p)
                    {
                        Console.WriteLine(od.Name + " hand (" + od.Hand.Count + "):");
                        for (var i = 0; i < od.Hand.Count; i++)
                            Console.WriteLine("  [" + i + "] " + od.Hand[i].Rank + " of " + od.Hand[i].Suit);
                    }
                    else
                    {
                        Console.WriteLine(od.Name + " hand: " + od.Hand.Count + " cards");
                    }
                }

            Console.WriteLine();
            if (p is HumanPlayer hp)
            {
                Console.WriteLine("Your turn!");
                var played = false;
                while (!played)
                {
                    var playableIndexes = new List<int>();
                    for (var i = 0; i < hp.Hand.Count; i++)
                    {
                        var c = hp.Hand[i];
                        if (c is WildCard || c.Rank == _currentRank || c.Suit == _currentSuit) playableIndexes.Add(i);
                    }

                    Console.Write("Choose index to play or D to draw: ");
                    var input = Console.ReadLine();
                    if (input == null)
                    {
                        Console.WriteLine("No input. Drawing.");
                        DrawOne(hp);
                        played = true;
                    }
                    else
                    {
                        var trimmed = input.Trim().ToLowerInvariant();
                        if (trimmed == "d")
                        {
                            DrawOne(hp);
                            if (hp.Hand.Count > 0)
                            {
                                var last = hp.Hand[hp.Hand.Count - 1];
                                if (last is WildCard || last.Rank == _currentRank || last.Suit == _currentSuit)
                                {
                                    Console.Write("Play the drawn card? (y/n): ");
                                    var pInput = Console.ReadLine();
                                    if (pInput != null && (pInput.Trim().ToLowerInvariant() == "y" ||
                                                           pInput.Trim().ToLowerInvariant() == "yes"))
                                        PlayCard(hp, hp.Hand.Count - 1);
                                }
                            }

                            played = true;
                        }
                        else
                        {
                            if (int.TryParse(trimmed, out var idx))
                            {
                                if (idx >= 0 && idx < hp.Hand.Count)
                                {
                                    if (playableIndexes.Contains(idx))
                                    {
                                        PlayCard(hp, idx);
                                        played = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Not playable. Try again.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Out of range.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Bad input.");
                            }
                        }
                    }
                }
            }
            else if (p is CpuPlayer cpu)
            {
                Console.WriteLine(cpu.Name + " turn!");
                var playableIndexes = new List<int>();
                for (var i = 0; i < cpu.Hand.Count; i++)
                {
                    var c = cpu.Hand[i];
                    if (c is WildCard || c.Rank == _currentRank || c.Suit == _currentSuit) playableIndexes.Add(i);
                }

                if (playableIndexes.Count > 0)
                {
                    var choice = playableIndexes[_r.Next(playableIndexes.Count)];
                    PlayCard(cpu, choice);
                }
                else
                {
                    DrawOne(cpu);
                    if (cpu.Hand.Count > 0)
                    {
                        var last = cpu.Hand[cpu.Hand.Count - 1];
                        if (last is WildCard || last.Rank == _currentRank || last.Suit == _currentSuit)
                            PlayCard(cpu, cpu.Hand.Count - 1);
                    }
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Unknown player type. Skipping.");
            }

            if (_gameOver) break;

            if (_deck.Count == 0)
            {
                var least = int.MaxValue;
                var bestName = "Nobody";
                foreach (var player in _players)
                    if (player is PlayerData pd)
                    {
                        if (pd.Hand.Count < least)
                        {
                            least = pd.Hand.Count;
                            bestName = pd.Name;
                        }
                        else if (pd.Hand.Count == least)
                        {
                            bestName = bestName + " and " + pd.Name;
                        }
                    }

                _gameOver = true;
                return bestName;
            }

            _turnIndex++;
        }

        return "Nobody";
    }

    private void MakeDeck()
    {
        _deck.Clear();
        var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
        var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        for (var s = 0; s < suits.Length; s++)
        for (var r = 0; r < ranks.Length; r++)
            if (ranks[r] == "8")
                _deck.Add(new WildCard { Suit = suits[s], Rank = ranks[r] });
            else
                _deck.Add(new Card { Suit = suits[s], Rank = ranks[r] });
    }

    private void ShuffleDeck()
    {
        for (var i = 0; i < _deck.Count; i++)
        {
            var j = _r.Next(_deck.Count);
            var tmp = _deck[i];
            _deck[i] = _deck[j];
            _deck[j] = tmp;
        }
    }

    private void Deal()
    {
        foreach (var p in _players)
            if (p is PlayerData pd)
                pd.Hand.Clear();

        for (var i = 0; i < _handSize; i++)
            foreach (var p in _players)
                if (p is PlayerData pd && _deck.Count > 0)
                {
                    pd.Hand.Add(_deck[0]);
                    _deck.RemoveAt(0);
                }
    }

    private void StartDiscard()
    {
        while (_deck.Count > 0)
        {
            var c = _deck[0];
            _deck.RemoveAt(0);
            _discard.Add(c);
            if (c is WildCard) continue;
            _currentSuit = c.Suit;
            _currentRank = c.Rank;
            return;
        }

        _currentSuit = "Hearts";
        _currentRank = "A";
    }

    private void DrawOne(PlayerData pd)
    {
        if (_deck.Count == 0) return;
        pd.Hand.Add(_deck[0]);
        _deck.RemoveAt(0);
    }

    private void PlayCard(PlayerData pd, int index)
    {
        if (index < 0 || index >= pd.Hand.Count) return;
        var c = pd.Hand[index];
        pd.Hand.RemoveAt(index);
        _discard.Add(c);

        if (c is WildCard)
        {
            if (pd is HumanPlayer)
            {
                Console.Write("Choose suit (H/D/C/S): ");
                var input = Console.ReadLine();
                if (input != null)
                {
                    var t = input.Trim().ToUpperInvariant();
                    if (t == "H") _currentSuit = "Hearts";
                    else if (t == "D") _currentSuit = "Diamonds";
                    else if (t == "C") _currentSuit = "Clubs";
                    else if (t == "S") _currentSuit = "Spades";
                    else _currentSuit = "Hearts";
                }
                else
                {
                    _currentSuit = "Hearts";
                }
            }
            else
            {
                var r = _r.Next(4);
                if (r == 0) _currentSuit = "Hearts";
                else if (r == 1) _currentSuit = "Diamonds";
                else if (r == 2) _currentSuit = "Clubs";
                else _currentSuit = "Spades";
            }

            _currentRank = "8";
        }
        else
        {
            _currentSuit = c.Suit;
            _currentRank = c.Rank;
        }

        if (pd.Hand.Count == 0) _gameOver = true;
    }
}
