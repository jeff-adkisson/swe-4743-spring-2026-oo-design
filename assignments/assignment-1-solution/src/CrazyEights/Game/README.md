# CrazyEights.Game

## Purpose

Coordinates gameplay flow, turn sequencing, and console interaction. This namespace owns the game loop, turn context, and high-level rules while remaining agnostic to concrete player types.

## Analysis vs. assignment-1.md

- `GameEngine` drives the loop using `Players` and `IPlayer.TakeTurn`, demonstrating polymorphism and dynamic dispatch without `is` checks.
- `TurnContext` and `GameContext` act as context objects, reducing parameter sprawl and aligning with the assignment's recommendation.
- `TurnAction` consolidates turn flow (select, draw, discard, wildcard suit changes) and keeps state transitions inside the engine rather than player hands, meeting the encapsulation guidance.
- `GameController` wraps replay behavior and introduction text, keeping `Program` focused on wiring only.
- `GameConsole` isolates I/O concerns so that the engine and domain are not tightly coupled to `Console` calls.

## UML (Mermaid)

```mermaid
classDiagram
    class GameContext {
        -ProgramContext _programContext
        +GameContext(ProgramContext programContext, Deck deck, DiscardPile discardPile, Players players)
        +Random RandomNumberGenerator
        +bool ShowAllHands
        +string GameTitle
        +Deck Deck
        +DiscardPile DiscardPile
        +Players Players
        +int HandSize
        +int TurnNumber
        +int IncrementTurn()
    }

    class TurnContext {
        +TurnContext(GameContext gameContext, Random rng, IPlayer currentPlayer, SuitType currentSuit, ICard topCard)
        +GameContext GameContext
        +Random RandomNumberGenerator
        +IPlayer CurrentPlayer
        +SuitType CurrentSuit
        +ICard TopCard
        +RankType CurrentRank
    }

    class TurnAction {
        <<static>>
        +void ShowTurn(GameContext gameContext, TurnContext turnContext)
        +void StartTurn(GameContext gameContext, TurnContext turnContext)
        +void ShowTopCard(ICard topCard, SuitType currentSuit, string prefix)
    }

    class GameEngine {
        -GameContext _gameContext
        +GameEngine(ProgramContext programContext)
        +void StartGame()
    }

    class GameController {
        -ProgramContext _programContext
        +GameController(ProgramContext programContext)
        +void Start()
    }

    class GameConsole {
        <<static>>
        +void WriteSeparator(int length, int blankLinesAround)
        +void Write(string text)
        +void WriteLine(string line)
        +string ReadLine(string line)
        +bool PromptYesNo(string line)
        +void Clear()
    }

    GameController ..> GameEngine : creates
    GameEngine ..> GameContext : owns
    GameEngine ..> DeckInitializer : builds deck
    GameEngine ..> PlayerRegistration : registers players
    GameEngine ..> TurnAction : runs turns
    GameEngine ..> TurnContext : creates
    TurnAction ..> TurnContext
    TurnAction ..> GameConsole
    TurnAction ..> PlayableCardsSelector
    TurnContext ..> GameContext
    TurnContext ..> IPlayer
```
