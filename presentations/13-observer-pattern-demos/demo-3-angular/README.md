# Demo 3: Observer Pattern in Angular with RxJS

This is a companion demo for **Section 5: Observer in Client-Side Web Frameworks** (Angular subsection) of the Observer Pattern lecture.

## What It Demonstrates

- **WeatherService** is the **Subject** — it holds a `BehaviorSubject<WeatherData>` and exposes a read-only `Observable` via `.asObservable()`.
- **Five components** are **Observers** — each subscribes to `weather$` in `ngOnInit()` and unsubscribes in `ngOnDestroy()`.
  - `CurrentConditionsComponent` — displays live temperature, humidity, and pressure.
  - `StatisticsComponent` — tracks min/avg/max temperature across readings.
  - `ForecastComponent` — compares pressure changes to predict weather trends.
  - `HeatIndexComponent` — computes the heat index from temperature and humidity.
  - `SnowComponent` — triggers a snowfall animation when temperature < 32°F and humidity > 75%.
- **Toggle switches** destroy and recreate components, triggering `ngOnDestroy` (unsubscribe) and `ngOnInit` (subscribe). The subscriber count chip updates live and turns red when no observers are subscribed.
- **Randomize and Notify** nudges all three values by a small random amount and pushes the update to all observers in one click.
- **Event log** shows observer subscribe/unsubscribe events and subject notifications in reverse chronological order.

## Observer Pattern Mapping

| Observer Concept | Angular/RxJS Equivalent |
|------------------|-------------------------|
| Subject          | `BehaviorSubject` in `WeatherService` |
| Observer         | Component with `.subscribe()` in `ngOnInit()` |
| Subscribe        | `observable.subscribe(callback)` |
| Unsubscribe      | `subscription.unsubscribe()` in `ngOnDestroy()` |
| Notify           | `subject.next(value)` |

## Project Structure

```
src/app/
  weather.service.ts          — Subject: BehaviorSubject + event log
  current-conditions.component.ts — Observer: live readings
  statistics.component.ts     — Observer: min/avg/max
  forecast.component.ts       — Observer: pressure trend
  heat-index.component.ts     — Observer: heat index calculation
  snow.component.ts           — Observer: snowfall animation
  app.ts                      — Shell: controls, toggles, event log
```

## Prerequisites

- Node.js 20+
- npm

## Running the Demo

```bash
cd 13-observer-pattern-demos/demo-3-angular
npm install
npx ng serve --open
```

This starts the dev server and opens `http://localhost:4200` in your browser.

## Usage

1. Enter temperature, humidity, and pressure values (or click **Randomize and Notify** for quick variation).
2. Click **Notify Observers** to push the current measurements to all subscribed components.
3. Toggle the switches to subscribe/unsubscribe individual observers. Watch the subscriber count chip change.
4. Use **All** / **None** links to bulk subscribe/unsubscribe.
5. Push new measurements with some observers toggled off — they receive nothing.
6. Toggle them back on — they resubscribe and receive the next update.
7. Check the **Event Log** to see subscribe, unsubscribe, and notification events.
8. Try temperature below 32°F with humidity above 75% — watch the snow fall.

## Styled with Pico.css

The UI uses [Pico.css](https://picocss.com/) for minimal, classless styling loaded from CDN.
