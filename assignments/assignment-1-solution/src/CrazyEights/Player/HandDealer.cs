using CrazyEights.Game;

namespace CrazyEights.Player;

public static class HandDealer
{
    public static Hand[] Deal(GameContext gameContext, int numberOfPlayers)
    {
        var hands = new Hand[numberOfPlayers];
        var deck = gameContext.Deck;
        var handSize = gameContext.HandSize;

        if (deck.CardCount < numberOfPlayers * handSize)
        {
            throw new InvalidOperationException("Not enough cards in the deck to deal hands to all players.");
        }

        for (var i = 0; i < numberOfPlayers; i++)
        {
            var hand = new Hand();
            for (var j = 0; j < handSize; j++)
            {
                var card = deck.DrawCard();
                hand.AddCard(card);
            }

            hands[i] = hand;
        }

        return hands;
    }
}