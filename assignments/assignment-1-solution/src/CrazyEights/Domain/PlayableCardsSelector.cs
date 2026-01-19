using CrazyEights.Cards;
using CrazyEights.Player;

namespace CrazyEights.Domain;

public static class PlayableCardsSelector
{
    public static IReadOnlyList<PlayableCard> Get(Hand hand, SuitType currentSuit, RankType currentRank)
    {
        var playableCards = new List<PlayableCard>();
        var selectorCount = 0;
        foreach (var card in hand.CardList)
        {
            var outcome = CanPlayCard(card, currentSuit, currentRank);
            if (!outcome.CanPlay)
                continue;

            selectorCount++;
            var payableCard = new PlayableCard(card, selectorCount, outcome.Reason);
            playableCards.Add(payableCard);
        }

        return playableCards.AsReadOnly();
    }

    public static PlayableResult CanPlayCard(ICard card, SuitType currentSuitType, RankType currentRankType)
    {
        if (Rank.IsWildcardRank(card.Rank))
            return new PlayableResult(true, "Wildcard!");

        if (currentRankType == card.Rank)
            return new PlayableResult(true, "Matches Rank");

        if (currentSuitType == card.Suit)
            return new PlayableResult(true, "Matches Suit");

        return new PlayableResult(false, "Not Playable");
    }
}