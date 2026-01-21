namespace CrazyEightsBadExample;

/// <summary>
/// Entry point that handles startup and the replay loop.
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        var seed = 0;
        var handSize = 5;
        var showAllHands = false;
        if (args.Length > 0)
        {
            var ok = int.TryParse(args[0], out seed);
            if (!ok) seed = 0;
        }

        if (args.Length > 1)
        {
            var ok = int.TryParse(args[1], out handSize);
            if (!ok || handSize <= 0) handSize = 5;
        }

        if (args.Length > 2)
        {
            var ok = bool.TryParse(args[2], out showAllHands);
            if (!ok) showAllHands = false;
        }

        Console.Clear();
        Console.WriteLine("=== Welcome to *Simplified* Crazy Eights (Bad Example Edition) ===");
        Console.WriteLine("Rules:");
        Console.WriteLine("- Play a card that matches rank or suit of the top discard.");
        Console.WriteLine("- Eights are wild and can be played anytime.");
        Console.WriteLine("- If you cannot play, you must draw one card.");
        Console.WriteLine("- If the deck runs out, player with fewest cards wins.");
        Console.WriteLine("- The CPU is random and likely bad.");
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();

        var keepGoing = true;
        while (keepGoing)
        {
            var gameSession = new GameSession(seed, handSize, showAllHands);
            var winnerName = gameSession.RunSession();

            Console.WriteLine();
            Console.WriteLine("Winner: " + winnerName);
            Console.WriteLine();
            Console.Write("Play again? (y/n): ");
            var answer = Console.ReadLine();
            if (answer == null || answer.Trim().Length == 0)
            {
                keepGoing = false;
            }
            else
            {
                var a = answer.Trim().ToLowerInvariant();
                if (a == "y" || a == "yes")
                {
                    keepGoing = true;
                    seed++;
                }
                else
                {
                    keepGoing = false;
                }
            }
        }
    }
}
