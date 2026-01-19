using CrazyEights.Game;

namespace CrazyEights;

internal static class Program
{
    private const int DefaultRandomSeed = 0;
    private const int DefaultCardsInHand = 5;
    private const bool ShowAllHands = false;

    private static void Main(string[] args)
    {

        int randomSeed = DefaultRandomSeed;
        if (args.Length > 0)
        {
            int.TryParse(args[0], out randomSeed);
        }

        int cardsInHand = DefaultCardsInHand;
        if (args.Length > 1)
        {
            int.TryParse(args[1], out cardsInHand);
        }

        bool showAllHands = ShowAllHands;
        if (args.Length > 2)
        {
            bool.TryParse(args[2], out showAllHands);
        }

        var gameContext = new ProgramContext(randomSeed, cardsInHand, showAllHands);

        var gameEngine = new GameEngine(gameContext);
        gameEngine.Start();
    }
}