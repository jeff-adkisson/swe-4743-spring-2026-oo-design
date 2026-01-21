# Crazy Eights (Java)

Java port of the Crazy Eights C# solution with the same gameplay and structure.

## Layout

- Source: `src-java/src/main/java/edu/kennesaw/crazy8s`
- Entry point: `edu.kennesaw.crazy8s.Main`

## Run (CLI)

From `assignment-1/solution./src-java`:

```bash
javac -d out $(find src/main/java -name "*.java")
java -cp out edu.kennesaw.crazy8s.Main [randomSeed] [handSize] [showAllHands]
```

Arguments (optional):
- `randomSeed` (int) - use 0 for a random seed.
- `handSize` (int) - starting cards per player.
- `showAllHands` (true/false) - reveal all hands.

## Run (Docker)

From `assignment-1/solution./src-java`:

```bash
docker build -t crazy8s-java .
docker run -it --rm crazy8s-java [randomSeed] [handSize] [showAllHands]
```
