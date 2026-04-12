import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates a "turn off" operation for a single device.
 */
export class TurnOffCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  constructor(private readonly device: Device) {
    this.label = `TurnOff: ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.state === 'off') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: 'TurnOff',
        success: true,
        message: 'Already off (no-op)',
      };
    }

    this.device.state = 'off';
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: 'TurnOff',
      success: true,
      message: 'Turned off',
    };
  }
}
