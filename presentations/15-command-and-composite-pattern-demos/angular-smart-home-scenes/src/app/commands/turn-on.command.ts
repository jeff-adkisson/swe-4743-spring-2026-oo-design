import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates a "turn on" operation for a single device.
 * The device is the **Receiver** — it performs the actual state change.
 *
 * This is also a **Leaf** in the Composite pattern: it has no children
 * and performs work directly.
 */
export class TurnOnCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  /**
   * @param device - The receiver device to turn on.
   */
  constructor(private readonly device: Device) {
    this.label = `TurnOn: ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.state === 'on') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: 'TurnOn',
        success: true,
        message: 'Already on (no-op)',
      };
    }

    this.device.state = 'on';
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: 'TurnOn',
      success: true,
      message: 'Turned on',
    };
  }
}
