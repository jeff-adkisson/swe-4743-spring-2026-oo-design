namespace CrazyEights.Domain;

public class PlayableResult
{
    public PlayableResult(bool canPlay, string reason = "")
    {
        CanPlay = canPlay;
        Reason = reason;
    }

    public bool CanPlay { get; }

    public string Reason { get; }
}