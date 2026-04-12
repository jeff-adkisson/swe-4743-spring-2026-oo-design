import { Component, input } from '@angular/core';
import { CommandResult } from '../../models';

/**
 * Execution Log Component
 *
 * Displays per-device results after a scene has been executed.
 *
 * This component demonstrates the Command pattern's result reporting:
 * each leaf command produces a CommandResult, and the composite
 * aggregates them. The log shows every individual outcome, making
 * partial failures visible.
 */
@Component({
  selector: 'app-execution-log',
  standalone: true,
  template: `
    <div class="panel">
      <div class="panel-header">Execution Log</div>
      @if (result(); as r) {
        <div class="summary" [class.all-success]="r.success" [class.has-failures]="!r.success">
          {{ r.deviceName }}: {{ r.message }}
        </div>
        @if (r.children && r.children.length > 0) {
          <div class="results">
            @for (child of flattenResults(r.children); track $index) {
              <div class="result-row" [class]="getResultClass(child)">
                <span class="result-icon">{{ getResultIcon(child) }}</span>
                <span class="result-device">{{ child.deviceName }}</span>
                <span class="result-op">{{ child.operation }}</span>
                <span class="result-msg">{{ child.message }}</span>
              </div>
            }
          </div>
        }
      } @else {
        <p class="empty-message">Execute a scene to see results here.</p>
      }
    </div>
  `,
  styles: `
    .empty-message {
      color: var(--text-secondary);
      font-style: italic;
      margin: 0;
    }

    .summary {
      padding: 0.5rem 0.75rem;
      border-radius: 6px;
      font-weight: 600;
      margin-bottom: 0.75rem;
    }

    .all-success {
      background: #dcfce7;
      color: #166534;
    }

    .has-failures {
      background: #fef2f2;
      color: #991b1b;
    }

    .results {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .result-row {
      display: grid;
      grid-template-columns: 1.5rem 1fr 1fr 2fr;
      gap: 0.5rem;
      padding: 0.35rem 0.5rem;
      border-radius: 4px;
      font-size: 0.85rem;
      align-items: center;
    }

    .result-success {
      background: #f0fdf4;
    }

    .result-noop {
      background: #fffbeb;
    }

    .result-failure {
      background: #fef2f2;
    }

    .result-icon {
      text-align: center;
    }

    .result-device {
      font-weight: 500;
    }

    .result-op {
      color: var(--text-secondary);
      font-family: monospace;
      font-size: 0.8rem;
    }

    .result-msg {
      color: var(--text-secondary);
    }
  `,
})
export class ExecutionLogComponent {
  /** The aggregate result from executing a scene's command tree. */
  readonly result = input<CommandResult | null>(null);

  /**
   * Flatten nested composite results into a flat list for display.
   * Composite results contain children which may themselves be composites.
   */
  flattenResults(results: CommandResult[]): CommandResult[] {
    const flat: CommandResult[] = [];
    for (const r of results) {
      if (r.children && r.children.length > 0) {
        flat.push(...this.flattenResults(r.children));
      } else {
        flat.push(r);
      }
    }
    return flat;
  }

  /** Determine the CSS class based on the result outcome. */
  getResultClass(result: CommandResult): string {
    if (!result.success) return 'result-row result-failure';
    if (result.message.includes('no-op') || result.message.includes('Already')) {
      return 'result-row result-noop';
    }
    return 'result-row result-success';
  }

  /** Get the appropriate icon for a result. */
  getResultIcon(result: CommandResult): string {
    if (!result.success) return '\u274c'; // red X
    if (result.message.includes('no-op') || result.message.includes('Already')) {
      return '\u26a0'; // warning
    }
    return '\u2705'; // green check
  }
}
