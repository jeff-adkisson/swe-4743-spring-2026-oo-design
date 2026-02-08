package assignment2solution;

import assignment2solution.userinterface.Application;

import java.io.InputStreamReader;
import java.io.OutputStreamWriter;

/**
 * The main entry point for the tea shop application.
 */
public final class Program {
    private Program() {
    }

    /**
     * The main method which starts the application.
     * System.in and System.out are passed to the application to enable
     * easier testing and separation of concerns.
     */
    public static void main(String[] args) {
        var application = new Application(new InputStreamReader(System.in), new OutputStreamWriter(System.out));
        application.run();
    }
}
