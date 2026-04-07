package edu.ksu.swe4743.smarthomescenes;

import edu.ksu.swe4743.smarthomescenes.builder.DeviceSceneBuilder;
import edu.ksu.swe4743.smarthomescenes.builder.SceneDirector;
import edu.ksu.swe4743.smarthomescenes.domain.DeviceGroupTarget;
import edu.ksu.swe4743.smarthomescenes.domain.DeviceScene;
import edu.ksu.swe4743.smarthomescenes.domain.SceneAction;
import edu.ksu.swe4743.smarthomescenes.domain.SceneTarget;
import edu.ksu.swe4743.smarthomescenes.domain.SpecificDeviceTarget;
import edu.ksu.swe4743.smarthomescenes.repository.SqliteSceneRepository;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Map;
import java.util.UUID;

public final class Program {
    private Program() {
    }

    public static void main(String[] args) throws Exception {
        String dbPath = resolveDatabasePath();
        var repository = new SqliteSceneRepository(dbPath);

        System.out.println("SMART HOME DEVICE SCENES DEMO (JAVA)");
        System.out.println("------------------------------------");
        System.out.printf("SQLite database: %s%n", dbPath);
        System.out.println();

        System.out.println("1. Verifying that the builder rejects incomplete scenes...");
        try {
            new DeviceSceneBuilder()
                .named("Broken Scene")
                .build();
        } catch (IllegalStateException exception) {
            System.out.printf("   Expected validation error: %s%n", exception.getMessage());
        }

        System.out.println();
        System.out.println("2. Building the Evening Arrival scene...");
        UUID porchLightId = UUID.fromString("11111111-1111-1111-1111-111111111111");
        var builder = new DeviceSceneBuilder();
        var director = new SceneDirector(builder);
        var scene = director.buildEveningArrival(porchLightId);
        printScene(scene);

        System.out.println();
        System.out.println("3. Saving the scene through SceneRepository...");
        repository.save(scene);
        System.out.println("   Scene saved.");

        System.out.println();
        System.out.println("4. Loading the scene back from SQLite...");
        var loadedScene = repository.getById(scene.getId());
        if (loadedScene == null) {
            throw new IllegalStateException("Expected the saved scene to exist.");
        }
        printScene(loadedScene);

        System.out.println();
        System.out.println("5. Listing all saved scenes...");
        for (DeviceScene savedScene : repository.listAll()) {
            System.out.printf("   - %s (%s)%n", savedScene.getName(), savedScene.getId());
        }
    }

    private static String resolveDatabasePath() throws Exception {
        String path = System.getenv("SCENE_DB_PATH");
        if (path == null || path.isBlank()) {
            path = Path.of("data", "smart-home-scenes.db").toString();
        }

        Path parent = Path.of(path).getParent();
        if (parent != null) {
            Files.createDirectories(parent);
        }

        return path;
    }

    private static void printScene(DeviceScene scene) {
        System.out.printf("   Scene: %s%n", scene.getName());
        System.out.printf("   Id:    %s%n", scene.getId());
        System.out.println("   Actions:");

        for (SceneAction action : scene.getActions()) {
            System.out.printf(
                "   %d. %s -> %s%s%n",
                action.getOrderIndex() + 1,
                describeTarget(action.getTarget()),
                action.getOperation(),
                describeParameters(action.getParameters()));
        }
    }

    private static String describeTarget(SceneTarget target) {
        if (target instanceof SpecificDeviceTarget specificDeviceTarget) {
            return "Device " + specificDeviceTarget.getDeviceId();
        }

        if (target instanceof DeviceGroupTarget groupTarget) {
            if (groupTarget.getLocation() == null || groupTarget.getLocation().isBlank()) {
                return "Group type=" + groupTarget.getDeviceType();
            }

            return "Group type=" + groupTarget.getDeviceType() + ", location=" + groupTarget.getLocation();
        }

        return "Unknown target";
    }

    private static String describeParameters(Map<String, String> parameters) {
        if (parameters.isEmpty()) {
            return "";
        }

        return parameters.entrySet().stream()
            .map(entry -> entry.getKey() + "=" + entry.getValue())
            .reduce((left, right) -> left + ", " + right)
            .map(joined -> " (" + joined + ")")
            .orElse("");
    }
}
