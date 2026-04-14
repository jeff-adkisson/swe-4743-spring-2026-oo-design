import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ClientErrorReport } from './client-error-report.model';

@Injectable({ providedIn: 'root' })
export class ClientLogService {

    private readonly endpoint = 'http://localhost:5280/api/client-log';

    /** Exposes every report sent to the server so the UI can display it. */
    readonly reports = signal<ClientErrorReport[]>([]);

    constructor(private http: HttpClient) {}

    reportError(report: ClientErrorReport): void {
        // Track for UI display
        this.reports.update(prev => [...prev, report]);

        // Fire-and-forget — a logging failure must never cascade
        this.http.post(this.endpoint, report).subscribe({
            error: () => console.warn('Failed to report client error to server.'),
        });
    }
}
