# Namespaces

## How To Run
`dotnet run Program-07.cs`

_Note: Requires C# 10 to run from the command line without a `.csproj` file._

## Class Diagram
```mermaid
classDiagram
direction TB

%% =============================
%% Demo.Main
%% =============================
namespace Demo.Main {
  class Program
  class CollectionInitializer
  class CollectionPrinter
  class ReadingPlanPrinter
  class ShortReadsPrinter
  class ArtifactsByCultureMaterialNamePrinter
}

%% =============================
%% Demo.Entities
%% =============================
namespace Demo.Entities {
  class ICollectionItem
  class IPublication
  class PublicationBase
  class IArtifact

  class PaperbackBook
  class Scroll
  class AudioBook

  class Jewelry
  class Container
  class Sculpture
}

%% =============================
%% Cross-namespace usage (modules depending on other modules)
%% =============================
Demo.Main.Program ..> Demo.Main.CollectionInitializer
Demo.Main.Program ..> Demo.Main.CollectionPrinter
Demo.Main.Program ..> Demo.Main.ReadingPlanPrinter
Demo.Main.Program ..> Demo.Main.ShortReadsPrinter
Demo.Main.Program ..> Demo.Main.ArtifactsByCultureMaterialNamePrinter

Demo.Main.CollectionInitializer ..> Demo.Entities.PaperbackBook
Demo.Main.CollectionInitializer ..> Demo.Entities.Scroll
Demo.Main.CollectionInitializer ..> Demo.Entities.AudioBook
Demo.Main.CollectionInitializer ..> Demo.Entities.Jewelry
Demo.Main.CollectionInitializer ..> Demo.Entities.Container
Demo.Main.CollectionInitializer ..> Demo.Entities.Sculpture

Demo.Main.CollectionInitializer ..> Demo.Entities.ICollectionItem
Demo.Main.CollectionPrinter ..> Demo.Entities.ICollectionItem
Demo.Main.ReadingPlanPrinter ..> Demo.Entities.ICollectionItem
Demo.Main.ShortReadsPrinter ..> Demo.Entities.ICollectionItem
Demo.Main.ArtifactsByCultureMaterialNamePrinter ..> Demo.Entities.ICollectionItem

Demo.Main.ReadingPlanPrinter ..> Demo.Entities.IPublication
Demo.Main.ShortReadsPrinter ..> Demo.Entities.IPublication
Demo.Main.ArtifactsByCultureMaterialNamePrinter ..> Demo.Entities.IArtifact
```