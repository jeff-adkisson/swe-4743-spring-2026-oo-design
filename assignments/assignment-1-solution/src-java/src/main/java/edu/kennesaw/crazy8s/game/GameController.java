package edu.kennesaw.crazy8s.game;

import edu.kennesaw.crazy8s.ProgramContext;

/**
 * Coordinates the overall game session loop and restarts.
 */
public class GameController {
    private final ProgramContext programContext;

    public GameController(ProgramContext programContext) {
        this.programContext = programContext;
    }

    public void start() {
        showIntroduction();
        boolean continuePlaying = true;
        while (continuePlaying) {
            GameConsole.clear();
            GameEngine gameEngine = new GameEngine(programContext);
            gameEngine.startGame();

            GameConsole.writeSeparator(50, 1);
            continuePlaying = GameConsole.promptYesNo("Do you want to play again? (Y/N): ");
        }
    }

    private static void showIntroduction() {
        GameConsole.clear();
        GameConsole.writeSeparator();
        GameConsole.writeLine("Welcome to *Simplified* Crazy Eights!");
        GameConsole.writeLine();
        GameConsole.writeLine("- Get rid of all your cards to win the game.");
        GameConsole.writeLine("  When the deck runs out, the player with the fewest cards wins.");
        GameConsole.writeLine("  If multiple players have the same fewest cards, they tie.");
        GameConsole.writeLine("- You can play a card that matches the rank or suit of the top discard.");
        GameConsole.writeLine("  For example, if the top card is a 5 of Hearts, you can play any 5 or any Heart.");
        GameConsole.writeLine("  Playing a rank may change the current suit to that of the played card.");
        GameConsole.writeLine("- Eights are wild and can be played on any card.");
        GameConsole.writeLine("  When you play an eight, you can choose a new suit.");
        GameConsole.writeLine("- If you cannot play a card, you must draw from the deck.");
        GameConsole.writeLine("  You can play a drawn cade immediately if it matches or is wild.");
        GameConsole.writeLine("- The CPU's gameplay is random. You are likely to win.");
        GameConsole.writeLine();
        GameConsole.writeLine("Good luck and have fun!");
        GameConsole.writeSeparator();
        GameConsole.readLine("Press Enter to start the game...");
    }
}
