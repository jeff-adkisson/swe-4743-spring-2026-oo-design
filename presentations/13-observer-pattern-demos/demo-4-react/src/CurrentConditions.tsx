import { useEffect } from 'react';
import { useWeather } from './WeatherContext';

export function CurrentConditions() {
  const { weather, registerSubscriber, unregisterSubscriber } = useWeather();

  // Mount = subscribe, unmount = unsubscribe
  useEffect(() => {
    registerSubscriber('CurrentConditionsDisplay');
    return () => unregisterSubscriber('CurrentConditionsDisplay');
  }, [registerSubscriber, unregisterSubscriber]);

  return (
    <article>
      <header>Current Conditions</header>
      <p>
        <strong>{weather.temperature}°F</strong> | {weather.humidity}% humidity |{' '}
        {weather.pressure} hPa
      </p>
    </article>
  );
}
