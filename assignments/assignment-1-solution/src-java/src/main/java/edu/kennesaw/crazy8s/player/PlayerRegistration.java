package edu.kennesaw.crazy8s.player;

import edu.kennesaw.crazy8s.game.GameConsole;
import edu.kennesaw.crazy8s.game.GameContext;

/**
 * Handles registering players for a new game.
 */
public final class PlayerRegistration {
    private PlayerRegistration() {
    }

    public static void register(GameContext gameContext) {
        String defaultName = HumanPlayer.DEFAULT_NAME;
        String prompt = "Enter your name (or press Enter for '" + defaultName + "'): ";
        String yourName = GameConsole.readLineRaw(prompt);
        if (yourName == null || yourName.trim().isEmpty()) {
            yourName = defaultName;
        } else {
            yourName = yourName.trim();
        }

        Players players = gameContext.getPlayers();
        Hand[] hands = HandDealer.deal(gameContext, 2);
        players.add(new HumanPlayer(yourName, hands[0]));
        players.add(new CpuPlayer(hands[1], gameContext.isShowAllHands()));
    }
}
