import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates a "set brightness" operation for a light device.
 * Demonstrates parameterized commands — the brightness value
 * is bound at command construction time.
 */
export class SetBrightnessCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  /**
   * @param device - The receiver light device.
   * @param brightness - Target brightness percentage (0–100).
   */
  constructor(
    private readonly device: Device,
    private readonly brightness: number,
  ) {
    this.label = `SetBrightness(${brightness}%): ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.type !== 'Light') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: `SetBrightness(${this.brightness})`,
        success: false,
        message: `Device is not a light (type: ${this.device.type})`,
      };
    }

    this.device.brightness = this.brightness;
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: `SetBrightness(${this.brightness})`,
      success: true,
      message: `Brightness set to ${this.brightness}%`,
    };
  }
}
