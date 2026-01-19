using CrazyEights.CardDeck;
using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Player;

namespace CrazyEights.Game;

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

    private static void DrawCard(GameContext gameContext, TurnContext turnContext)
    {
        var newCard = gameContext.Deck.DrawCard();
        var playerHand = turnContext.Hand;
        playerHand.AddCard(newCard);
        var name = turnContext.CurrentPlayer.Name;
        ShowMessage($"{name} has no playable cards. Drawing one card...", true);
        ShowMessage($"{name} drew {newCard.GetDescription()}");
        GameConsole.ReadLine("Press Enter to continue...");
    }

    private static void SelectCard(GameContext gameContext, TurnContext turnContext)
    {
        var currentPlayer = turnContext.CurrentPlayer;
        var selectedCard = currentPlayer.SelectCard(turnContext);

        if (!selectedCard.IsSelectable)
        {
            DrawCard(gameContext, turnContext);
            return;
        }

        turnContext.Hand.RemoveCard(selectedCard);
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
        var hand = turnContext.Hand;

        Console.WriteLine();
        GameConsole.WriteLine($"{name}'s hand");

        foreach (var card in hand.CardList) GameConsole.WriteLine($"  - {card.GetDescription()}");
    }

    private static void ShowRemainingDeckCount(Deck deck)
    {
        var cardCount = deck.CardCount;
        GameConsole.WriteLine($"Deck remaining: {cardCount} {Card.GetPluralCardLabel(cardCount)}");
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
            GameConsole.Write($"{name}: {cardCount} {Card.GetPluralCardLabel(cardCount)}");
        }

        GameConsole.WriteLine();
    }
}