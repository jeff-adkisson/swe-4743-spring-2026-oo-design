package edu.ksu.swe4743.smarthomescenes.repository;

import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Statement;

public final class SchemaInitializer {
    private SchemaInitializer() {
    }

    public static void initialize(Connection connection) throws SQLException {
        try (Statement statement = connection.createStatement()) {
            statement.executeUpdate(
                """
                CREATE TABLE IF NOT EXISTS scenes (
                    id TEXT PRIMARY KEY,
                    name TEXT NOT NULL
                );
                """
            );

            statement.executeUpdate(
                """
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
                """
            );
        }
    }
}
