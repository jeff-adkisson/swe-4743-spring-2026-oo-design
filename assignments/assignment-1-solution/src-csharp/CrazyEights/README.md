# CrazyEights (root namespace)

## Purpose

The root namespace is the composition root for the application. It owns startup wiring and runtime configuration, then hands control to the game layer without embedding gameplay rules.

## Analysis vs. assignment-1.md

- `Program` constructs a `ProgramContext`, creates `GameController`, and starts the game, which matches the requirement that `Main` only wires objects and launches gameplay.
- `ProgramContext` centralizes configuration (seed, hand size, reveal hands) so other namespaces can accept a context object instead of many parameters, aligning with the assignment's context-object guidance.
- No game rules, turn logic, or card-matching logic appear here, keeping the composition root "short and boring" as requested.

## UML (Mermaid)

```mermaid
classDiagram
    class Program {
        <<static>>
        -Main(string[] args)
    }

    class ProgramContext {
        +int DefaultRandomSeed
        +int DefaultHandSize
        +bool DefaultShowAllHands
        +ProgramContext(int randomSeed, int handSize, bool showAllHands)
        +bool ShowAllHands
        +int HandSize
        +Random RandomNumberGenerator
    }

    Program ..> ProgramContext : builds
    Program ..> GameController : starts
```

## Cross-namespace UML (Mermaid)

```mermaid
classDiagram
    class Program
    class ProgramContext
    class GameController
    class GameEngine
    class GameContext
    class TurnContext
    class TurnAction
    class GameConsole
    class Deck
    class DiscardPile
    class DeckInitializer
    class ICard
    class Card
    class IPlayer
    class PlayerBase
    class HumanPlayer
    class CpuPlayer
    class Hand
    class Players
    class PlayerRegistration
    class HandDealer
    class PlayableCardsSelector
    class RankType
    class SuitType

    Program ..> ProgramContext
    Program ..> GameController
    GameController ..> GameEngine
    GameEngine ..> GameContext
    GameEngine ..> DeckInitializer
    GameEngine ..> TurnAction
    GameEngine ..> TurnContext
    GameContext ..> Deck
    GameContext ..> DiscardPile
    GameContext ..> Players
    TurnContext ..> GameContext
    TurnContext ..> IPlayer
    TurnAction ..> GameConsole
    TurnAction ..> PlayableCardsSelector

    DeckInitializer ..> Deck
    DeckInitializer ..> Card
    Deck ..> ICard
    DiscardPile ..> ICard

    IPlayer <|.. PlayerBase
    PlayerBase <|-- HumanPlayer
    PlayerBase <|-- CpuPlayer
    Players ..> IPlayer
    Hand ..> ICard
    Hand ..> PlayableCardsSelector
    PlayerRegistration ..> HandDealer
    PlayerRegistration ..> HumanPlayer
    PlayerRegistration ..> CpuPlayer

    Card ..> RankType
    Card ..> SuitType
    PlayableCardsSelector ..> RankType
    PlayableCardsSelector ..> SuitType
```
