import { useEffect, useRef } from 'react';
import { useWeather } from './WeatherContext';

export function Forecast() {
  const { weather, hasData, registerSubscriber, unregisterSubscriber } = useWeather();
  const lastPressure = useRef(0);
  const currentPressure = useRef(0);

  let forecast = 'Waiting for data...';
  if (hasData) {
    lastPressure.current = currentPressure.current;
    currentPressure.current = weather.pressure;

    if (weather.temperature >= 110) {
      forecast = 'You are cooked!';
    } else if (weather.temperature <= 32) {
      forecast = 'Snow!';
    } else if (lastPressure.current === 0) {
      forecast = 'Collecting baseline...';
    } else if (currentPressure.current > lastPressure.current) {
      forecast = 'Improving weather on the way!';
    } else if (currentPressure.current < lastPressure.current) {
      forecast = 'Watch out for cooler, rainy weather.';
    } else {
      forecast = 'More of the same.';
    }
  }

  useEffect(() => {
    registerSubscriber('ForecastDisplay');
    return () => unregisterSubscriber('ForecastDisplay');
  }, [registerSubscriber, unregisterSubscriber]);

  return (
    <article>
      <header>Forecast</header>
      <p>{forecast}</p>
    </article>
  );
}
