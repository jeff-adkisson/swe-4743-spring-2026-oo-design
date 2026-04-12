import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates a "lock" operation for a door lock device.
 */
export class LockCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  constructor(private readonly device: Device) {
    this.label = `Lock: ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.state === 'locked') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: 'Lock',
        success: true,
        message: 'Already locked (no-op)',
      };
    }

    this.device.state = 'locked';
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: 'Lock',
      success: true,
      message: 'Locked',
    };
  }
}
