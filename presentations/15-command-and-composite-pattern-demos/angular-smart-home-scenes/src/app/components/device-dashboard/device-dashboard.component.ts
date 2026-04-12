import { Component, input } from '@angular/core';
import { Device } from '../../models';

/**
 * Device Dashboard Component
 *
 * Displays the current state of all devices in the smart home.
 *
 * After a scene is executed, commands mutate device state.
 * This dashboard reflects those changes in real time because
 * Angular's change detection picks up the property mutations.
 *
 * In the Command pattern, these devices are the **Receivers** —
 * the objects that perform the actual work.
 */
@Component({
  selector: 'app-device-dashboard',
  standalone: true,
  template: `
    <div class="panel">
      <div class="panel-header">Device Dashboard (Receivers)</div>
      <div class="device-grid">
        @for (device of devices(); track device.id) {
          <div class="device-card">
            <div class="device-icon">{{ getDeviceIcon(device) }}</div>
            <div class="device-info">
              <div class="device-name">{{ device.name }}</div>
              <div class="device-location">{{ device.location }}</div>
            </div>
            <div class="device-state">
              <span class="badge" [class]="getBadgeClass(device)">
                {{ device.state }}
              </span>
              @if (device.type === 'Light' && device.brightness !== undefined) {
                <span class="device-detail">{{ device.brightness }}%</span>
              }
              @if (device.type === 'Thermostat' && device.temperature !== undefined) {
                <span class="device-detail">{{ device.temperature }}°F</span>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: `
    .device-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: 0.5rem;
    }

    .device-card {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.6rem 0.75rem;
      border: 1px solid var(--panel-border);
      border-radius: 6px;
    }

    .device-icon {
      font-size: 1.5rem;
      width: 2rem;
      text-align: center;
      flex-shrink: 0;
    }

    .device-info {
      flex: 1;
      min-width: 0;
    }

    .device-name {
      font-weight: 500;
      font-size: 0.9rem;
    }

    .device-location {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .device-state {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: 0.2rem;
      flex-shrink: 0;
    }

    .device-detail {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }
  `,
})
export class DeviceDashboardComponent {
  /** All devices in the registry. */
  readonly devices = input.required<readonly Device[]>();

  /** Get an emoji icon based on device type. */
  getDeviceIcon(device: Device): string {
    switch (device.type) {
      case 'Light':
        return device.state === 'on' ? '\uD83D\uDCA1' : '\uD83D\uDD26';
      case 'DoorLock':
        return device.state === 'locked' ? '\uD83D\uDD12' : '\uD83D\uDD13';
      case 'Thermostat':
        return '\uD83C\uDF21\uFE0F';
      case 'Fan':
        return device.state === 'on' ? '\uD83C\uDF00' : '\uD83D\uDCA8';
      default:
        return '\uD83D\uDCF1';
    }
  }

  /** Get the CSS class for the state badge. */
  getBadgeClass(device: Device): string {
    switch (device.state) {
      case 'on':
        return 'badge badge-on';
      case 'off':
        return 'badge badge-off';
      case 'locked':
        return 'badge badge-locked';
      case 'unlocked':
        return 'badge badge-unlocked';
      default:
        return 'badge';
    }
  }
}
