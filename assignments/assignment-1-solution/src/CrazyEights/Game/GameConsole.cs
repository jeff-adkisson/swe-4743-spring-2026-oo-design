namespace CrazyEights.Game;

public static class GameConsole
{
    public static void WriteSeparator(int length = 50)
    {
        WriteLine(new string('=', length));
    }

    public static void Write(string text)
    {
        Console.Write(text);
    }

    public static void WriteLine(string line = "")
    {
        Console.WriteLine(line);
    }

    public static string ReadLine(string line = "")
    {
        Write(line);
        return (Console.ReadLine() ?? "").Trim().ToLower();
    }
}