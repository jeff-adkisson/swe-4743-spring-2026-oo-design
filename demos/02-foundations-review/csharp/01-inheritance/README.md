# Inheritance

## How To Run
`dotnet run Program-01.cs`

_Note: Requires C# 10 to run from the command line without a `.csproj` file._

## Class Diagram

```mermaid
---
config:
  theme: mc
---
classDiagram
direction BT

class Publication {
  <<abstract>>
  +Title: string
  +Publication(title: string)
  +GetDetails() string
}

class PaperbackBook {
  +Pages: int
  +PaperbackBook(title: string, pages: int)
}

class Scroll {
  +LengthMillimeters: int
  +Scroll(title: string, lengthMillimeters: int)
}

PaperbackBook --|> Publication : "is-a"
Scroll --|> Publication : "is-a"
```