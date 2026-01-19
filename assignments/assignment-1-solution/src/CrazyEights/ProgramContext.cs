namespace CrazyEights;

public class ProgramContext
{
    public const int DefaultRandomSeed = 0;
    public const int DefaultHandSize = 5;
    public const bool DefaultShowAllHands = false;

    public ProgramContext(
        int randomSeed = DefaultRandomSeed,
        int handSize = DefaultHandSize,
        bool showAllHands = DefaultShowAllHands)
    {
        ShowAllHands = showAllHands;
        HandSize = handSize;
        RandomNumberGenerator = randomSeed == 0 ? new Random() : new Random(randomSeed);
    }

    public bool ShowAllHands { get; }

    public int HandSize { get; }

    public Random RandomNumberGenerator { get; }
}