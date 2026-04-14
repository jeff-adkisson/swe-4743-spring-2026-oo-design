# Client-Side Error Logging Demo

Lecture 16, Appendix B — Tiny Design Choices, Massive Consequences

Demonstrates how runtime errors in an Angular SPA can be captured, packaged with context, and forwarded to an ASP.NET Core API for server-side logging.

## Architecture

```mermaid
flowchart TB
    subgraph "Server (ASP.NET Core :5280)"
        CTRL["ClientLogController\n(rate-limited 10/min)"]
        LOG["ILogger output\n[CLIENT-SIDE ERROR]"]
        CTRL --> LOG
    end
    subgraph "Browser (Angular :4200)"
        BTN["Button click"] --> ERR["TypeError thrown"]
        ERR --> GEH["GlobalErrorHandler"]
        GEH --> UI["UI displays\nerror payload"]
    end

    GEH -->|"POST /api/client-log"| CTRL
```

## Prerequisites

- .NET 8+ SDK
- Node.js 18+ and npm
- Angular CLI (`npm install -g @angular/cli`)

## Running the Demo

Open two terminals:

### Terminal 1 — API (port 5280)

```bash
cd Api
dotnet run
```

### Terminal 2 — Angular Client (port 4200)

```bash
cd client
ng serve
```

Open http://localhost:4200 and click **Throw Runtime Error**.

- The Angular UI will display the error report payload that was sent to the server.
- The API terminal will show a `[CLIENT-SIDE ERROR]` log entry with the full details.
