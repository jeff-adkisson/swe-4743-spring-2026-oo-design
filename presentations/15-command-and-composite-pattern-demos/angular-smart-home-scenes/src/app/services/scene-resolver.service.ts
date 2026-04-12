import { Injectable } from '@angular/core';
import { SceneDefinition, SceneAction, Device } from '../models';
import {
  IDeviceCommand,
  CompositeCommand,
  TurnOnCommand,
  TurnOffCommand,
  SetBrightnessCommand,
  SetTemperatureCommand,
  LockCommand,
  UnlockCommand,
} from '../commands';
import { DeviceRegistryService } from './device-registry.service';

/**
 * Scene Resolver Service
 *
 * Transforms a SceneDefinition (data) into a CompositeCommand (behavior).
 *
 * This is where the Command and Composite patterns come together:
 * 1. Each scene action is resolved to one or more target devices.
 * 2. For each device, a concrete command (Leaf) is created.
 * 3. All commands are assembled into a CompositeCommand tree.
 *
 * Group targets are resolved at execution time, not at definition time.
 * This means devices added after a scene was created are automatically
 * included when the scene runs — a key requirement from the project spec.
 *
 * Pattern roles in this class:
 * - Acts as the **Client** in the Command pattern (creates commands and binds receivers).
 * - Builds the **Composite** tree structure.
 */
@Injectable({ providedIn: 'root' })
export class SceneResolverService {
  constructor(private readonly deviceRegistry: DeviceRegistryService) {}

  /**
   * Resolve a scene definition into an executable command tree.
   *
   * @param scene - The scene definition to resolve.
   * @returns A CompositeCommand representing the entire scene.
   */
  resolve(scene: SceneDefinition): CompositeCommand {
    const root = new CompositeCommand(scene.name);

    for (const action of scene.actions) {
      const devices = this.resolveTargets(action);

      if (devices.length === 0) {
        continue; // No matching devices — skip this action
      }

      // If this is a group action, wrap the commands in a sub-composite
      // so the tree visualization shows the grouping clearly
      if (!action.deviceId && devices.length > 1) {
        const groupLabel = this.buildGroupLabel(action);
        const group = new CompositeCommand(groupLabel);

        for (const device of devices) {
          group.add(this.createCommand(device, action));
        }

        root.add(group);
      } else {
        // Single device target — add directly to root
        for (const device of devices) {
          root.add(this.createCommand(device, action));
        }
      }
    }

    return root;
  }

  /**
   * Resolve an action's target(s) to concrete devices.
   *
   * - If the action has a deviceId, look up that specific device.
   * - Otherwise, resolve the device group by type and optional location.
   */
  private resolveTargets(action: SceneAction): Device[] {
    if (action.deviceId) {
      const device = this.deviceRegistry.getById(action.deviceId);
      return device ? [device] : [];
    }

    return this.deviceRegistry.findByTypeAndLocation(
      action.deviceType!,
      action.location,
    );
  }

  /**
   * Create the appropriate concrete command for a device and action.
   *
   * This is the factory method that maps operation strings to
   * Command pattern classes. Each case creates a Leaf command
   * bound to its Receiver (the device).
   */
  private createCommand(device: Device, action: SceneAction): IDeviceCommand {
    switch (action.operation) {
      case 'TurnOn':
        return new TurnOnCommand(device);
      case 'TurnOff':
        return new TurnOffCommand(device);
      case 'SetBrightness':
        return new SetBrightnessCommand(
          device,
          parseInt(action.parameters?.['brightness'] ?? '100', 10),
        );
      case 'SetTemperature':
        return new SetTemperatureCommand(
          device,
          parseInt(action.parameters?.['temperature'] ?? '72', 10),
        );
      case 'Lock':
        return new LockCommand(device);
      case 'Unlock':
        return new UnlockCommand(device);
      default:
        throw new Error(`Unknown operation: ${action.operation}`);
    }
  }

  /**
   * Build a human-readable label for a group composite node.
   */
  private buildGroupLabel(action: SceneAction): string {
    const parts: string[] = [action.operation];
    if (action.deviceType) {
      parts.push(`all ${action.deviceType}s`);
    }
    if (action.location) {
      parts.push(`in ${action.location}`);
    }
    return parts.join(' ');
  }
}
