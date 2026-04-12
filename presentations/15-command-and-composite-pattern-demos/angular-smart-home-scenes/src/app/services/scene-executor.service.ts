import { Injectable } from '@angular/core';
import { SceneDefinition, CommandResult } from '../models';
import { CompositeCommand } from '../commands';
import { SceneResolverService } from './scene-resolver.service';

/**
 * Scene Executor Service
 *
 * The **Invoker** in the Command pattern.
 *
 * This service orchestrates scene execution:
 * 1. Accepts a scene definition.
 * 2. Delegates to SceneResolverService to build the command tree.
 * 3. Calls execute() on the root CompositeCommand.
 * 4. Returns the execution results.
 *
 * The invoker does not know what commands it is executing.
 * It depends only on IDeviceCommand (through the CompositeCommand).
 * This is the Dependency Inversion Principle in action.
 */
@Injectable({ providedIn: 'root' })
export class SceneExecutorService {
  constructor(private readonly resolver: SceneResolverService) {}

  /**
   * Resolve and execute a scene definition.
   *
   * @param scene - The scene to execute.
   * @returns An object containing the built command tree and the execution result.
   */
  execute(scene: SceneDefinition): SceneExecutionResult {
    // Step 1: Resolve the scene definition into a command tree
    const commandTree = this.resolver.resolve(scene);

    // Step 2: Execute the entire tree (Composite pattern handles delegation)
    const result = commandTree.execute();

    return { commandTree, result };
  }

  /**
   * Build the command tree without executing it.
   * Used by the tree visualization to show the structure before execution.
   *
   * @param scene - The scene to resolve.
   * @returns The composite command tree.
   */
  preview(scene: SceneDefinition): CompositeCommand {
    return this.resolver.resolve(scene);
  }
}

/**
 * The result of executing a scene, including both the command tree
 * (for visualization) and the aggregate result (for the execution log).
 */
export interface SceneExecutionResult {
  /** The composite command tree that was built and executed. */
  commandTree: CompositeCommand;

  /** The aggregate result from executing the tree. */
  result: CommandResult;
}
