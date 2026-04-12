import { CommandResult } from '../models';

/**
 * Command Pattern — Command Interface
 *
 * This is the **Component** interface shared by both the Command and
 * Composite patterns in this demo:
 *
 * - **Command pattern role**: defines the execution contract that all
 *   concrete commands must honor.
 * - **Composite pattern role**: serves as the Component interface that
 *   both Leaf commands and CompositeCommand implement.
 *
 * The invoker (SceneExecutorService) depends only on this interface,
 * following the Dependency Inversion Principle (DIP).
 */
export interface IDeviceCommand {
  /**
   * Execute this command and return the result.
   *
   * For leaf commands: performs a single device operation.
   * For composite commands: delegates to children and aggregates results.
   */
  execute(): CommandResult;

  /**
   * Human-readable label for display in the tree visualization.
   * Used by the PrimeNG Tree component to label each node.
   */
  readonly label: string;

  /**
   * Whether this command is a composite (has children).
   * Used to distinguish leaves from composites in the tree UI.
   */
  readonly isComposite: boolean;
}
