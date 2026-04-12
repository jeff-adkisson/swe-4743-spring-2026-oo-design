import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates an "unlock" operation for a door lock device.
 */
export class UnlockCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  constructor(private readonly device: Device) {
    this.label = `Unlock: ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.state === 'unlocked') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: 'Unlock',
        success: true,
        message: 'Already unlocked (no-op)',
      };
    }

    this.device.state = 'unlocked';
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: 'Unlock',
      success: true,
      message: 'Unlocked',
    };
  }
}
