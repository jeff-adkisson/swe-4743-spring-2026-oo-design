// Observer Pattern — Section 4: Weather Station (Multiple Observers)
// Run with: dotnet run 13-observer-pattern-weather-station-demo-2.cs

// --- Demo (top-level statements must precede type declarations) ---

Console.Clear();

var station = new WeatherStation();

var currentDisplay = new CurrentConditionsDisplay();
var statsDisplay = new StatisticsDisplay();
var forecastDisplay = new ForecastDisplay();
var heatIndexDisplay = new HeatIndexDisplay();

station.Subscribe(currentDisplay);
station.Subscribe(statsDisplay);
station.Subscribe(forecastDisplay);
station.Subscribe(heatIndexDisplay);

Console.WriteLine("=== First Reading ===");
station.SetMeasurements(80, 65, 1013.1f);

Console.WriteLine("\n=== Second Reading ===");
station.SetMeasurements(82, 70, 1012.5f);

Console.WriteLine("\n=== Third Reading (forecast unsubscribed) ===");
station.Unsubscribe(forecastDisplay);
station.SetMeasurements(78, 90, 1011.0f);

// --- Interfaces ---

public interface IObserver
{
    void Update(ISubject subject);
}

public interface ISubject
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    void Notify();
}

// --- Subject ---

public class WeatherStation : ISubject
{
    private readonly List<IObserver> _observers = new();

    public float Temperature { get; private set; }
    public float Humidity { get; private set; }
    public float Pressure { get; private set; }

    public void Subscribe(IObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
            observer.Update(this);
    }

    public void SetMeasurements(float temperature, float humidity, float pressure)
    {
        Console.WriteLine($"Setting measurements: {temperature}°F, {humidity}% humidity, {pressure} hPa");
        Temperature = temperature;
        Humidity = humidity;
        Pressure = pressure;
        Console.WriteLine("Measurements updated, notifying observers (if any)...");
        Notify();
    }
}

// --- Observers ---

public class CurrentConditionsDisplay : IObserver
{
    public void Update(ISubject subject)
    {
        if (subject is WeatherStation ws)
        {
            Console.WriteLine("*** CurrentConditionsDisplay received update ***");
            Console.WriteLine(
                $"[Current Conditions] {ws.Temperature}°F, " +
                $"{ws.Humidity}% humidity, {ws.Pressure} hPa");
            Console.WriteLine("*** End of update ***");
            Console.WriteLine();
        }
    }
}

public class StatisticsDisplay : IObserver
{
    private readonly List<float> _temperatures = new();

    public void Update(ISubject subject)
    {
        if (subject is WeatherStation ws)
        {
            _temperatures.Add(ws.Temperature);
            float avg = _temperatures.Average();
            float min = _temperatures.Min();
            float max = _temperatures.Max();
            Console.WriteLine("*** StatisticsDisplay received update ***");
            Console.WriteLine(
                $"[Statistics] Avg: {avg:F1}°F, Min: {min}°F, Max: {max}°F");
            Console.WriteLine("*** End of update ***");
            Console.WriteLine();
        }
    }
}

public class ForecastDisplay : IObserver
{
    private float _lastPressure;
    private float _currentPressure;

    public void Update(ISubject subject)
    {
        if (subject is WeatherStation ws)
        {
            _lastPressure = _currentPressure;
            _currentPressure = ws.Pressure;

            string forecast = _currentPressure > _lastPressure
                ? "Improving weather on the way!"
                : _currentPressure < _lastPressure
                    ? "Watch out for cooler, rainy weather."
                    : "More of the same.";

            Console.WriteLine("*** ForecastDisplay received update ***");
            Console.WriteLine($"[Forecast] {forecast}");
            Console.WriteLine("*** End of update ***");
            Console.WriteLine();
        }
    }
}

public class HeatIndexDisplay : IObserver
{
    public void Update(ISubject subject)
    {
        if (subject is WeatherStation ws)
        {
            double hi = ComputeHeatIndex(ws.Temperature, ws.Humidity);
            Console.WriteLine("*** HeatIndexDisplay received update ***");
            Console.WriteLine($"[Heat Index] {hi:F2}°F");
            Console.WriteLine("*** End of update ***");
            Console.WriteLine();
        }
    }

    private static double ComputeHeatIndex(float t, float rh)
    {
        return 16.923 + 1.85212e-1 * t + 5.37941 * rh
             - 1.00254e-1 * t * rh + 9.41695e-3 * t * t
             + 7.28898e-3 * rh * rh + 3.45372e-4 * t * t * rh
             - 8.14971e-4 * t * rh * rh + 1.02102e-5 * t * t * rh * rh
             - 3.8646e-5 * t * t * t + 2.91583e-5 * rh * rh * rh
             + 1.42721e-6 * t * t * t * rh + 1.97483e-7 * t * rh * rh * rh
             - 2.18429e-8 * t * t * t * rh * rh
             + 8.43296e-10 * t * t * rh * rh * rh
             - 4.81975e-11 * t * t * t * rh * rh * rh;
    }
}
