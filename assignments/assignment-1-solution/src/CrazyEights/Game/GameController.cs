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
}