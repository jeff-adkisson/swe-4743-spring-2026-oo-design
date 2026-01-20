namespace CrazyEights.Game;

public class GameController
{
    private readonly ProgramContext _programContext;

    public GameController(ProgramContext programContext)
    {
        _programContext = programContext;
    }

    public void Start()
    {
        ShowIntroduction();
        var continuePlaying = true;
        while (continuePlaying)
        {
            GameConsole.Clear();
            var gameEngine = new GameEngine(_programContext);
            gameEngine.StartGame();

            GameConsole.WriteSeparator(blankLinesAround: 1);
            continuePlaying = GameConsole.PromptYesNo("Do you want to play again? (Y/N): ");
        }
    }

    private static void ShowIntroduction()
    {
        GameConsole.Clear();
        GameConsole.WriteSeparator();
        GameConsole.WriteLine("Welcome to *Simplified* Crazy Eights!");
        GameConsole.WriteLine();
        GameConsole.WriteLine("- Get rid of all your cards to win the game.");
        GameConsole.WriteLine("  When the deck runs out, the player with the fewest cards wins.");
        GameConsole.WriteLine("  If multiple players have the same fewest cards, they tie.");
        GameConsole.WriteLine("- You can play a card that matches the rank or suit of the top discard.");
        GameConsole.WriteLine("  For example, if the top card is a 5 of Hearts, you can play any 5 or any Heart.");
        GameConsole.WriteLine("  Playing a rank may change the current suit to that of the played card.");
        GameConsole.WriteLine("- Eights are wild and can be played on any card.");
        GameConsole.WriteLine("  When you play an eight, you can choose a new suit.");
        GameConsole.WriteLine("- If you cannot play a card, you must draw from the deck.");
        GameConsole.WriteLine("  You can play a drawn cade immediately if it matches or is wild.");
        GameConsole.WriteLine("- The CPU's gameplay is random. You are likely to win.");
        GameConsole.WriteLine();
        GameConsole.WriteLine("Good luck and have fun!");
        GameConsole.WriteSeparator();
        GameConsole.ReadLine("Press Enter to start the game...");
    }
}