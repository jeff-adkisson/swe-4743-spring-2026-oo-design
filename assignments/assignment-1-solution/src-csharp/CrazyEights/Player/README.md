# CrazyEights.Player

## Purpose

Defines player behavior and hand management. This namespace provides the player interface and base class, concrete human/CPU strategies, and supporting classes for dealing and player rotation.

## Analysis vs. assignment-1.md

- `IPlayer` and `PlayerBase` meet the interface and abstract class requirements while keeping hand state encapsulated in `Hand`.
- `HumanPlayer` and `CpuPlayer` implement decision-making, enabling `GameEngine` to call `TakeTurn` without checking types, which matches the polymorphism requirement.
- `Hand` provides controlled access to cards and uses domain playability checks, aligning with encapsulation rules (no public mutable hand list).
- `Players` stores a collection of `IPlayer`, enabling turn rotation through the interface as required.
- `PlayerRegistration` and `HandDealer` keep setup logic out of `Main`, though `PlayerRegistration` performs console input directly rather than through a UI abstraction.

## UML (Mermaid)

```mermaid
classDiagram
    class IPlayer {
        <<interface>>
        +string Name
        +int CardCount
        +bool ShowHand
        +void AddCard(ICard card)
        +void RemoveCard(ICard card)
        +IReadOnlyList~ICard~ PeekHand()
        +void TakeTurn(TurnContext context)
        +ICard SelectCard(TurnContext context)
        +SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
        +bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    }

    class PlayerBase {
        <<abstract>>
        +PlayerBase(string name, Hand hand, bool showHand)
        +string Name
        +int CardCount
        +bool ShowHand
        +void AddCard(ICard card)
        +void RemoveCard(ICard card)
        +IReadOnlyList~ICard~ PeekHand()
        +void TakeTurn(TurnContext context)
        +ICard SelectCard(TurnContext context)
        +SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
        +bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    }

    class HumanPlayer {
        +HumanPlayer(string name, Hand hand)
        +ICard SelectCard(TurnContext context)
        +SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
        +bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    }

    class CpuPlayer {
        +CpuPlayer(Hand hand, bool showHand)
        +ICard SelectCard(TurnContext context)
        +SuitType SelectSuit(GameContext gameContext, TurnContext turnContext)
        +bool WillPlayDrawnCard(TurnContext turnContext, ICard drawnCard)
    }

    class Hand {
        -List~ICard~ _cards
        +int CardCount
        +IReadOnlyList~ICard~ CardList
        +void AddCard(ICard card)
        +void RemoveCard(ICard card)
        +IReadOnlyList~PlayableCard~ GetPlayableCards(SuitType currentSuit, RankType currentRank)
    }

    class Players {
        -List~IPlayer~ _players
        -int _currentPlayerIndex
        +IReadOnlyList~IPlayer~ List
        +IPlayer CurrentPlayer
        +void Add(IPlayer player)
        +IPlayer MoveToNextPlayer()
        +int GetSmallestHandCardCount()
        +IReadOnlyList~IPlayer~ GetPlayersWithLeastCards()
    }

    class HandDealer {
        <<static>>
        +Hand[] Deal(GameContext gameContext, int numberOfPlayers)
    }

    class PlayerRegistration {
        <<static>>
        +void Register(GameContext gameContext)
    }

    IPlayer <|.. PlayerBase
    PlayerBase <|-- HumanPlayer
    PlayerBase <|-- CpuPlayer
    PlayerBase ..> Hand
    Hand ..> PlayableCardsSelector
    Players ..> IPlayer
    Hand ..> ICard
    Hand ..> PlayableCard
    HandDealer ..> GameContext
    PlayerRegistration ..> GameContext
    PlayerRegistration ..> HumanPlayer
    PlayerRegistration ..> CpuPlayer
    PlayerRegistration ..> HandDealer
```
