// =============================================================================
// TRAFFIC LIGHT STATE MACHINE DEMO
// =============================================================================
//
// This is a single-file demo of the State Machine (aka State) design pattern,
// using a traffic light as the domain model.
//
// KEY PATTERN CONCEPTS ILLUSTRATED:
//
//   1. State Interface (ITrafficLightState) — defines the contract every
//      concrete state must implement: handle events and expose metadata.
//
//   2. Concrete States (RedState, GreenState, YellowState, WalkState) —
//      each encapsulates the behavior and transition logic for one state.
//      Transition rules live *inside* the state, not in a giant switch/if block.
//
//   3. Context (TrafficLightController) — holds a reference to the current
//      state and delegates event handling to it. External code interacts with
//      the context, never directly with state objects.
//
//   4. Events — two things can happen in this system:
//        • PhaseComplete  — the timer for the current state expires.
//        • WalkButtonPressed — a pedestrian requests a walk signal.
//      Each state decides independently how to respond to each event.
//
// STATE DIAGRAM:
//
//        ┌───────────────────────────────────────────────┐
//        │                                               │
//      [Red] ──(phase complete, no walk)──► [Green]      │
//        │                                    │          │
//        │ (phase complete, walk requested)   │ (phase   │
//        ▼                                    │ complete)│
//      [Walk] ──(phase complete)──► [Green]   │          │
//                                             ▼          │
//                                          [Yellow] ─────┘
//
//   Walk button can be pressed in any state. It is *deferred* (queued) and
//   honored only during the Red → next-state transition.
//
// HOW TO RUN:
//
//   Option A — run as a standalone script (requires .NET 9+ SDK):
//     $ dotnet run 12-state-machine-pattern-traffic-light-demo.cs
//
//   Option B — create a project and run:
//     $ mkdir TrafficLightDemo && cd TrafficLightDemo
//     $ dotnet new console --force
//     $ cp <this-file> Program.cs
//     $ dotnet run
//
// INTERACTION:
//   • Press W at any time to request a pedestrian walk signal.
//   • Press Q to quit.
//
// =============================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficLightStateMachineDemo
{
    public static class Program
    {
        public static async Task Main()
        {
            using var cts = new CancellationTokenSource();
            var controller = new TrafficLightController();

            Console.Clear();
            Console.WriteLine("TRAFFIC LIGHT INTERACTIVE STATE MACHINE DEMO");
            Console.WriteLine("--- Press W to request walk.");
            Console.WriteLine("--- Press Q to quit.");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine();

            var inputTask = Task.Run(() => ListenForInput(controller, cts));
            var simulationTask = RunSimulationAsync(controller, cts.Token);

            await simulationTask;
            await inputTask;
        }

        private static async Task RunSimulationAsync(
            TrafficLightController controller,
            CancellationToken cancellationToken)
        {
            controller.PrintStatus();

            while (!cancellationToken.IsCancellationRequested)
            {
                var duration = controller.GetCurrentStateDuration();

                for (int remaining = duration; remaining > 0; remaining--)
                {
                    controller.PrintCountdown(remaining);

                    try
                    {
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }

                controller.OnPhaseComplete();
                controller.PrintStatus();
            }
        }

        private static void ListenForInput(
            TrafficLightController controller,
            CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.W:
                        controller.PressWalkButton();
                        break;

                    case ConsoleKey.Q:
                        Console.WriteLine("Quitting...");
                        cts.Cancel();
                        break;
                }
            }
        }
    }

    // -------------------------------------------------------------------------
    // Enum used for display purposes (colored console output).
    // -------------------------------------------------------------------------
    public enum StateName
    {
        Red,
        Yellow,
        Green,
        Walk
    }

    // -------------------------------------------------------------------------
    // CONTEXT — The object whose behavior changes based on its current state.
    //
    // The controller does NOT contain transition logic. It delegates to the
    // current state object. This is the core benefit of the pattern: adding a
    // new state requires a new class, not editing a central switch statement.
    // -------------------------------------------------------------------------
    public sealed class TrafficLightController
    {
        private readonly object _lock = new();
        private ITrafficLightState _currentState;

        public TrafficLightController()
        {
            _currentState = new RedState();
        }

        public bool WalkButtonPressed { get; private set; }

        public StateName CurrentState
        {
            get
            {
                lock (_lock)
                {
                    return _currentState.Name;
                }
            }
        }

        public void PressWalkButton()
        {
            lock (_lock)
            {
                Console.WriteLine("Event: Walk button pressed.");
                _currentState.HandleWalkButtonPressed(this);
            }
        }

        public void OnPhaseComplete()
        {
            lock (_lock)
            {
                Console.WriteLine("Event: Phase complete.");
                _currentState.HandlePhaseComplete(this);
            }
        }

        public void RegisterWalkRequest()
        {
            WalkButtonPressed = true;
        }

        public void ClearWalkRequest()
        {
            WalkButtonPressed = false;
        }

        public void TransitionTo(ITrafficLightState newState)
        {
            Console.Write("Transition: ");
            WriteStateName(_currentState.Name);
            Console.Write(" -> ");
            WriteStateName(newState.Name);
            Console.WriteLine();
            Console.WriteLine();

            _currentState = newState;
        }

        public int GetCurrentStateDuration()
        {
            lock (_lock)
            {
                return _currentState.DurationSeconds;
            }
        }

        public void PrintStatus()
        {
            lock (_lock)
            {
                Console.Write("Current State: ");
                WriteStateName(_currentState.Name);
                Console.WriteLine($", Walk Requested: {WalkButtonPressed}");
                Console.WriteLine(new string('-', 50));
            }
        }

        public void PrintCountdown(int remainingSeconds)
        {
            lock (_lock)
            {
                Console.Write("State: ");
                WriteStateName(_currentState.Name);
                Console.WriteLine(
                    $" | Walk Requested: {WalkButtonPressed,-5} | Next transition in: {remainingSeconds} second(s)");
            }
        }

        private static void WriteStateName(StateName stateName)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = GetConsoleColor(stateName);
            Console.Write(stateName);
            Console.ForegroundColor = previousColor;
        }

        private static ConsoleColor GetConsoleColor(StateName stateName)
        {
            return stateName switch
            {
                StateName.Red => ConsoleColor.Red,
                StateName.Yellow => ConsoleColor.Yellow,
                StateName.Green => ConsoleColor.Green,
                StateName.Walk => ConsoleColor.White,
                _ => Console.ForegroundColor
            };
        }
    }

    // -------------------------------------------------------------------------
    // STATE INTERFACE — Every concrete state must implement this.
    //
    // Each state exposes:
    //   • Name / DurationSeconds — metadata used by the simulation loop.
    //   • HandlePhaseComplete    — what happens when this state's timer expires.
    //   • HandleWalkButtonPressed — what happens when a pedestrian presses W.
    //
    // Notice that the handler methods receive the controller (context) so
    // they can call TransitionTo() to change the current state.
    // -------------------------------------------------------------------------
    public interface ITrafficLightState
    {
        StateName Name { get; }
        int DurationSeconds { get; }

        void HandlePhaseComplete(TrafficLightController controller);
        void HandleWalkButtonPressed(TrafficLightController controller);
    }

    // -------------------------------------------------------------------------
    // CONCRETE STATES — Each class below is one state in the machine.
    //
    // The transition logic is the interesting part: look at HandlePhaseComplete
    // in each class to see how it decides which state to move to next.
    // -------------------------------------------------------------------------

    // Red: the only state that checks for a pending walk request.
    // If walk was requested, it transitions to Walk instead of Green.
    public sealed class RedState : ITrafficLightState
    {
        public StateName Name => StateName.Red;
        public int DurationSeconds => 5;

        public void HandlePhaseComplete(TrafficLightController controller)
        {
            if (controller.WalkButtonPressed)
            {
                controller.ClearWalkRequest();
                controller.TransitionTo(new WalkState());
            }
            else
            {
                controller.TransitionTo(new GreenState());
            }
        }

        public void HandleWalkButtonPressed(TrafficLightController controller)
        {
            controller.RegisterWalkRequest();
            Console.WriteLine("Walk request registered. It will be honored when Red completes.");
        }
    }

    // Walk: a transient state that always goes to Green when done.
    // Pressing the walk button again while already walking is ignored.
    public sealed class WalkState : ITrafficLightState
    {
        public StateName Name => StateName.Walk;
        public int DurationSeconds => 4;

        public void HandlePhaseComplete(TrafficLightController controller)
        {
            controller.TransitionTo(new GreenState());
        }

        public void HandleWalkButtonPressed(TrafficLightController controller)
        {
            Console.WriteLine("Walk button ignored because Walk is already active.");
        }
    }

    // Green: always transitions to Yellow. Walk requests are deferred.
    public sealed class GreenState : ITrafficLightState
    {
        public StateName Name => StateName.Green;
        public int DurationSeconds => 5;

        public void HandlePhaseComplete(TrafficLightController controller)
        {
            controller.TransitionTo(new YellowState());
        }

        public void HandleWalkButtonPressed(TrafficLightController controller)
        {
            controller.RegisterWalkRequest();
            Console.WriteLine("Walk request registered. It will be honored on the next Red cycle.");
        }
    }

    // Yellow: always transitions to Red. Walk requests are deferred.
    public sealed class YellowState : ITrafficLightState
    {
        public StateName Name => StateName.Yellow;
        public int DurationSeconds => 3;

        public void HandlePhaseComplete(TrafficLightController controller)
        {
            controller.TransitionTo(new RedState());
        }

        public void HandleWalkButtonPressed(TrafficLightController controller)
        {
            controller.RegisterWalkRequest();
            Console.WriteLine("Walk request registered. It will be honored when Red completes.");
        }
    }
}