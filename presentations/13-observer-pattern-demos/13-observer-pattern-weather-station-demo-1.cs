// Observer Pattern — Section 3: Weather Station (Single Observer)
// Run with: dotnet run 13-observer-pattern-weather-station-demo-1.cs

// --- Demo (top-level statements must precede type declarations) ---

Console.Clear();

var station = new WeatherStation();

var currentDisplay = new CurrentConditionsDisplay();
station.Subscribe(currentDisplay);
station.SetMeasurements(80, 65, 1013.1f);
station.SetMeasurements(82, 70, 1012.5f);

// Unsubscribe and verify no more notifications
station.Unsubscribe(currentDisplay);
station.SetMeasurements(78, 90, 1011.0f);
Console.WriteLine("(No output above — currentDisplay was unsubscribed before the third reading)");

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

// --- Observer ---

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
