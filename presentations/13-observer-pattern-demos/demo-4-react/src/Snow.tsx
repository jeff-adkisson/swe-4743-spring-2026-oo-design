import { useMemo } from 'react';
import { useWeather } from './WeatherContext';
import './Snow.css';

export function Snow() {
  const { weather, hasData } = useWeather();
  const snowing = hasData && weather.temperature <= 32;

  const flakes = useMemo(
    () =>
      Array.from({ length: 60 }, (_, i) => ({
        id: i,
        x: Math.random() * 100,
        speed: 3 + Math.random() * 5,
        delay: Math.random() * 5,
        size: 28 + Math.random() * 120,
        opacity: 0.4 + Math.random() * 0.6,
      })),
    []
  );

  if (!snowing) return null;

  return (
    <div className="snow-container" aria-hidden="true">
      {flakes.map((f) => (
        <div
          key={f.id}
          className="snowflake"
          style={{
            left: `${f.x}%`,
            animationDuration: `${f.speed}s`,
            animationDelay: `${f.delay}s`,
            fontSize: `${f.size}px`,
            opacity: f.opacity,
          }}
        >
          &#10052;
        </div>
      ))}
    </div>
  );
}
