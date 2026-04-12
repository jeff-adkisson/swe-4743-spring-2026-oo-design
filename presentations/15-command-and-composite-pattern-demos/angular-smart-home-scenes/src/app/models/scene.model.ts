/**
 * Represents a scene definition — a named preset that stores
 * an ordered list of device actions.
 *
 * Scene definitions are the **data** that drives command tree construction.
 * They describe *what* should happen, not *how* to do it.
 * The Command and Composite patterns handle the "how."
 *
 * This model mirrors the DeviceScene from Lecture 14 (Builder pattern).
 */
export interface SceneDefinition {
  /** Unique identifier for this scene. */
  id: string;

  /** Human-readable scene name (e.g., "Evening Arrival"). */
  name: string;

  /** Description of what this scene does. */
  description: string;

  /** Ordered list of actions to execute. Order matters. */
  actions: SceneAction[];
}

/**
 * A single action within a scene definition.
 *
 * Each action targets either a specific device (by deviceId)
 * or a device group (by type and/or location).
 * Group targets are resolved at execution time — see SceneResolverService.
 */
export interface SceneAction {
  /** Target a specific device by ID. Mutually exclusive with type/location. */
  deviceId?: string;

  /** Target all devices of this type (e.g., "Light"). Used for group targets. */
  deviceType?: string;

  /** Target all devices in this location (e.g., "Living Room"). Used for group targets. */
  location?: string;

  /** The operation to perform (e.g., "TurnOn", "SetBrightness", "Lock"). */
  operation: string;

  /** Operation parameters (e.g., { brightness: "40" }). */
  parameters?: Record<string, string>;
}

/**
 * The result of executing a single command against a device.
 *
 * Every command — whether leaf or composite — produces a CommandResult.
 * This enables per-device result reporting and partial failure tracking.
 */
export interface CommandResult {
  /** The device ID (or composite name) this result applies to. */
  deviceId: string;

  /** Human-readable device name. */
  deviceName: string;

  /** The operation that was performed. */
  operation: string;

  /** Whether the operation succeeded. */
  success: boolean;

  /** Human-readable outcome message. */
  message: string;

  /** Child results (only populated for composite commands). */
  children?: CommandResult[];
}
