import { Component, inject } from '@angular/core';
import { ClientLogService } from './client-log.service';
import { JsonPipe, DatePipe } from '@angular/common';

@Component({
    selector: 'app-root',
    imports: [JsonPipe, DatePipe],
    template: `
        <main>
            <h1>Client-Side Error Logging Demo</h1>
            <p class="subtitle">
                Lecture 16, Appendix B &mdash; Tiny Design Choices, Massive Consequences
            </p>

            <section class="actions">
                <h2>Trigger an Error</h2>
                <p>Click the button below to throw a runtime error inside this Angular component.
                   The global <code>ErrorHandler</code> will catch it, package it with context,
                   and POST it to the ASP.NET Core API at <code>/api/client-log</code>.</p>

                <button (click)="throwError()">
                    Throw Runtime Error
                </button>
            </section>

            @if (logService.reports().length > 0) {
                <section class="reports">
                    <h2>Error Reports Sent to Server</h2>
                    <p class="note">
                        Each card below shows the exact JSON payload that was POSTed
                        to the API. Check the API console for the corresponding
                        <code>[CLIENT-SIDE ERROR]</code> log entry.
                    </p>

                    @for (report of logService.reports(); track $index) {
                        <div class="report-card">
                            <div class="report-header">
                                <span class="badge">Report #{{ $index + 1 }}</span>
                                <span class="timestamp">{{ report.timestamp | date:'medium' }}</span>
                            </div>

                            <table>
                                <tr><th>Message</th><td>{{ report.message }}</td></tr>
                                <tr><th>URL</th><td>{{ report.url }}</td></tr>
                                <tr><th>User</th><td>{{ report.user }}</td></tr>
                                <tr><th>Component</th><td>{{ report.component ?? '(not detected)' }}</td></tr>
                                <tr><th>User Agent</th><td class="ua">{{ report.userAgent }}</td></tr>
                            </table>

                            <details>
                                <summary>Stack Trace</summary>
                                <pre>{{ report.stack }}</pre>
                            </details>

                            <details>
                                <summary>Raw JSON Payload</summary>
                                <pre>{{ report | json }}</pre>
                            </details>
                        </div>
                    }
                </section>
            }
        </main>
    `,
    styles: [`
        :host {
            font-family: system-ui, -apple-system, sans-serif;
            display: block;
            max-width: 860px;
            margin: 2rem auto;
            padding: 0 1rem;
            color: #1a1a2e;
        }
        h1 { margin-bottom: 0.25rem; }
        .subtitle { color: #666; margin-top: 0; }
        .actions { margin: 2rem 0; }
        button {
            background: #d32f2f;
            color: white;
            border: none;
            padding: 0.75rem 1.5rem;
            font-size: 1rem;
            border-radius: 6px;
            cursor: pointer;
            transition: background 0.2s;
        }
        button:hover { background: #b71c1c; }
        .reports { margin-top: 2rem; }
        .note { color: #555; font-size: 0.9rem; }
        .report-card {
            border: 1px solid #ddd;
            border-left: 4px solid #d32f2f;
            border-radius: 6px;
            padding: 1rem 1.25rem;
            margin-bottom: 1.5rem;
            background: #fafafa;
        }
        .report-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 0.75rem;
        }
        .badge {
            background: #d32f2f;
            color: white;
            padding: 0.2rem 0.6rem;
            border-radius: 4px;
            font-size: 0.8rem;
            font-weight: 600;
        }
        .timestamp { color: #888; font-size: 0.85rem; }
        table { width: 100%; border-collapse: collapse; margin-bottom: 0.75rem; }
        th {
            text-align: left;
            width: 120px;
            padding: 0.3rem 0.5rem;
            color: #555;
            font-size: 0.85rem;
            vertical-align: top;
        }
        td { padding: 0.3rem 0.5rem; font-size: 0.85rem; }
        .ua { word-break: break-all; font-size: 0.75rem; color: #777; }
        details { margin-top: 0.5rem; }
        summary {
            cursor: pointer;
            font-size: 0.85rem;
            color: #1976d2;
            font-weight: 500;
        }
        pre {
            background: #263238;
            color: #eeffff;
            padding: 1rem;
            border-radius: 4px;
            overflow-x: auto;
            font-size: 0.8rem;
            line-height: 1.4;
            white-space: pre-wrap;
            word-break: break-word;
        }
        code {
            background: #e8eaf6;
            padding: 0.1rem 0.3rem;
            border-radius: 3px;
            font-size: 0.85em;
        }
    `],
})
export class App {
    readonly logService = inject(ClientLogService);

    throwError(): void {
        // Simulate a real runtime error inside an Angular component.
        // This will be caught by GlobalErrorHandler automatically.
        const obj: any = null;
        obj.thisPropertyDoesNotExist();  // throws TypeError
    }
}
