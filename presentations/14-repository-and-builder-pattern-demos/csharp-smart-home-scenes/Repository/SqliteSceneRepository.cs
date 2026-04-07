using System.Text.Json;
using Microsoft.Data.Sqlite;
using SmartHomeScenesDemo.Domain;

namespace SmartHomeScenesDemo.Repository;

public sealed class SqliteSceneRepository : ISceneRepository
{
    private readonly string _connectionString;

    public SqliteSceneRepository(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            throw new ArgumentException("Database path is required.", nameof(dbPath));
        }

        _connectionString = $"Data Source={dbPath}";

        using var connection = OpenConnection();
        SchemaInitializer.Initialize(connection);
    }

    public void Save(DeviceScene scene)
    {
        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();

        using (var sceneCommand = connection.CreateCommand())
        {
            sceneCommand.Transaction = transaction;
            sceneCommand.CommandText =
                """
                INSERT INTO scenes (id, name)
                VALUES ($id, $name)
                ON CONFLICT(id) DO UPDATE SET name = excluded.name;
                """;
            sceneCommand.Parameters.AddWithValue("$id", scene.Id.ToString());
            sceneCommand.Parameters.AddWithValue("$name", scene.Name);
            sceneCommand.ExecuteNonQuery();
        }

        using (var deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM scene_actions WHERE scene_id = $sceneId;";
            deleteCommand.Parameters.AddWithValue("$sceneId", scene.Id.ToString());
            deleteCommand.ExecuteNonQuery();
        }

        foreach (var action in scene.Actions)
        {
            using var actionCommand = connection.CreateCommand();
            actionCommand.Transaction = transaction;
            actionCommand.CommandText =
                """
                INSERT INTO scene_actions (
                    id,
                    scene_id,
                    order_index,
                    target_kind,
                    target_device_id,
                    target_device_type,
                    target_location,
                    operation,
                    parameters_json)
                VALUES (
                    $id,
                    $sceneId,
                    $orderIndex,
                    $targetKind,
                    $targetDeviceId,
                    $targetDeviceType,
                    $targetLocation,
                    $operation,
                    $parametersJson);
                """;

            actionCommand.Parameters.AddWithValue("$id", action.Id.ToString());
            actionCommand.Parameters.AddWithValue("$sceneId", scene.Id.ToString());
            actionCommand.Parameters.AddWithValue("$orderIndex", action.OrderIndex);
            actionCommand.Parameters.AddWithValue("$targetKind", TargetKindToStorageValue(action.Target.Kind));
            actionCommand.Parameters.AddWithValue("$operation", action.Operation);
            actionCommand.Parameters.AddWithValue("$parametersJson", JsonSerializer.Serialize(action.Parameters));

            switch (action.Target)
            {
                case SpecificDeviceTarget specificTarget:
                    actionCommand.Parameters.AddWithValue("$targetDeviceId", specificTarget.DeviceId.ToString());
                    actionCommand.Parameters.AddWithValue("$targetDeviceType", DBNull.Value);
                    actionCommand.Parameters.AddWithValue("$targetLocation", DBNull.Value);
                    break;

                case DeviceGroupTarget groupTarget:
                    actionCommand.Parameters.AddWithValue("$targetDeviceId", DBNull.Value);
                    actionCommand.Parameters.AddWithValue("$targetDeviceType", groupTarget.DeviceType);
                    actionCommand.Parameters.AddWithValue("$targetLocation", groupTarget.Location ?? (object)DBNull.Value);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown target type: {action.Target.GetType().Name}");
            }

            actionCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public DeviceScene? GetById(Guid id)
    {
        using var connection = OpenConnection();

        string? name = null;

        using (var sceneCommand = connection.CreateCommand())
        {
            sceneCommand.CommandText =
                """
                SELECT name
                FROM scenes
                WHERE id = $id;
                """;
            sceneCommand.Parameters.AddWithValue("$id", id.ToString());

            using var reader = sceneCommand.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            name = reader.GetString(0);
        }

        var actions = new List<SceneAction>();

        using (var actionCommand = connection.CreateCommand())
        {
            actionCommand.CommandText =
                """
                SELECT
                    id,
                    order_index,
                    target_kind,
                    target_device_id,
                    target_device_type,
                    target_location,
                    operation,
                    parameters_json
                FROM scene_actions
                WHERE scene_id = $sceneId
                ORDER BY order_index;
                """;
            actionCommand.Parameters.AddWithValue("$sceneId", id.ToString());

            using var reader = actionCommand.ExecuteReader();
            while (reader.Read())
            {
                var actionId = Guid.Parse(reader.GetString(0));
                var orderIndex = reader.GetInt32(1);
                var targetKind = reader.GetString(2);
                var targetDeviceId = reader.IsDBNull(3) ? null : reader.GetString(3);
                var targetDeviceType = reader.IsDBNull(4) ? null : reader.GetString(4);
                var targetLocation = reader.IsDBNull(5) ? null : reader.GetString(5);
                var operation = reader.GetString(6);
                var parametersJson = reader.GetString(7);

                var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(parametersJson)
                    ?? new Dictionary<string, string>();

                SceneTarget target = targetKind switch
                {
                    "device" => new SpecificDeviceTarget(Guid.Parse(targetDeviceId!)),
                    "group" => new DeviceGroupTarget(targetDeviceType!, targetLocation),
                    _ => throw new InvalidOperationException($"Unknown target kind '{targetKind}'.")
                };

                actions.Add(new SceneAction(
                    actionId,
                    orderIndex,
                    target,
                    operation,
                    parameters));
            }
        }

        return new DeviceScene(id, name!, actions);
    }

    public IReadOnlyList<DeviceScene> ListAll()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id
            FROM scenes
            ORDER BY name;
            """;

        var scenes = new List<DeviceScene>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var id = Guid.Parse(reader.GetString(0));
            var scene = GetById(id);
            if (scene is not null)
            {
                scenes.Add(scene);
            }
        }

        return scenes;
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static string TargetKindToStorageValue(SceneTargetKind kind)
    {
        return kind switch
        {
            SceneTargetKind.SpecificDevice => "device",
            SceneTargetKind.DeviceGroup => "group",
            _ => throw new InvalidOperationException($"Unknown target kind: {kind}")
        };
    }
}
