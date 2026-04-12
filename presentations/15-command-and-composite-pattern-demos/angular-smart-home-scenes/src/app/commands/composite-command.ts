import { CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Composite Pattern — Composite Node
 *
 * CompositeCommand implements the same IDeviceCommand interface as
 * leaf commands, but holds an ordered list of children. When executed,
 * it delegates to each child in order and aggregates the results.
 *
 * This is the defining characteristic of the Composite pattern:
 * the composite IS-A component. Callers (the invoker / SceneExecutor)
 * do not know or care whether they are executing a single command
 * or an entire tree.
 *
 * In the scene domain, a CompositeCommand represents either:
 * - The entire scene (root composite)
 * - A group of commands targeting the same device group
 *
 * Partial failure handling: execution continues even if individual
 * child commands fail. The aggregate result reports per-device outcomes.
 */
export class CompositeCommand implements IDeviceCommand {
  readonly isComposite = true;
  readonly label: string;
  private readonly children: IDeviceCommand[] = [];

  /**
   * @param name - Human-readable name for this composite (e.g., scene name or group name).
   */
  constructor(name: string) {
    this.label = name;
  }

  /**
   * Add a child command to this composite.
   * Children are executed in the order they are added.
   *
   * @param child - A leaf command or another composite.
   */
  add(child: IDeviceCommand): void {
    this.children.push(child);
  }

  /**
   * Returns a read-only view of the children.
   * Used by the tree visualization component.
   */
  getChildren(): readonly IDeviceCommand[] {
    return this.children;
  }

  /**
   * Execute all children in order and aggregate results.
   *
   * Per the project specification, execution does NOT abort on
   * partial failure — every child executes regardless of prior results.
   */
  execute(): CommandResult {
    const childResults: CommandResult[] = this.children.map((child) =>
      child.execute(),
    );

    const succeeded = childResults.filter((r) => r.success).length;
    const failed = childResults.filter((r) => !r.success).length;

    return {
      deviceId: this.label,
      deviceName: this.label,
      operation: 'CompositeExecute',
      success: failed === 0,
      message: `${succeeded} succeeded, ${failed} failed`,
      children: childResults,
    };
  }
}
