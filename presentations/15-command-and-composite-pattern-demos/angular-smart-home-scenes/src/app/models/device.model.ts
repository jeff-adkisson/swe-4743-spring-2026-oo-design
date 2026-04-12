/**
 * Represents a smart home device in the simulator.
 *
 * Each device has a current state (e.g., 'on', 'off', 'locked', 'unlocked')
 * and optional properties like brightness and color.
 *
 * In the Command pattern, devices act as **Receivers** — they perform
 * the actual work when a command is executed.
 */
export interface Device {
  /** Unique identifier for this device. */
  id: string;

  /** Human-readable name (e.g., "Porch Light", "Front Door Lock"). */
  name: string;

  /** Device type for group resolution (e.g., "Light", "DoorLock", "Thermostat"). */
  type: string;

  /** Physical location in the home (e.g., "Living Room", "Kitchen"). */
  location: string;

  /** Current device state. Mutable — commands change this value. */
  state: string;

  /** Current brightness percentage (0–100). Only meaningful for lights. */
  brightness?: number;

  /** Current color as a hex string. Only meaningful for lights. */
  color?: string;

  /** Current temperature setpoint. Only meaningful for thermostats. */
  temperature?: number;
}
