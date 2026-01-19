namespace CrazyEights;

public class ProgramContext
{
    public ProgramContext(int randomSeed = 0, int handSize = 7, bool showAllHands = false)
    {
        ShowAllHands = showAllHands;
        HandSize = handSize;
        RandomNumberGenerator = randomSeed == 0 ? new Random() : new Random(randomSeed);
    }

    public bool ShowAllHands { get; }

    public int HandSize { get; }

    public Random RandomNumberGenerator { get; }
}