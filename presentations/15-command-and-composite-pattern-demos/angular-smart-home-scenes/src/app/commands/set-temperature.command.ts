import { Device, CommandResult } from '../models';
import { IDeviceCommand } from './device-command.interface';

/**
 * Command Pattern — Concrete Command (Leaf)
 *
 * Encapsulates a "set temperature" operation for a thermostat device.
 * Demonstrates parameterized commands with a numeric value.
 */
export class SetTemperatureCommand implements IDeviceCommand {
  readonly label: string;
  readonly isComposite = false;

  /**
   * @param device - The receiver thermostat device.
   * @param temperature - Target temperature in degrees Fahrenheit.
   */
  constructor(
    private readonly device: Device,
    private readonly temperature: number,
  ) {
    this.label = `SetTemperature(${temperature}°F): ${device.name}`;
  }

  execute(): CommandResult {
    if (this.device.type !== 'Thermostat') {
      return {
        deviceId: this.device.id,
        deviceName: this.device.name,
        operation: `SetTemperature(${this.temperature})`,
        success: false,
        message: `Device is not a thermostat (type: ${this.device.type})`,
      };
    }

    this.device.temperature = this.temperature;
    return {
      deviceId: this.device.id,
      deviceName: this.device.name,
      operation: `SetTemperature(${this.temperature})`,
      success: true,
      message: `Temperature set to ${this.temperature}°F`,
    };
  }
}
