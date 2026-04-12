import { Component, input, output } from '@angular/core';
import { SceneDefinition } from '../../models';

/**
 * Scene List Component
 *
 * Displays all available scene definitions and allows the user to
 * select a scene for preview or execution.
 *
 * This component is purely presentational — it emits events upward
 * and lets the parent coordinate execution.
 */
@Component({
  selector: 'app-scene-list',
  standalone: true,
  template: `
    <div class="panel">
      <div class="panel-header">Available Scenes</div>
      <div class="scene-cards">
        @for (scene of scenes(); track scene.id) {
          <div
            class="scene-card"
            [class.selected]="selectedSceneId() === scene.id"
            (click)="selectScene.emit(scene)"
          >
            <div class="scene-name">{{ scene.name }}</div>
            <div class="scene-desc">{{ scene.description }}</div>
            <div class="scene-actions-count">
              {{ scene.actions.length }} action{{ scene.actions.length !== 1 ? 's' : '' }}
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: `
    .scene-cards {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .scene-card {
      padding: 0.75rem 1rem;
      border: 1px solid var(--panel-border);
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.15s ease;

      &:hover {
        border-color: #93c5fd;
        background: #eff6ff;
      }

      &.selected {
        border-color: #3b82f6;
        background: #eff6ff;
        box-shadow: 0 0 0 1px #3b82f6;
      }
    }

    .scene-name {
      font-weight: 600;
      margin-bottom: 0.25rem;
    }

    .scene-desc {
      font-size: 0.85rem;
      color: var(--text-secondary);
      margin-bottom: 0.25rem;
    }

    .scene-actions-count {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }
  `,
})
export class SceneListComponent {
  /** All available scene definitions. */
  readonly scenes = input.required<SceneDefinition[]>();

  /** The currently selected scene ID (for highlighting). */
  readonly selectedSceneId = input<string | null>(null);

  /** Emitted when the user clicks a scene card. */
  readonly selectScene = output<SceneDefinition>();
}
