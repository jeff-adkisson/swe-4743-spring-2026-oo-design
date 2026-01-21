namespace CrazyEightsBadExample;

/// <summary>
/// Basic holder for player identity and cards.
/// </summary>
internal class PlayerData
{
    public List<Card> Hand = new();
    public int Id;
    public string Name = "";
}
