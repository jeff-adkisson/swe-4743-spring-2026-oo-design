# Demo 5: Message Queue Chat with RabbitMQ, C#, Angular, and SSE

This demo is a companion for **Section 6: Decoupling with a Message Queue** in the Observer lecture.

It implements **Wacky Chat**, a small live chat room with this flow:

- Each visitor joins with a unique lowercase handle.
- The back end creates **one RabbitMQ queue per connected visitor**.
- Chat events are published once to a **fanout exchange**.
- RabbitMQ copies each event into every connected visitor queue.
- The ASP.NET back end consumes each visitor queue and pushes deliveries to the browser with **Server-Sent Events (SSE)**.

## What It Demonstrates

- **Publisher**: the ASP.NET back end publishes chat events to `wacky-chat.exchange`.
- **Broker**: RabbitMQ routes each published event to all bound visitor queues.
- **Per-visitor queue**: each connected visitor gets a queue such as `wacky-chat.visitor.max.ab12cd34`.
- **Subscriber push**: the browser does not poll for new messages. The server streams queue deliveries over `text/event-stream`.
- **Join/leave events**: when someone joins, everyone sees `{name} has joined the chat...`. When someone leaves, everyone sees `{name} has left the chat...`.

## Username Rules

- Allowed characters: lowercase letters, numbers, dashes, underscores
- Regex: `^[a-z0-9_-]+$`
- Names must be unique while connected

Examples:

- `max`
- `chloe_2`
- `jeffr0x_4ever`

Rejected examples:

- `Jeff`
- `space cadet`
- `chloe!`

## Architecture

```text
Browser
  ├─ POST /api/chat/join
  ├─ POST /api/chat/messages
  ├─ POST /api/chat/leave
  └─ GET  /api/chat/stream/{sessionId}  (SSE)

ASP.NET Core
  ├─ validates usernames and messages
  ├─ tracks connected sessions
  ├─ creates one RabbitMQ queue per visitor
  ├─ publishes chat events to a fanout exchange
  └─ consumes each visitor queue and pushes deliveries via SSE

RabbitMQ
  ├─ exchange: wacky-chat.exchange
  └─ queues: one queue per connected visitor
```

## Prerequisites

- Docker Desktop or Docker Engine with Compose
- Optional: `ngrok` if you want to expose the chat to students over the public internet

## Build and Run

From this directory:

```bash
cd presentations/13-observer-pattern-demos/demo-5-message-queue-chat
docker compose up --build
```

When the containers are ready:

- Chat app: [http://localhost:8080](http://localhost:8080)
- RabbitMQ management UI: [http://localhost:15672](http://localhost:15672)
- RabbitMQ credentials: `guest` / `guest`

To stop the demo:

```bash
docker compose down
```

To stop and remove any leftover RabbitMQ data or containers for a clean restart:

```bash
docker compose down --volumes --remove-orphans
```

## Using ngrok in Class

The simplest classroom setup is to expose only the chat app:

```bash
ngrok http --domain=wackychat.ngrok.app 8080
```

Reserve `wacky-chat.ngrok.app` in your ngrok Domains dashboard first if it is not already attached to your account. Once it exists, students can use the same stable URL throughout the demo instead of a fresh random tunnel name.

If your reserved hostname uses a different ngrok-managed suffix, substitute that exact domain in the command above.

You can keep the RabbitMQ management UI local in your own browser at:

- [http://localhost:15672](http://localhost:15672)

If you also want a public RabbitMQ management URL for a second device, start another ngrok tunnel separately for port `15672`.

## Local Development Notes

If you want to run pieces outside Docker:

- RabbitMQ default host for local development is `localhost`
- Angular dev server can run on `http://localhost:4200`
- The Angular app is already configured to call the back end at `http://localhost:8080` when it detects port `4200`

Typical local workflow:

```bash
cd backend
dotnet run
```

In another terminal:

```bash
cd frontend
npm install
npm run start
```

And separately:

```bash
docker run --rm -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

## Files

- `frontend/` Angular UI
- `backend/` ASP.NET Core API + SSE bridge + RabbitMQ integration
- `compose.yaml` demo stack
- `Dockerfile` production image that serves the Angular build from ASP.NET Core
