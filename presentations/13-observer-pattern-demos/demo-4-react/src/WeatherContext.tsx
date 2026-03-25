import { createContext, useContext, useState, useCallback, useRef, ReactNode } from 'react';

export interface WeatherData {
  temperature: number;
  humidity: number;
  pressure: number;
}

export interface LogEntry {
  timestamp: string;
  message: string;
}

interface WeatherContextType {
  weather: WeatherData;
  hasData: boolean;
  updateMeasurements: (t: number, h: number, p: number) => void;
  subscriberCount: number;
  registerSubscriber: (name: string) => void;
  unregisterSubscriber: (name: string) => void;
  log: LogEntry[];
  clearLog: () => void;
}

const WeatherContext = createContext<WeatherContextType | null>(null);

export function WeatherProvider({ children }: { children: ReactNode }) {
  const [weather, setWeather] = useState<WeatherData>({
    temperature: 0,
    humidity: 0,
    pressure: 0,
  });
  const [hasData, setHasData] = useState(false);
  const [subscriberCount, setSubscriberCount] = useState(0);
  const [log, setLog] = useState<LogEntry[]>([]);
  const countRef = useRef(0);

  const appendLog = useCallback((message: string) => {
    const timestamp = new Date().toLocaleTimeString();
    setLog((prev) => [{ timestamp, message }, ...prev]);
  }, []);

  const updateMeasurements = useCallback(
    (temp: number, humidity: number, pressure: number) => {
      setHasData(true);
      appendLog(`Notification({ temperature: ${temp}, humidity: ${humidity}, pressure: ${pressure} })`);
      setWeather({ temperature: temp, humidity, pressure });
    },
    [appendLog]
  );

  const registerSubscriber = useCallback(
    (name: string) => {
      countRef.current += 1;
      setSubscriberCount(countRef.current);
      appendLog(`${name}.subscribe() — now ${countRef.current} observer(s)`);
    },
    [appendLog]
  );

  const unregisterSubscriber = useCallback(
    (name: string) => {
      countRef.current -= 1;
      setSubscriberCount(countRef.current);
      appendLog(`${name}.unsubscribe() — now ${countRef.current} observer(s)`);
    },
    [appendLog]
  );

  const clearLog = useCallback(() => setLog([]), []);

  return (
    <WeatherContext.Provider
      value={{
        weather,
        hasData,
        updateMeasurements,
        subscriberCount,
        registerSubscriber,
        unregisterSubscriber,
        log,
        clearLog,
      }}
    >
      {children}
    </WeatherContext.Provider>
  );
}

export function useWeather(): WeatherContextType {
  const ctx = useContext(WeatherContext);
  if (!ctx) throw new Error('useWeather must be inside WeatherProvider');
  return ctx;
}
