using CrazyEights.Game;

namespace CrazyEights;

internal static class Program
{
    private static void Main(string[] args)
    {
        var randomSeed = args.Length > 0 ? int.Parse(args[0]) : 0;
        var cardsInHand = args.Length > 1 ? int.Parse(args[1]) : 7;
        var gameContext = new ProgramContext(randomSeed, cardsInHand);

        var gameEngine = new GameEngine(gameContext);
        gameEngine.Start();
    }
}