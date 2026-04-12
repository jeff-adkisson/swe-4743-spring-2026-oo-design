import { Component, input, computed } from '@angular/core';
import { Tree } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { CompositeCommand, IDeviceCommand } from '../../commands';

/**
 * Scene Tree Component
 *
 * Visualizes the Composite pattern's tree structure using PrimeNG's Tree.
 *
 * This component converts the CompositeCommand tree into PrimeNG TreeNode
 * format, making the Composite pattern's recursive part-whole structure
 * directly visible in the UI.
 *
 * Key mapping:
 * - CompositeCommand → folder icon, expandable node with children
 * - Leaf commands → bolt icon, terminal node
 */
@Component({
  selector: 'app-scene-tree',
  standalone: true,
  imports: [Tree],
  template: `
    <div class="panel">
      <div class="panel-header">Command Tree (Composite Pattern)</div>
      @if (treeNodes().length > 0) {
        <p-tree
          [value]="treeNodes()"
          [filter]="false"
          [selectionMode]="null"
          styleClass="scene-tree"
        />
      } @else {
        <p class="empty-message">Select a scene to view its command tree.</p>
      }
    </div>
  `,
  styles: `
    .empty-message {
      color: var(--text-secondary);
      font-style: italic;
      margin: 0;
    }

    :host ::ng-deep .scene-tree {
      border: none;
      padding: 0;
      background: transparent;
    }
  `,
})
export class SceneTreeComponent {
  /** The composite command tree to visualize. */
  readonly commandTree = input<CompositeCommand | null>(null);

  /**
   * Convert the CompositeCommand tree to PrimeNG TreeNode format.
   *
   * This computed signal reacts to changes in the commandTree input
   * and rebuilds the tree node array automatically.
   */
  readonly treeNodes = computed<TreeNode[]>(() => {
    const tree = this.commandTree();
    if (!tree) {
      return [];
    }
    return [this.toTreeNode(tree)];
  });

  /**
   * Recursively convert an IDeviceCommand to a PrimeNG TreeNode.
   *
   * Composites become expandable folder nodes with children.
   * Leaves become terminal bolt nodes.
   */
  private toTreeNode(command: IDeviceCommand): TreeNode {
    if (command.isComposite) {
      const composite = command as CompositeCommand;
      return {
        label: composite.label,
        icon: 'pi pi-folder-open',
        expanded: true,
        children: composite.getChildren().map((c) => this.toTreeNode(c)),
      };
    }

    return {
      label: command.label,
      icon: 'pi pi-bolt',
      leaf: true,
    };
  }
}
