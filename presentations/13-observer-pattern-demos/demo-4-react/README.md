# Demo 4: Observer Pattern in React with Context

This is a companion demo for **Section 5: Observer in Client-Side Web Frameworks** (React subsection) of the Observer Pattern lecture.

## What It Demonstrates

- **WeatherContext** is the **Subject** — it holds state via `useState` and broadcasts changes to every component that calls `useWeather()`.
- **Five components** are **Observers** — each subscribes by mounting inside the `WeatherProvider` and unsubscribes by unmounting.
  - `CurrentConditions` — displays live temperature, humidity, and pressure.
  - `Statistics` — tracks min/avg/max temperature across readings.
  - `Forecast` — compares pressure changes to predict weather trends.
  - `HeatIndex` — computes the heat index from temperature and humidity.
  - `Snow` / `Fire` — snowfall animation at <= 32°F, fire animation at >= 110°F (linked to the Forecast toggle).
- **Toggle switches** mount and unmount components, triggering `useEffect` cleanup (unsubscribe). The subscriber count chip updates live and turns red when no observers are subscribed.
- **Randomize and Notify** nudges all three values by a small random amount and pushes the update to all observers in one click.
- **Event log** shows observer subscribe/unsubscribe events and subject notifications in reverse chronological order.

## Observer Pattern Mapping

| Observer Concept | React Equivalent |
|------------------|------------------|
| Subject          | Context + `useState` in `WeatherProvider` |
| Observer         | Component that calls `useWeather()` |
| Subscribe        | Component mounts inside Provider (`useEffect` runs) |
| Unsubscribe      | Component unmounts (`useEffect` cleanup runs) |
| Notify           | `setState` triggers re-render of all consuming components |

## Project Structure

```
src/
  WeatherContext.tsx    — Subject: state + context provider + event log
  CurrentConditions.tsx — Observer: live readings
  Statistics.tsx        — Observer: min/avg/max
  Forecast.tsx          — Observer: pressure trend
  HeatIndex.tsx         — Observer: heat index calculation
  Snow.tsx / Snow.css   — Observer: snowfall animation
  Fire.tsx / Fire.css   — Observer: fire animation
  App.tsx               — Shell: controls, toggles, event log
```

## Prerequisites

- Node.js 20+
- npm

## Running the Demo

```bash
cd 13-observer-pattern-demos/demo-4-react
npm install
npm run dev
```

This starts the Vite dev server. Open the URL shown in the terminal (typically `http://localhost:5173`).

## Usage

1. Enter temperature, humidity, and pressure values (or click **Randomize and Notify** for quick variation).
2. Click **Notify Observers** to push the current measurements to all mounted components.
3. Toggle the switches to mount/unmount individual observers. Watch the subscriber count chip change.
4. Use **All** / **None** links to bulk mount/unmount.
5. Push new measurements with some observers unmounted — they receive nothing.
6. Toggle them back on — they remount and receive the next update.
7. Check the **Event Log** to see subscribe, unsubscribe, and notification events.
8. Try temperature <= 32°F for snow, or >= 110°F for fire (linked to the Forecast toggle).

## Styled with Pico.css

The UI uses [Pico.css](https://picocss.com/) for minimal, classless styling loaded from CDN.
