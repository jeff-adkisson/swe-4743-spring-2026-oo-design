using CrazyEights.Game;

namespace CrazyEights.Player;

public static class PlayerRegistration
{
    public static void Register(GameContext gameContext)
    {
        var defaultName = HumanPlayer.DefaultName;
        Console.Write($"Enter your name (or press Enter for '{defaultName}'):");
        var yourName = Console.ReadLine();
        yourName = string.IsNullOrEmpty(yourName) ? defaultName : yourName.Trim();

        var players = gameContext.Players;
        var hands = HandDealer.Deal(gameContext, 2);
        players.Add(new HumanPlayer(yourName, hands[0]));
        players.Add(new CpuPlayer(hands[1], gameContext.ShowAllHands));
    }
}