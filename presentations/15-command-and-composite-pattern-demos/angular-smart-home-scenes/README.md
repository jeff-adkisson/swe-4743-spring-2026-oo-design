# Smart Home Scene Executor — Angular Demo

**Companion demo for Lecture 15: Command and Composite Patterns**

This Angular application demonstrates the Command and Composite design patterns by implementing a smart home scene executor. Scenes are named presets (e.g., "Evening Arrival," "Movie Mode") that execute a batch of device operations as a single unit.

The demo is 100% client-side — no backend required. Device data and scene definitions are loaded from a static JSON file.

## Quick Start

```bash
npm install
ng serve
```

Open [http://localhost:4200](http://localhost:4200).

## What This Demo Shows

| Feature | Pattern Demonstrated |
|---|---|
| Each device operation is a class implementing `IDeviceCommand` | **Command** — encapsulate operations as objects |
| `CompositeCommand` holds children and delegates `execute()` | **Composite** — uniform treatment of leaf and group |
| PrimeNG Tree visualizes the command tree | **Composite** — recursive tree structure made visible |
| Group actions resolve to individual device commands at runtime | Runtime target resolution |
| Execution continues on partial failure | Partial failure tolerance |
| Device dashboard updates after execution | Commands mutating receiver state |

## Architecture

```
src/app/
├── models/                    # Domain interfaces
│   ├── device.model.ts        # Device (the Receiver)
│   └── scene.model.ts         # SceneDefinition, SceneAction, CommandResult
├── commands/                  # Command + Composite pattern implementation
│   ├── device-command.interface.ts   # IDeviceCommand (Component interface)
│   ├── composite-command.ts          # CompositeCommand (Composite)
│   ├── turn-on.command.ts            # Leaf command
│   ├── turn-off.command.ts           # Leaf command
│   ├── set-brightness.command.ts     # Leaf command (parameterized)
│   ├── set-temperature.command.ts    # Leaf command (parameterized)
│   ├── lock.command.ts               # Leaf command
│   └── unlock.command.ts             # Leaf command
├── services/
│   ├── device-registry.service.ts    # Manages devices (Receivers)
│   ├── scene-resolver.service.ts     # Builds command tree (Client)
│   └── scene-executor.service.ts     # Runs command tree (Invoker)
├── components/
│   ├── scene-list/            # Scene selection UI
│   ├── scene-tree/            # PrimeNG Tree visualization
│   ├── execution-log/         # Per-device result display
│   └── device-dashboard/      # Current device states
└── app.ts                     # Root component (orchestration)
```

## Pattern Role Mapping

### Command Pattern

```
IDeviceCommand          → Command interface
TurnOnCommand, etc.     → ConcreteCommand (Leaf)
Device                  → Receiver
SceneExecutorService    → Invoker
SceneResolverService    → Client (creates commands, binds receivers)
```

### Composite Pattern

```
IDeviceCommand          → Component (shared interface)
TurnOnCommand, etc.     → Leaf (terminal nodes)
CompositeCommand        → Composite (holds children, delegates execute())
PrimeNG Tree            → Visualization of the composite structure
```

## Demo Flow

1. **Select a scene** from the left panel — the command tree is previewed in the center.
2. **Click "Execute Scene"** — the command tree is built, executed, and results appear in the execution log.
3. **Observe the device dashboard** (right panel) — device states update after execution.
4. **Click "Reset Devices"** to restore initial states and re-run scenes.

## Data

Scene definitions and device data are in `public/scene-data.json`. The demo includes four scenes:

- **Evening Arrival** — Turn on porch light, turn on living room lights at 40% brightness, unlock front door.
- **Movie Mode** — Dim living room to 20%, turn off kitchen lights, lock front door.
- **Lock Down** — Lock all doors, turn off all lights.
- **Good Night** — Lock all doors, turn off all lights, set thermostat to 68°F.

## Technology

- Angular 19+ with standalone components and signals
- PrimeNG 21 for the Tree component
- TypeScript with strict mode
- No backend — all logic runs in the browser
