using System.Collections.Immutable;

namespace CrazyEights.Domain;

public static class Rank
{
    private const RankType WildcardRank = RankType.Eight;

    public static string GetRankName(RankType rank)
    {
        return rank.ToString();
    }

    public static bool IsWildcardRank(RankType rank)
    {
        return rank == WildcardRank;
    }

    public static IReadOnlyList<RankType> GetRanks()
    {
        return Enum.GetValues<RankType>()
            .Where(rank => rank != RankType.NotSet)
            .ToImmutableList();
    }
}