using CrazyEights.CardDeck;
using CrazyEights.Player;

namespace CrazyEights.Game;

public class GameEngine
{
    private readonly GameContext _gameContext;

    public GameEngine(ProgramContext programContext)
    {
        var deck = DeckInitializer.CreateCardDeck(programContext);

        _gameContext = new GameContext(
            programContext,
            deck,
            new DiscardPile(),
            new Players());
    }

    public void StartGame()
    {
        PlayerRegistration.Register(_gameContext);
        GameConsole.WriteSeparator();
        GameConsole.WriteLine(_gameContext.GameTitle);
        GameConsole.WriteSeparator();

        var deck = _gameContext.Deck;
        var discardPile = InitializeDiscardPile(deck);

        var players = _gameContext.Players;
        var currentPlayer = players.CurrentPlayer;

        ExecuteGameLoop(deck, discardPile, currentPlayer, players);

        ShowWinners(players);
    }

    private static void ShowWinners(Players players)
    {
        GameConsole.WriteLine();

        var winners = players.GetPlayersWithLeastCards();
        if (winners.Count == 1)
        {
            var winner = winners[0];
            GameConsole.WriteLine($"***** {winner.Name} wins the game! *****");
            return;
        }

        //tie between multiple players
        var tieWinnerNames = string.Join(", ", winners.Select(w => w.Name));
        GameConsole.WriteLine($"***** It's a tie between: {tieWinnerNames}! *****");
    }

    private void ExecuteGameLoop(Deck deck, DiscardPile discardPile, IPlayer currentPlayer, Players players)
    {
        //continue until deck is empty or a player has no cards left
        while (!deck.IsEmpty && players.GetSmallestHandCardCount() > 0)
        {
            _gameContext.IncrementTurn();
            var turnContext = GetTurnContext(_gameContext, discardPile, currentPlayer);
            GameConsole.WriteLine();
            TurnAction.ShowTurn(_gameContext, turnContext);
            TurnAction.StartTurn(_gameContext, turnContext);
            currentPlayer = players.MoveToNextPlayer();
        }
    }

    private static TurnContext GetTurnContext(GameContext gameContext, DiscardPile discardPile, IPlayer currentPlayer)
    {
        var rng = gameContext.RandomNumberGenerator;
        var currentSuit = discardPile.ActiveSuit;
        var topCard = discardPile.TopCard;

        var turnContext = new TurnContext(rng, currentPlayer, currentSuit, topCard);
        return turnContext;
    }

    private DiscardPile InitializeDiscardPile(Deck deck)
    {
        var discardPile = _gameContext.DiscardPile;
        discardPile.AddCard(deck.DrawCard());

        var topCard = discardPile.TopCard;
        TurnAction.ShowTopCard(topCard, topCard.Suit, "Starting discard: ");
        return discardPile;
    }
}