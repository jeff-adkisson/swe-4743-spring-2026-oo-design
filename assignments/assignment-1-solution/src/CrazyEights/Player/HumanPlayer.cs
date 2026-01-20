using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;

namespace CrazyEights.Player;

public class HumanPlayer : PlayerBase
{
    public const string DefaultName = "Player";

    private const bool ShowMyHand = true;

    public HumanPlayer(string name, Hand hand) : base(name, hand, ShowMyHand)
    {
    }

    public override ICard SelectCard(TurnContext turnContext)
    {
        var playableCards =
            Hand.GetPlayableCards(turnContext.CurrentSuit, turnContext.CurrentRank);

        //cannot select a card
        if (playableCards.Count == 0)
            return UnselectedCard.Instance;

        ShowPlayableCardsInHand(turnContext, playableCards);

        var isValidInput = false;
        ICard selectedCard = UnselectedCard.Instance;
        while (!isValidInput)
        {
            var input = GameConsole.ReadLine("Choose a card number to play or 0 to draw: ");
            var isNumber = int.TryParse(input, out var selectedNumber);
            if (!isNumber || selectedNumber < 0 || selectedNumber > playableCards.Count)
            {
                GameConsole.WriteLine("- Invalid choice! Please try again.");
                continue;
            }

            if (selectedNumber > 0)
                selectedCard = playableCards[selectedNumber - 1].Card;
            isValidInput = true;
        }

        return selectedCard;
    }

    private static void ShowPlayableCardsInHand(TurnContext turnContext, IReadOnlyList<PlayableCard> playableCards)
    {
        var currentPlayer = turnContext.CurrentPlayer;
        var name = currentPlayer.Name;

        GameConsole.WriteLine();
        GameConsole.WriteLine($"{name}'s playable cards");

        for (var i = 0; i < playableCards.Count; i++)
        {
            var playableCard = playableCards[i];
            var card = playableCard.Card;
            var selector = i + 1;
            var cardDescription = card.GetDescription();
            var playableReason = playableCard.PlayableReason;
            GameConsole.WriteLine($"  [{selector}] {cardDescription} ({playableReason})");
        }
    }

    public override SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
    {
        var currentSuit = gameContext.DiscardPile.ActiveSuit;
        var suits = Suit.GetSuits();
        var name = turnContext.CurrentPlayer.Name;

        //select a suit
        var isValidInput = false;
        var selectedSuit = SuitType.NotSet;

        //choose the first letter of a valid suit
        while (!isValidInput)
        {
            GameConsole.WriteLine();
            GameConsole.WriteLine($"{name}, you played a wildcard! Choose a suit:");
            foreach (var suit in suits)
            {
                var suitName = Suit.GetSuitName(suit);
                var letter = suitName[0].ToString().ToUpper();
                var isCurrentSuitNote = currentSuit == suit ? " (current suit)" : "";
                GameConsole.WriteLine($"  [{letter}] {suit}{isCurrentSuitNote}");
            }

            var input = GameConsole.ReadLine("Enter the letter of your chosen suit: ");
            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                GameConsole.WriteLine("- Invalid choice! Please try again.");
                continue;
            }

            var selectedLetter = char.ToUpper(input[0]);
            switch (selectedLetter)
            {
                case 'H':
                    selectedSuit = SuitType.Hearts;
                    break;
                case 'D':
                    selectedSuit = SuitType.Diamonds;
                    break;
                case 'C':
                    selectedSuit = SuitType.Clubs;
                    break;
                case 'S':
                    selectedSuit = SuitType.Spades;
                    break;
                default:
                    GameConsole.WriteLine("- Invalid choice! Please try again.");
                    continue;
            }

            isValidInput = true;
        }

        return selectedSuit;
    }

    public override bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    {
        GameConsole.WriteLine();
        var playDrawnCard = GameConsole.PromptYesNo("You drew a playable card. Play it now (Y/N): ");
        return playDrawnCard;
    }
}
