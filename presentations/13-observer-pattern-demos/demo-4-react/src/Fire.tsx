import { useMemo } from 'react';
import { useWeather } from './WeatherContext';
import './Fire.css';

export function Fire() {
  const { weather, hasData } = useWeather();
  const burning = hasData && weather.temperature >= 110;

  const flames = useMemo(
    () =>
      Array.from({ length: 50 }, (_, i) => ({
        id: i,
        x: Math.random() * 100,
        speed: 2 + Math.random() * 4,
        delay: Math.random() * 4,
        size: 28 + Math.random() * 160,
        opacity: 0.5 + Math.random() * 0.5,
        waggle: 1.5 + Math.random() * 2,
      })),
    []
  );

  if (!burning) return null;

  return (
    <div className="fire-container" aria-hidden="true">
      {flames.map((f) => (
        <div
          key={f.id}
          className="flame"
          style={{
            left: `${f.x}%`,
            animationDuration: `${f.speed}s, ${f.waggle}s`,
            animationDelay: `${f.delay}s`,
            fontSize: `${f.size}px`,
            opacity: f.opacity,
          }}
        >
          &#128293;
        </div>
      ))}
    </div>
  );
}
