import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { ClientLogService } from './client-log.service';
import { ClientErrorReport } from './client-error-report.model';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

    // Use Injector to avoid circular dependency with HttpClient
    constructor(private injector: Injector) {}

    handleError(error: unknown): void {
        // Always log to the browser console for local debugging
        console.error('[GlobalErrorHandler caught]', error);

        const logService = this.injector.get(ClientLogService);

        const report: ClientErrorReport = {
            message: error instanceof Error ? error.message : String(error),
            stack: error instanceof Error ? error.stack : undefined,
            url: window.location.pathname,
            user: 'SpaErrorLoggingDemoUser',           // static for this demo
            timestamp: new Date().toISOString(),
            userAgent: navigator.userAgent,
            component: this.extractComponentName(error),
        };

        logService.reportError(report);
    }

    /** Best-effort extraction of an Angular component name from the stack trace. */
    private extractComponentName(error: unknown): string | undefined {
        const stack = error instanceof Error ? error.stack : undefined;
        if (!stack) return undefined;

        // Look for "ComponentName." or "ComponentName_" in the stack
        const match = stack.match(/at (\w+Component)\./);
        return match?.[1];
    }
}
