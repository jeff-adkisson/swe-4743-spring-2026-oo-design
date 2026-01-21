package edu.kennesaw.crazy8s;

import java.util.Random;

/**
 * Holds startup configuration values for the game.
 */
public class ProgramContext {
    public static final int DEFAULT_RANDOM_SEED = 0;
    public static final int DEFAULT_HAND_SIZE = 5;
    public static final boolean DEFAULT_SHOW_ALL_HANDS = false;

    private final boolean showAllHands;
    private final int handSize;
    private final Random randomNumberGenerator;

    public ProgramContext(int randomSeed, int handSize, boolean showAllHands) {
        this.showAllHands = showAllHands;
        this.handSize = handSize;
        this.randomNumberGenerator = randomSeed == 0 ? new Random() : new Random(randomSeed);
    }

    public boolean isShowAllHands() {
        return showAllHands;
    }

    public int getHandSize() {
        return handSize;
    }

    public Random getRandomNumberGenerator() {
        return randomNumberGenerator;
    }
}
