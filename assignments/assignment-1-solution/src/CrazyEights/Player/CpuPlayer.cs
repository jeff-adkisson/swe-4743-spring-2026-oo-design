using CrazyEights.Cards;
using CrazyEights.Domain;
using CrazyEights.Game;

namespace CrazyEights.Player;

public class CpuPlayer : PlayerBase
{
    private const string CpuPlayerName = "CPU";

    public CpuPlayer(Hand hand, bool showHand) : base(CpuPlayerName, hand, showHand)
    {
    }

    public override ICard SelectCard(TurnContext turnContext)
    {
        var rng = turnContext.RandomNumberGenerator;
        var playableCards = Hand.GetPlayableCards(turnContext.CurrentSuit, turnContext.CurrentRank);

        //cannot play a card
        if (playableCards.Count == 0) return UnselectedCard.Instance;

        //select whether to play a card or not
        var playACard = rng.NextDouble() > 0.5;
        if (!playACard) return UnselectedCard.Instance;

        //randomly select a card to play
        var randomCardIndex =
            turnContext.RandomNumberGenerator.Next(0, playableCards.Count);
        var selectedCard = playableCards[randomCardIndex].Card;

        return selectedCard;
    }

    public override SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
    {
        var rng = turnContext.RandomNumberGenerator;
        var suitValues = Suit.GetSuits();
        var randomSuitIndex = rng.Next(0, suitValues.Count - 1);
        var selectedSuit = suitValues[randomSuitIndex];
        return selectedSuit;
    }

    public override bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    {
        var rng = turnContext.RandomNumberGenerator;
        var playDrawnCard = rng.NextDouble() > 0.5;
        return playDrawnCard;
    }
}