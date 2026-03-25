import { useState } from 'react';
import { WeatherProvider, useWeather } from './WeatherContext';
import { CurrentConditions } from './CurrentConditions';
import { Statistics } from './Statistics';
import { Forecast } from './Forecast';
import { HeatIndex } from './HeatIndex';
import { Snow } from './Snow';
import { Fire } from './Fire';

function Dashboard() {
  const { subscriberCount, updateMeasurements, log, clearLog } = useWeather();

  const [temperature, setTemperature] = useState(80);
  const [humidity, setHumidity] = useState(65);
  const [pressure, setPressure] = useState(1013.1);

  const [showCurrent, setShowCurrent] = useState(true);
  const [showStats, setShowStats] = useState(true);
  const [showForecast, setShowForecast] = useState(true);
  const [showHeatIndex, setShowHeatIndex] = useState(true);

  const pushMeasurements = () => {
    updateMeasurements(temperature, humidity, pressure);
  };

  const randomizeAndNotify = () => {
    const t = Math.round((temperature + (Math.random() * 6 - 3)) * 10) / 10;
    const h = Math.round(Math.min(100, Math.max(0, humidity + (Math.random() * 10 - 5))) * 10) / 10;
    const p = Math.round((pressure + (Math.random() * 4 - 2)) * 10) / 10;
    setTemperature(t);
    setHumidity(h);
    setPressure(p);
    updateMeasurements(t, h, p);
  };

  const setAll = (on: boolean) => {
    setShowCurrent(on);
    setShowStats(on);
    setShowForecast(on);
    setShowHeatIndex(on);
  };

  return (
    <>
      {showForecast && (
        <>
          <Snow />
          <Fire />
        </>
      )}
      <main className="container">
        <h1>Observer Pattern &mdash; React Weather Station</h1>
        <p>
          Each component below is an <strong>Observer</strong> that subscribes to
          the <code>WeatherContext</code> (<strong>Subject</strong>) via React
          Context. Toggle them on/off to see mount/unmount (subscribe/unsubscribe) in action.
        </p>

        <hr />

        <div className="grid" style={{ gridTemplateColumns: '5fr 3fr', alignItems: 'start' }}>
          {/* Left column: controls + event log */}
          <section>
            <h2>Push New Measurements</h2>
            <form
              onSubmit={(e) => {
                e.preventDefault();
                pushMeasurements();
              }}
            >
              <div className="grid">
                <label>
                  Temp (°F)
                  <input
                    type="number"
                    step="0.1"
                    value={temperature}
                    onChange={(e) => setTemperature(parseFloat(e.target.value) || 0)}
                  />
                </label>
                <label>
                  Humidity (%)
                  <input
                    type="number"
                    step="0.1"
                    value={humidity}
                    onChange={(e) => setHumidity(parseFloat(e.target.value) || 0)}
                  />
                </label>
                <label>
                  Pressure (hPa)
                  <input
                    type="number"
                    step="0.1"
                    value={pressure}
                    onChange={(e) => setPressure(parseFloat(e.target.value) || 0)}
                  />
                </label>
              </div>
              <div className="grid" style={{ gridTemplateColumns: '1fr 1fr' }}>
                <button type="submit">Notify Observers</button>
                <button type="button" className="secondary" onClick={randomizeAndNotify}>
                  Randomize and Notify
                </button>
              </div>
            </form>

            {/* Event log */}
            <h2 style={{ display: 'flex', alignItems: 'center' }}>
              Event Log
              <small style={{ marginLeft: 'auto', fontSize: '0.8rem' }}>
                <a
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    clearLog();
                  }}
                >
                  Clear
                </a>
              </small>
            </h2>
            <small>Shows observer subscribe/unsubscribe events and subject notifications</small>
            <pre style={{ maxHeight: '800px', overflowY: 'auto' }}>
              <code>
                {log.map((entry, i) => (
                  <span key={i}>
                    [{entry.timestamp}] {entry.message}
                    {'\n'}
                  </span>
                ))}
              </code>
            </pre>
          </section>

          {/* Right column: observer toggles + cards */}
          <section>
            <h2 style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              Observers
              <span
                style={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  minWidth: '1.6rem',
                  height: '1.6rem',
                  padding: '0 0.5rem',
                  borderRadius: '999px',
                  background: subscriberCount === 0 ? '#dc3545' : 'var(--pico-primary-background)',
                  color: 'var(--pico-primary-inverse)',
                  fontSize: '0.8rem',
                  fontWeight: 600,
                }}
              >
                {subscriberCount}
              </span>
              <small style={{ marginLeft: 'auto', fontSize: '0.8rem' }}>
                <a href="#" onClick={(e) => { e.preventDefault(); setAll(true); }}>All</a>
                {' | '}
                <a href="#" onClick={(e) => { e.preventDefault(); setAll(false); }}>None</a>
              </small>
            </h2>

            <label>
              <input type="checkbox" role="switch" checked={showCurrent} onChange={(e) => setShowCurrent(e.target.checked)} />
              Current Conditions
            </label>
            {showCurrent && <CurrentConditions />}

            <label>
              <input type="checkbox" role="switch" checked={showStats} onChange={(e) => setShowStats(e.target.checked)} />
              Statistics
            </label>
            {showStats && <Statistics />}

            <label>
              <input type="checkbox" role="switch" checked={showForecast} onChange={(e) => setShowForecast(e.target.checked)} />
              Forecast
            </label>
            {showForecast && <Forecast />}

            <label>
              <input type="checkbox" role="switch" checked={showHeatIndex} onChange={(e) => setShowHeatIndex(e.target.checked)} />
              Heat Index
            </label>
            {showHeatIndex && <HeatIndex />}
          </section>
        </div>
      </main>
    </>
  );
}

export default function App() {
  return (
    <WeatherProvider>
      <Dashboard />
    </WeatherProvider>
  );
}
