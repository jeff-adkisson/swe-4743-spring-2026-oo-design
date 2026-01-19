using CrazyEights.Game;

namespace CrazyEights;

internal static class Program
{
    private static void Main(string[] args)
    {
        var randomSeed = ProgramContext.DefaultRandomSeed;
        if (args.Length > 0 && int.TryParse(args[0], out var parsedSeed))
            randomSeed = parsedSeed;

        var cardsInHand = ProgramContext.DefaultHandSize;
        if (args.Length > 1 && int.TryParse(args[1], out var parsedHandSize) && parsedHandSize > 0)
            cardsInHand = parsedHandSize;

        var showAllHands = ProgramContext.DefaultShowAllHands;
        if (args.Length > 2 && bool.TryParse(args[2], out var parsedShowAllHands))
            showAllHands = parsedShowAllHands;

        var programContext = new ProgramContext(randomSeed, cardsInHand, showAllHands);

        var gameController = new GameController(programContext);
        gameController.Start();
    }
}