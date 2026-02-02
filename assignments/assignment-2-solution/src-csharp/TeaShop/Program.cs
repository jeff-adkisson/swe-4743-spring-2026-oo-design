using Assignment2Solution.UserInterface;

namespace Assignment2Solution;

/// <summary>
///     The main entry point for the tea shop application.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main method which starts the application.
    /// </summary>
    public static void Main()
    {
        var application = new Application(Console.In, Console.Out);
        application.Run();
    }
}