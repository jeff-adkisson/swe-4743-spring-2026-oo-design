using CrazyEights.Cards;

namespace CrazyEights.Domain;

public static class PlayableCardsSelector
{
    public static IReadOnlyList<PlayableCard> Get(
        IReadOnlyList<ICard> cards,
        SuitType currentSuit,
        RankType currentRank)
    {
        var playableCards = new List<PlayableCard>();
        foreach (var card in cards)
        {
            var outcome = CanPlayCard(card, currentSuit, currentRank);
            if (!outcome.CanPlay)
                continue;

            var playableCard = new PlayableCard(card, outcome.Reason);
            playableCards.Add(playableCard);
        }

        return playableCards.AsReadOnly();
    }

    public static IsPlayable CanPlayCard(ICard card, SuitType currentSuitType, RankType currentRankType)
    {
        if (Rank.IsWildcardRank(card.Rank))
            return new IsPlayable(true, "Wildcard!");

        if (currentRankType == card.Rank)
            return new IsPlayable(true, "Matches Rank");

        if (currentSuitType == card.Suit)
            return new IsPlayable(true, "Matches Suit");

        return new IsPlayable(false, "Not Playable");
    }
}