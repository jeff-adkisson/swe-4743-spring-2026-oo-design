package edu.kennesaw.crazy8s.game;

import java.util.Scanner;

/**
 * Provides console input and output helpers for the game.
 */
public final class GameConsole {
    private static final char SEPARATOR_CHAR = '=';
    private static final Scanner SCANNER = new Scanner(System.in);

    private GameConsole() {
    }

    public static void writeSeparator(int length, int blankLinesAround) {
        for (int i = 0; i < blankLinesAround; i++) {
            writeLine();
        }
        writeLine(String.valueOf(SEPARATOR_CHAR).repeat(length));
        for (int i = 0; i < blankLinesAround; i++) {
            writeLine();
        }
    }

    public static void writeSeparator() {
        writeSeparator(50, 0);
    }

    public static void write(String text) {
        System.out.print(text);
    }

    public static void writeLine() {
        System.out.println();
    }

    public static void writeLine(String line) {
        System.out.println(line);
    }

    public static String readLine(String line) {
        write(line);
        if (!SCANNER.hasNextLine()) {
            return "";
        }

        return SCANNER.nextLine().trim().toLowerCase();
    }

    public static String readLineRaw(String line) {
        write(line);
        if (!SCANNER.hasNextLine()) {
            return "";
        }

        return SCANNER.nextLine();
    }

    public static boolean promptYesNo(String line) {
        boolean isValidInput = false;
        String output = "";
        while (!isValidInput) {
            output = readLine(line);
            if (output.equals("y") || output.equals("yes") || output.equals("n") || output.equals("no")) {
                isValidInput = true;
            }

            if (!isValidInput) {
                writeLine("- Invalid choice! Please enter Y or N.");
            }
        }

        return output.equals("y") || output.equals("yes");
    }

    public static void clear() {
        System.out.print("\033[H\033[2J");
        System.out.flush();
    }
}
