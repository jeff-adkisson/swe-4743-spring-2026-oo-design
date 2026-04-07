package edu.ksu.swe4743.smarthomescenes.repository;

import edu.ksu.swe4743.smarthomescenes.domain.DeviceGroupTarget;
import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import edu.ksu.swe4743.smarthomescenes.domain.SceneAction;
import edu.ksu.swe4743.smarthomescenes.domain.SceneTarget;
import edu.ksu.swe4743.smarthomescenes.domain.SpecificDeviceTarget;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.UUID;

public final class SqliteSceneRepository implements SceneRepository {
    private final String connectionString;

    public SqliteSceneRepository(String dbPath) {
        this.connectionString = "jdbc:sqlite:" + dbPath;

        try (Connection connection = openConnection()) {
            SchemaInitializer.initialize(connection);
        } catch (SQLException exception) {
            throw new IllegalStateException("Failed to initialize SQLite schema.", exception);
        }
    }

    @Override
    public void save(DeviceScene scene) {
        try (Connection connection = openConnection()) {
            connection.setAutoCommit(false);

            try (PreparedStatement sceneStatement = connection.prepareStatement(
                """
                INSERT INTO scenes (id, name)
                VALUES (?, ?)
                ON CONFLICT(id) DO UPDATE SET name = excluded.name;
                """
            )) {
                sceneStatement.setString(1, scene.getId().toString());
                sceneStatement.setString(2, scene.getName());
                sceneStatement.executeUpdate();
            }

            try (PreparedStatement deleteStatement = connection.prepareStatement(
                "DELETE FROM scene_actions WHERE scene_id = ?;"
            )) {
                deleteStatement.setString(1, scene.getId().toString());
                deleteStatement.executeUpdate();
            }

            try (PreparedStatement actionStatement = connection.prepareStatement(
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
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);
                """
            )) {
                for (SceneAction action : scene.getActions()) {
                    actionStatement.setString(1, action.getId().toString());
                    actionStatement.setString(2, scene.getId().toString());
                    actionStatement.setInt(3, action.getOrderIndex());
                    actionStatement.setString(4, targetKindToStorageValue(action.getTarget()));
                    actionStatement.setString(8, action.getOperation());
                    actionStatement.setString(9, ParameterJson.toJson(action.getParameters()));

                    if (action.getTarget() instanceof SpecificDeviceTarget target) {
                        actionStatement.setString(5, target.getDeviceId().toString());
                        actionStatement.setString(6, null);
                        actionStatement.setString(7, null);
                    } else if (action.getTarget() instanceof DeviceGroupTarget target) {
                        actionStatement.setString(5, null);
                        actionStatement.setString(6, target.getDeviceType());
                        actionStatement.setString(7, target.getLocation());
                    } else {
                        throw new IllegalStateException("Unknown target type: " + action.getTarget().getClass().getName());
                    }

                    actionStatement.executeUpdate();
                }
            }

            connection.commit();
        } catch (SQLException exception) {
            throw new IllegalStateException("Failed to save scene.", exception);
        }
    }

    @Override
    public DeviceScene getById(UUID id) {
        try (Connection connection = openConnection()) {
            String name;

            try (PreparedStatement sceneStatement = connection.prepareStatement(
                """
                SELECT name
                FROM scenes
                WHERE id = ?;
                """
            )) {
                sceneStatement.setString(1, id.toString());

                try (ResultSet resultSet = sceneStatement.executeQuery()) {
                    if (!resultSet.next()) {
                        return null;
                    }

                    name = resultSet.getString("name");
                }
            }

            List<SceneAction> actions = new ArrayList<>();
            try (PreparedStatement actionStatement = connection.prepareStatement(
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
                WHERE scene_id = ?
                ORDER BY order_index;
                """
            )) {
                actionStatement.setString(1, id.toString());

                try (ResultSet resultSet = actionStatement.executeQuery()) {
                    while (resultSet.next()) {
                        actions.add(mapAction(resultSet));
                    }
                }
            }

            return new DeviceScene(id, name, actions);
        } catch (SQLException exception) {
            throw new IllegalStateException("Failed to load scene.", exception);
        }
    }

    @Override
    public List<DeviceScene> listAll() {
        try (Connection connection = openConnection();
             PreparedStatement statement = connection.prepareStatement(
                 """
                 SELECT id
                 FROM scenes
                 ORDER BY name;
                 """
             );
             ResultSet resultSet = statement.executeQuery()) {
            List<DeviceScene> scenes = new ArrayList<>();
            while (resultSet.next()) {
                var scene = getById(UUID.fromString(resultSet.getString("id")));
                if (scene != null) {
                    scenes.add(scene);
                }
            }

            return scenes;
        } catch (SQLException exception) {
            throw new IllegalStateException("Failed to list scenes.", exception);
        }
    }

    private Connection openConnection() throws SQLException {
        return DriverManager.getConnection(connectionString);
    }

    private SceneAction mapAction(ResultSet resultSet) throws SQLException {
        UUID actionId = UUID.fromString(resultSet.getString("id"));
        int orderIndex = resultSet.getInt("order_index");
        String targetKind = resultSet.getString("target_kind");
        String operation = resultSet.getString("operation");
        Map<String, String> parameters = ParameterJson.fromJson(resultSet.getString("parameters_json"));

        SceneTarget target = switch (targetKind) {
            case "device" -> new SpecificDeviceTarget(UUID.fromString(resultSet.getString("target_device_id")));
            case "group" -> new DeviceGroupTarget(
                resultSet.getString("target_device_type"),
                resultSet.getString("target_location"));
            default -> throw new IllegalStateException("Unknown target kind: " + targetKind);
        };

        return new SceneAction(actionId, orderIndex, target, operation, parameters);
    }

    private String targetKindToStorageValue(SceneTarget target) {
        if (target instanceof SpecificDeviceTarget) {
            return "device";
        }

        if (target instanceof DeviceGroupTarget) {
            return "group";
        }

        throw new IllegalStateException("Unknown target type: " + target.getClass().getName());
    }
}
