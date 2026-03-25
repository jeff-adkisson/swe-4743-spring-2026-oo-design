import { useEffect } from 'react';
import { useWeather } from './WeatherContext';

function computeHeatIndex(t: number, rh: number): number {
  return (
    16.923 +
    1.85212e-1 * t +
    5.37941 * rh -
    1.00254e-1 * t * rh +
    9.41695e-3 * t * t +
    7.28898e-3 * rh * rh +
    3.45372e-4 * t * t * rh -
    8.14971e-4 * t * rh * rh +
    1.02102e-5 * t * t * rh * rh -
    3.8646e-5 * t * t * t +
    2.91583e-5 * rh * rh * rh +
    1.42721e-6 * t * t * t * rh +
    1.97483e-7 * t * rh * rh * rh -
    2.18429e-8 * t * t * t * rh * rh +
    8.43296e-10 * t * t * rh * rh * rh -
    4.81975e-11 * t * t * t * rh * rh * rh
  );
}

export function HeatIndex() {
  const { weather, registerSubscriber, unregisterSubscriber } = useWeather();
  const hi = computeHeatIndex(weather.temperature, weather.humidity);

  useEffect(() => {
    registerSubscriber('HeatIndexDisplay');
    return () => unregisterSubscriber('HeatIndexDisplay');
  }, [registerSubscriber, unregisterSubscriber]);

  return (
    <article>
      <header>Heat Index</header>
      <p>
        <strong>{hi.toFixed(2)}°F</strong>
      </p>
    </article>
  );
}
