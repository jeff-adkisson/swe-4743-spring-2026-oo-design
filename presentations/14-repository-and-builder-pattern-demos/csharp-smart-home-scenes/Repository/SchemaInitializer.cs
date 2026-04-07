using Microsoft.Data.Sqlite;

namespace SmartHomeScenesDemo.Repository;

public static class SchemaInitializer
{
    public static void Initialize(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS scenes (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS scene_actions (
                id TEXT PRIMARY KEY,
                scene_id TEXT NOT NULL,
                order_index INTEGER NOT NULL,
                target_kind TEXT NOT NULL,
                target_device_id TEXT NULL,
                target_device_type TEXT NULL,
                target_location TEXT NULL,
                operation TEXT NOT NULL,
                parameters_json TEXT NOT NULL,
                FOREIGN KEY (scene_id) REFERENCES scenes(id)
            );
            """;

        command.ExecuteNonQuery();
    }
}
