# CrazyEights.CardDeck

## Purpose

Encapsulates deck and discard pile state transitions. This namespace owns card creation, shuffling, drawing, and discard pile rules so that other parts of the system cannot mutate collections directly.

## Analysis vs. assignment-1.md

- `Deck` and `DiscardPile` hide their internal stacks and expose only safe operations (`DrawCard`, `AddCard`, `OverrideTopCardSuit`), meeting the encapsulation requirements.
- `DeckInitializer` builds a standard deck and shuffles using the program context RNG, keeping deck setup out of `Main` and avoiding hard-coded rules in the engine.
- Discard pile suit overrides implement the "eights are wild" rule while keeping suit state centralized in the pile, aligning with the rule ownership guidance.

## UML (Mermaid)

```mermaid
classDiagram
    class Deck {
        -Stack~ICard~ _cards
        +Deck(IEnumerable~ICard~ cards)
        +int CardCount
        +bool IsEmpty
        +ICard DrawCard()
    }

    class DiscardPile {
        -Stack~ICard~ _discardedCards
        -SuitType OverriddenSuit
        +ICard TopCard
        +SuitType ActiveSuit
        +void AddCard(ICard card)
        +void OverrideTopCardSuit(SuitType newSuit)
    }

    class DeckInitializer {
        <<static>>
        +Deck CreateCardDeck(ProgramContext programContext)
    }

    DeckInitializer ..> Deck : creates
    DeckInitializer ..> Card : builds
    DeckInitializer ..> Rank
    DeckInitializer ..> Suit
    DeckInitializer ..> ProgramContext
    DiscardPile ..> ICard
    DiscardPile ..> SuitType
    Deck ..> ICard
```
