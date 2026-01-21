package edu.kennesaw.crazy8s;

import edu.kennesaw.crazy8s.game.GameController;

/**
 * Entry point for the Crazy Eights console application.
 */
public final class Main {
    private Main() {
    }

    public static void main(String[] args) {
        int randomSeed = ProgramContext.DEFAULT_RANDOM_SEED;
        if (args.length > 0) {
            Integer parsedSeed = tryParseInt(args[0]);
            if (parsedSeed != null) {
                randomSeed = parsedSeed;
            }
        }

        int cardsInHand = ProgramContext.DEFAULT_HAND_SIZE;
        if (args.length > 1) {
            Integer parsedHandSize = tryParseInt(args[1]);
            if (parsedHandSize != null && parsedHandSize > 0) {
                cardsInHand = parsedHandSize;
            }
        }

        boolean showAllHands = ProgramContext.DEFAULT_SHOW_ALL_HANDS;
        if (args.length > 2) {
            Boolean parsedShowAllHands = tryParseBoolean(args[2]);
            if (parsedShowAllHands != null) {
                showAllHands = parsedShowAllHands;
            }
        }

        ProgramContext programContext = new ProgramContext(randomSeed, cardsInHand, showAllHands);

        GameController gameController = new GameController(programContext);
        gameController.start();
    }

    private static Integer tryParseInt(String value) {
        try {
            return Integer.parseInt(value);
        } catch (NumberFormatException ex) {
            return null;
        }
    }

    private static Boolean tryParseBoolean(String value) {
        if (value == null) {
            return null;
        }

        String normalized = value.trim().toLowerCase();
        if ("true".equals(normalized)) {
            return Boolean.TRUE;
        }
        if ("false".equals(normalized)) {
            return Boolean.FALSE;
        }

        return null;
    }
}
