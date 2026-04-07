package edu.ksu.swe4743.smarthomescenes.repository;

import java.util.LinkedHashMap;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

final class ParameterJson {
    private static final Pattern ENTRY_PATTERN =
        Pattern.compile("\"((?:\\\\.|[^\"])*)\"\\s*:\\s*\"((?:\\\\.|[^\"])*)\"");

    private ParameterJson() {
    }

    static String toJson(Map<String, String> parameters) {
        if (parameters.isEmpty()) {
            return "{}";
        }

        var parts = parameters.entrySet().stream()
            .map(entry -> "\"" + escape(entry.getKey()) + "\":\"" + escape(entry.getValue()) + "\"")
            .toList();

        return "{" + String.join(",", parts) + "}";
    }

    static Map<String, String> fromJson(String json) {
        var trimmed = json == null ? "" : json.trim();
        if (trimmed.isEmpty() || trimmed.equals("{}")) {
            return Map.of();
        }

        var results = new LinkedHashMap<String, String>();
        Matcher matcher = ENTRY_PATTERN.matcher(trimmed);
        while (matcher.find()) {
            results.put(unescape(matcher.group(1)), unescape(matcher.group(2)));
        }

        return Map.copyOf(results);
    }

    private static String escape(String input) {
        return input
            .replace("\\", "\\\\")
            .replace("\"", "\\\"");
    }

    private static String unescape(String input) {
        return input
            .replace("\\\"", "\"")
            .replace("\\\\", "\\");
    }
}
