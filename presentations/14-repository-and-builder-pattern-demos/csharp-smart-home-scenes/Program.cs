using SmartHomeScenesDemo.Builder;
using SmartHomeScenesDemo.Domain;
using SmartHomeScenesDemo.Repository;

var dbPath = ResolveDatabasePath();
var repository = new SqliteSceneRepository(dbPath);

Console.WriteLine("SMART HOME DEVICE SCENES DEMO (C#)");
Console.WriteLine("----------------------------------");
Console.WriteLine($"SQLite database: {dbPath}");
Console.WriteLine();

Console.WriteLine("1. Verifying that the builder rejects incomplete scenes...");
try
{
    new DeviceSceneBuilder()
        .Named("Broken Scene")
        .Build();
}
catch (InvalidOperationException exception)
{
    Console.WriteLine($"   Expected validation error: {exception.Message}");
}

Console.WriteLine();
Console.WriteLine("2. Building the Evening Arrival scene...");
var porchLightId = Guid.Parse("11111111-1111-1111-1111-111111111111");
var builder = new DeviceSceneBuilder();
var director = new SceneDirector(builder);
var scene = director.BuildEveningArrival(porchLightId);
PrintScene(scene);

Console.WriteLine();
Console.WriteLine("3. Saving the scene through ISceneRepository...");
repository.Save(scene);
Console.WriteLine("   Scene saved.");

Console.WriteLine();
Console.WriteLine("4. Loading the scene back from SQLite...");
var loadedScene = repository.GetById(scene.Id)
    ?? throw new InvalidOperationException("Expected the saved scene to exist.");
PrintScene(loadedScene);

Console.WriteLine();
Console.WriteLine("5. Listing all saved scenes...");
foreach (var savedScene in repository.ListAll())
{
    Console.WriteLine($"   - {savedScene.Name} ({savedScene.Id})");
}

static string ResolveDatabasePath()
{
    var path = Environment.GetEnvironmentVariable("SCENE_DB_PATH");
    if (string.IsNullOrWhiteSpace(path))
    {
        path = Path.Combine(AppContext.BaseDirectory, "data", "smart-home-scenes.db");
    }

    var directory = Path.GetDirectoryName(path);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    return path;
}

static void PrintScene(DeviceScene scene)
{
    Console.WriteLine($"   Scene: {scene.Name}");
    Console.WriteLine($"   Id:    {scene.Id}");
    Console.WriteLine("   Actions:");

    foreach (var action in scene.Actions)
    {
        Console.WriteLine(
            $"   {action.OrderIndex + 1}. {DescribeTarget(action.Target)} -> {action.Operation}{DescribeParameters(action.Parameters)}");
    }
}

static string DescribeTarget(SceneTarget target)
{
    return target switch
    {
        SpecificDeviceTarget specificDeviceTarget => $"Device {specificDeviceTarget.DeviceId}",
        DeviceGroupTarget groupTarget when string.IsNullOrWhiteSpace(groupTarget.Location) =>
            $"Group type={groupTarget.DeviceType}",
        DeviceGroupTarget groupTarget =>
            $"Group type={groupTarget.DeviceType}, location={groupTarget.Location}",
        _ => "Unknown target"
    };
}

static string DescribeParameters(IReadOnlyDictionary<string, string> parameters)
{
    if (parameters.Count == 0)
    {
        return string.Empty;
    }

    var joined = string.Join(", ", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    return $" ({joined})";
}
