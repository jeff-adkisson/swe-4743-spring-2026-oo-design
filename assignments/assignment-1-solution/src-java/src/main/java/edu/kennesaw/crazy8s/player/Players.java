package edu.kennesaw.crazy8s.player;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

/**
 * Manages the list of players and turn order.
 */
public class Players {
    private final List<Player> players = new ArrayList<>();
    private int currentPlayerIndex;

    public List<Player> getList() {
        return Collections.unmodifiableList(players);
    }

    public Player getCurrentPlayer() {
        if (players.isEmpty()) {
            throw new IllegalStateException("No players are registered.");
        }

        return players.get(currentPlayerIndex);
    }

    public void add(Player player) {
        players.add(player);
    }

    public Player moveToNextPlayer() {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.size();
        return getCurrentPlayer();
    }

    public int getSmallestHandCardCount() {
        int smallest = Integer.MAX_VALUE;
        for (Player player : players) {
            smallest = Math.min(smallest, player.getCardCount());
        }
        return smallest;
    }

    public List<Player> getPlayersWithLeastCards() {
        int smallestHandCount = getSmallestHandCardCount();
        List<Player> winners = new ArrayList<>();
        for (Player player : players) {
            if (player.getCardCount() == smallestHandCount) {
                winners.add(player);
            }
        }

        winners.sort(Comparator.comparing(Player::getName));
        return Collections.unmodifiableList(winners);
    }
}
