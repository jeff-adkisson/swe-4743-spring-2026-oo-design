namespace CrazyEights.Game;

public static class GameConsole
{
    private const char SeparatorChar = '=';

    public static void WriteSeparator(int length = 50, int blankLinesAround = 0)
    {
        for (var i = 0; i < blankLinesAround; i++) WriteLine();
        WriteLine(new string(SeparatorChar, length));
        for (var i = 0; i < blankLinesAround; i++) WriteLine();
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

    public static bool PromptYesNo(string line = "")
    {
        var isValidInput = false;
        var output = "";
        while (!isValidInput)
        {
            output = ReadLine(line);
            if (output == "y" || output == "yes" || output == "n" || output == "no")
                isValidInput = true;

            if (!isValidInput)
                WriteLine("- Invalid choice! Please enter Y or N.");
        }

        return output == "y" || output == "yes";
    }

    public static void Clear()
    {
        Console.Clear();
    }
}