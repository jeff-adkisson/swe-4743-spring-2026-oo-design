import { useEffect, useRef } from 'react';
import { useWeather } from './WeatherContext';

export function Statistics() {
  const { weather, hasData, registerSubscriber, unregisterSubscriber } = useWeather();
  const readings = useRef<number[]>([]);
  const lastTemp = useRef<number | null>(null);

  // Track readings — only push when temperature actually changes
  if (hasData && weather.temperature !== lastTemp.current) {
    lastTemp.current = weather.temperature;
    readings.current.push(weather.temperature);
  }

  const count = readings.current.length;
  const avg = count > 0 ? readings.current.reduce((a, b) => a + b, 0) / count : 0;
  const min = count > 0 ? Math.min(...readings.current) : 0;
  const max = count > 0 ? Math.max(...readings.current) : 0;

  useEffect(() => {
    registerSubscriber('StatisticsDisplay');
    return () => unregisterSubscriber('StatisticsDisplay');
  }, [registerSubscriber, unregisterSubscriber]);

  return (
    <article>
      <header>Statistics</header>
      <p>
        Avg: <strong>{avg.toFixed(1)}°F</strong> | Min: {min}°F | Max: {max}°F
      </p>
      <small>{count} reading(s) recorded</small>
    </article>
  );
}
