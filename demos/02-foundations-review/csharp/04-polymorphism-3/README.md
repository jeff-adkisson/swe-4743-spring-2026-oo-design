# Polymorphism 3

## How To Run
`dotnet run Program-04.cs`

_Note: Requires C# 10 to run from the command line without a `.csproj` file._

## Class Diagram

```mermaid
classDiagram
direction BT
    class ICollectionItem {
	    +Title: string
	    +GetDetails() string
    }

    class IPublication {
	    +EstimateReadingMinutes() int
    }

    class IArtifact {
	    +Culture: string
	    +Material: string
    }

    class PaperbackBook {
	    +Title: string
	    +Pages: int
	    +PaperbackBook(title: string, pages: int)
	    +GetDetails() string
	    +EstimateReadingMinutes() int
    }

    class Scroll {
	    +Title: string
	    +LengthMillimeters: int
	    +Scroll(title: string, lengthMillimeters: int)
	    +GetDetails() string
	    +EstimateReadingMinutes() int
    }

    class AudioBook {
	    +Title: string
	    +DurationMinutes: int
	    +AudioBook(title: string, durationMinutes: int)
	    +GetDetails() string
	    +EstimateReadingMinutes() int
    }

    class Jewelry {
	    +Title: string
	    +Culture: string
	    +Material: string
	    +Jewelry(title: string, culture: string, material: string)
	    +GetDetails() string
    }

    class Container {
	    +Title: string
	    +Culture: string
	    +Material: string
	    +Container(title: string, culture: string, material: string)
	    +GetDetails() string
    }

    class Sculpture {
	    +Title: string
	    +Culture: string
	    +Material: string
	    +Sculpture(title: string, culture: string, material: string)
	    +GetDetails() string
    }

	<<interface>> ICollectionItem
	<<interface>> IPublication
	<<interface>> IArtifact

    IPublication --|> ICollectionItem : "is-a"
    IArtifact --|> ICollectionItem : "is-a"
    PaperbackBook ..|> IPublication : "implements"
    Scroll ..|> IPublication : "implements"
    AudioBook ..|> IPublication : "implements"
    Jewelry ..|> IArtifact : "implements"
    Container ..|> IArtifact : "implements"
    Sculpture ..|> IArtifact : "implements"
    ```