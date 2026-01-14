# Polymorphism 1

## How To Run
`dotnet run Program-02.cs`

_Note: Requires C# 10 to run from the command line without a `.csproj` file._

## Class Diagram

```mermaid
classDiagram
direction BT

class Publication {
  <<abstract>>
  +Title: string
  +Publication(title: string)
  +GetDetails() string* 
}

class PaperbackBook {
  +Pages: int
  +PaperbackBook(title: string, pages: int)
    +GetDetails() string
}

class Scroll {
  +LengthMillimeters: int
  +Scroll(title: string, lengthMillimeters: int)
  +GetDetails() string
}

PaperbackBook --|> Publication : "is-a"
Scroll --|> Publication : "is-a"

```