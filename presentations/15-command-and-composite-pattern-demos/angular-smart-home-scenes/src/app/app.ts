import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Button } from 'primeng/button';
import { SceneDefinition, CommandResult, Device } from './models';
import { CompositeCommand } from './commands';
import { DeviceRegistryService, SceneExecutorService } from './services';
import { SceneListComponent } from './components/scene-list/scene-list.component';
import { SceneTreeComponent } from './components/scene-tree/scene-tree.component';
import { ExecutionLogComponent } from './components/execution-log/execution-log.component';
import { DeviceDashboardComponent } from './components/device-dashboard/device-dashboard.component';

/**
 * Root Application Component
 *
 * Orchestrates the scene executor demo by:
 * 1. Loading fake scene and device data from a JSON file.
 * 2. Coordinating scene selection, preview, and execution.
 * 3. Displaying results across the four child components.
 *
 * This component acts as the **Client** in the Command pattern —
 * it creates the scenario and triggers the Invoker (SceneExecutorService).
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    Button,
    SceneListComponent,
    SceneTreeComponent,
    ExecutionLogComponent,
    DeviceDashboardComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  /** All available scene definitions loaded from JSON. */
  readonly scenes = signal<SceneDefinition[]>([]);

  /** The currently selected scene. */
  readonly selectedScene = signal<SceneDefinition | null>(null);

  /** The command tree built from the selected scene (for preview). */
  readonly commandTree = signal<CompositeCommand | null>(null);

  /** The result from the last scene execution. */
  readonly executionResult = signal<CommandResult | null>(null);

  /** All devices in the registry (for dashboard display). */
  readonly devices = signal<readonly Device[]>([]);

  /** Raw device data for reset purposes. */
  private initialDevices: Device[] = [];

  constructor(
    private readonly http: HttpClient,
    private readonly deviceRegistry: DeviceRegistryService,
    private readonly sceneExecutor: SceneExecutorService,
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  /**
   * Load scene definitions and device data from the static JSON file.
   */
  private loadData(): void {
    this.http
      .get<{ devices: Device[]; scenes: SceneDefinition[] }>('scene-data.json')
      .subscribe((data) => {
        this.initialDevices = data.devices;
        this.deviceRegistry.loadDevices(data.devices);
        this.scenes.set(data.scenes);
        this.devices.set(this.deviceRegistry.getAll());
      });
  }

  /**
   * Handle scene selection — preview the command tree without executing.
   */
  onSelectScene(scene: SceneDefinition): void {
    this.selectedScene.set(scene);
    this.executionResult.set(null);

    // Build the command tree for preview (Composite pattern visualization)
    const tree = this.sceneExecutor.preview(scene);
    this.commandTree.set(tree);
  }

  /**
   * Execute the currently selected scene.
   *
   * This triggers the full Command + Composite pipeline:
   * 1. SceneResolverService resolves targets and builds the command tree.
   * 2. CompositeCommand.execute() delegates to all leaf commands.
   * 3. Each leaf command mutates its receiver device.
   * 4. Results are aggregated and displayed.
   */
  onExecuteScene(): void {
    const scene = this.selectedScene();
    if (!scene) return;

    const { commandTree, result } = this.sceneExecutor.execute(scene);
    this.commandTree.set(commandTree);
    this.executionResult.set(result);

    // Refresh the device list to reflect state changes
    this.devices.set([...this.deviceRegistry.getAll()]);
  }

  /**
   * Reset all devices to their initial state.
   * Useful for re-running demos.
   */
  onResetDevices(): void {
    this.deviceRegistry.resetDevices(this.initialDevices);
    this.devices.set(this.deviceRegistry.getAll());
    this.executionResult.set(null);

    // Re-preview the selected scene with fresh device state
    const scene = this.selectedScene();
    if (scene) {
      this.commandTree.set(this.sceneExecutor.preview(scene));
    }
  }
}
