package edu.kennesaw.crazy8s.domain;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Provides helpers for card ranks and wildcard logic.
 */
public final class Rank {
    private static final RankType WILDCARD_RANK = RankType.EIGHT;

    private Rank() {
    }

    public static String getRankName(RankType rank) {
        return rank.toString();
    }

    public static boolean isWildcardRank(RankType rank) {
        return rank == WILDCARD_RANK;
    }

    public static List<RankType> getRanks() {
        List<RankType> ranks = new ArrayList<>();
        for (RankType rank : RankType.values()) {
            if (rank != RankType.NOT_SET) {
                ranks.add(rank);
            }
        }

        return Collections.unmodifiableList(ranks);
    }
}
