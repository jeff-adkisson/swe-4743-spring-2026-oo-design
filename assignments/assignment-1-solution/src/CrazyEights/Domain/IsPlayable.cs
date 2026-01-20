namespace CrazyEights.Domain;

public class IsPlayable
{
    public IsPlayable(bool canPlay, string reason = "")
    {
        CanPlay = canPlay;
        Reason = reason;
    }

    public bool CanPlay { get; }

    public string Reason { get; }
}