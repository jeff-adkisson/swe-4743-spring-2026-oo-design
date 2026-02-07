using Assignment2Solution.UserInterface;

namespace Assignment2Solution;

/// <summary>
///     The main entry point for the tea shop application.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main method which starts the application.
    ///     Console.In and Console.Out are passed to the application to enable
    ///     easier testing and separation of concerns.
    /// </summary>
    public static void Main()
    {
        var application = new Application(Console.In, Console.Out);
        application.Run();
    }
}