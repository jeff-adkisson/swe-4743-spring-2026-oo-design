import { Injectable } from '@angular/core';
import { Device } from '../models';

/**
 * Device Registry Service
 *
 * Holds the in-memory collection of smart home devices and provides
 * lookup methods for the scene resolver.
 *
 * In the Command pattern, this service manages the **Receivers** —
 * the devices that commands operate on.
 *
 * In a real application, this would query a backend API or database.
 * Here, it loads from a static JSON file to keep the demo self-contained.
 */
@Injectable({ providedIn: 'root' })
export class DeviceRegistryService {
  private devices: Device[] = [];

  /**
   * Initialize the registry with device data.
   * Called once at application startup.
   */
  loadDevices(devices: Device[]): void {
    // Deep copy to avoid mutating the original JSON data
    this.devices = devices.map((d) => ({ ...d }));
  }

  /**
   * Get all devices in the registry.
   * Returns a read-only snapshot for dashboard display.
   */
  getAll(): readonly Device[] {
    return this.devices;
  }

  /**
   * Find a device by its unique ID.
   * Used when a scene action targets a specific device.
   */
  getById(id: string): Device | undefined {
    return this.devices.find((d) => d.id === id);
  }

  /**
   * Find devices matching a type and optional location.
   *
   * This is the **runtime resolution** method used by the scene resolver.
   * Group targets (e.g., "all Lights in Living Room") are resolved here
   * at execution time, so newly added devices are automatically included.
   *
   * @param type - Device type to match (e.g., "Light", "DoorLock").
   * @param location - Optional location filter. If omitted, matches all locations.
   */
  findByTypeAndLocation(type: string, location?: string): Device[] {
    return this.devices.filter((d) => {
      const typeMatch = d.type === type;
      const locationMatch = !location || d.location === location;
      return typeMatch && locationMatch;
    });
  }

  /**
   * Reset all devices to their initial state.
   * Useful for re-running demos without reloading the page.
   */
  resetDevices(devices: Device[]): void {
    this.devices = devices.map((d) => ({ ...d }));
  }
}
