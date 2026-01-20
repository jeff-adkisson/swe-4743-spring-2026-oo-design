# CrazyEights.Domain

## Purpose

The domain namespace defines the *game concepts and rules* independent of UI or storage. It models ranks, suits, and "playability" rules so that other namespaces can ask domain questions ("can this card be played now?") without duplicating logic.

## Analysis vs. assignment-1.md

- `RankType` and `SuitType` are explicit domain vocabularies, keeping card identity and rule comparisons strongly typed.
- `Rank` and `Suit` expose domain rules (wildcard rank, valid ranks/suits, name/label helpers), which centralizes rules instead of scattering conditionals across the codebase.
- `PlayableCardsSelector` and `IsPlayable` encode the matching rules (rank, suit, wildcard), aligning directly with the assignment's simplified Crazy Eights rules and reducing the need for type checks.
- This namespace stays UI-free and is reusable by both the engine and players, matching the expectation that domain logic should be isolated from interaction concerns.

## UML (Mermaid)

```mermaid
classDiagram
    class RankType {
        <<enumeration>>
        NotSet
        Two
        Three
        Four
        Five
        Six
        Seven
        Eight
        Nine
        Ten
        Jack
        Queen
        King
        Ace
    }

    class SuitType {
        <<enumeration>>
        NotSet
        Hearts
        Diamonds
        Clubs
        Spades
    }

    class Rank {
        <<static>>
        -RankType WildcardRank
        +string GetRankName(RankType rank)
        +bool IsWildcardRank(RankType rank)
        +IReadOnlyList~RankType~ GetRanks()
    }

    class Suit {
        <<static>>
        +string GetSuitSymbol(SuitType suit)
        +string GetSuitName(SuitType suit)
        +IReadOnlyList~SuitType~ GetSuits()
    }

    class IsPlayable {
        +IsPlayable(bool canPlay, string reason)
        +bool CanPlay
        +string Reason
    }

    class PlayableCard {
        +PlayableCard(ICard card, string playableReason)
        +ICard Card
        +string PlayableReason
    }

    class PlayableCardsSelector {
        <<static>>
        +IReadOnlyList~PlayableCard~ Get(IReadOnlyList~ICard~ cards, SuitType currentSuit, RankType currentRank)
        +IsPlayable CanPlayCard(ICard card, SuitType currentSuit, RankType currentRank)
    }

    Rank ..> RankType
    Suit ..> SuitType
    PlayableCardsSelector ..> Rank
    PlayableCardsSelector ..> SuitType
    PlayableCardsSelector ..> RankType
    PlayableCardsSelector ..> IsPlayable
    PlayableCardsSelector ..> PlayableCard
    PlayableCard ..> ICard
```
