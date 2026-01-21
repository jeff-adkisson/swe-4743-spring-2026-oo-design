using CrazyEights.CardDeck;
using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Player;

namespace CrazyEights.Game;

/// <summary>
/// Implements turn execution steps and game UI prompts.
/// </summary>
public static class TurnAction
{
    public static void ShowTurn(GameContext gameContext, TurnContext turnContext)
    {
        var turnCount = gameContext.TurnNumber;
        var players = gameContext.Players;

        GameConsole.WriteLine($"----- TURN {turnCount} -----");
        ShowTopCard(turnContext.TopCard, turnContext.CurrentSuit);
        ShowRemainingDeckCount(gameContext.Deck);
        ShowCardCounts(players);

        ShowMessage($"{turnContext.CurrentPlayer.Name}'s turn".ToUpper(), true);
    }

    public static void StartTurn(GameContext gameContext, TurnContext turnContext)
    {
        var player = turnContext.CurrentPlayer;

        if (player.ShowHand)
            ShowAllCardsInHand(turnContext);

        SelectCard(gameContext, turnContext);
    }

    private static ICard DrawCard(GameContext gameContext, TurnContext turnContext)
    {
        var player = turnContext.CurrentPlayer;
        var name = player.Name;
        ShowMessage($"{name} has no playable cards. Drawing one card...", true);

        var drawnCard = gameContext.Deck.DrawCard();
        player.AddCard(drawnCard);

        var cardDescription = player.ShowHand ? drawnCard.GetDescription() : "a card";
        ShowMessage($"{name} drew {cardDescription}");

        var canPlayDrawnCard = PlayableCardsSelector.CanPlayCard(
            drawnCard,
            turnContext.CurrentSuit,
            turnContext.CurrentRank);
        if (canPlayDrawnCard.CanPlay && player.WillPlayDrawnCard(turnContext, drawnCard)) return drawnCard;

        GameConsole.ReadLine("Press Enter to continue...");
        return UnselectedCard.Instance;
    }

    private static void SelectCard(GameContext gameContext, TurnContext turnContext)
    {
        var currentPlayer = turnContext.CurrentPlayer;
        var selectedCard = currentPlayer.SelectCard(turnContext);

        if (!selectedCard.IsSelectable)
        {
            var drawnCard = DrawCard(gameContext, turnContext);
            if (!drawnCard.IsSelectable) return;
            selectedCard = drawnCard;
        }

        currentPlayer.RemoveCard(selectedCard);
        gameContext.DiscardPile.AddCard(selectedCard);
        var name = turnContext.CurrentPlayer.Name;
        ShowMessage($"{name} selected {selectedCard.GetDescription()}");

        if (Rank.IsWildcardRank(selectedCard.Rank)) ChooseSuit(gameContext, turnContext);
    }

    private static void ChooseSuit(GameContext gameContext, TurnContext turnContext)
    {
        var currentPlayer = turnContext.CurrentPlayer;
        var chosenSuit = currentPlayer.SelectSuit(gameContext, turnContext);
        var name = turnContext.CurrentPlayer.Name;

        var discardPile = gameContext.DiscardPile;
        if (discardPile.ActiveSuit != chosenSuit)
        {
            discardPile.OverrideTopCardSuit(chosenSuit);
            ShowMessage($"{name} changed suit to {chosenSuit}");
            return;
        }

        ShowMessage($"{name} left the suit as {chosenSuit}");
    }

    private static void ShowMessage(string action, bool showBlankLineBefore = false)
    {
        if (showBlankLineBefore) GameConsole.WriteLine();
        GameConsole.WriteLine($"** {action}");
    }

    private static void ShowAllCardsInHand(TurnContext turnContext)
    {
        var name = turnContext.CurrentPlayer.Name;
        var hand = turnContext.CurrentPlayer.PeekHand();

        Console.WriteLine();
        GameConsole.WriteLine($"{name}'s hand");

        foreach (var card in hand) GameConsole.WriteLine($"  - {card.GetDescription()}");
    }

    private static void ShowRemainingDeckCount(Deck deck)
    {
        var cardCount = deck.CardCount;
        GameConsole.WriteLine($"Deck remaining: {cardCount} {GetPluralCardLabel(cardCount)}");
    }

    public static void ShowTopCard(ICard topCard, SuitType currentSuit, string prefix = "Top discard: ")
    {
        var isSuitChanged = topCard.Suit != currentSuit;
        var suitToMatch = isSuitChanged
            ? $" (Suit to match: {currentSuit})"
            : "";

        GameConsole.WriteLine($"{prefix}{topCard.GetDescription()}{suitToMatch}");
    }

    private static void ShowCardCounts(Players players)
    {
        for (var i = 0; i < players.List.Count; i++)
        {
            if (i > 0) GameConsole.Write(" | ");
            var player = players.List[i];
            var name = player.Name;
            var cardCount = player.CardCount;
            GameConsole.Write($"{name}: {cardCount} {GetPluralCardLabel(cardCount)}");
        }

        GameConsole.WriteLine();
    }

    private static string GetPluralCardLabel(int count)
    {
        return count == 1 ? "card" : "cards";
    }
}
