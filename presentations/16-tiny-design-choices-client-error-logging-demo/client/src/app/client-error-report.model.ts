export interface ClientErrorReport {
    message: string;
    stack?: string;
    url: string;
    user?: string;
    timestamp: string;
    userAgent: string;
    component?: string;
}
