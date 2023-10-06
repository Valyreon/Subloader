namespace SubloaderCLI;
public static class ConsoleHelper
{
    public static void WriteExceptionMessage(string message)
    {
        var tmp = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Error occured: ");
        Console.ForegroundColor = tmp;
        Console.WriteLine(message);
    }

    public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Red)
    {
        var tmp = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = tmp;
    }

    public static void WriteLine(string label, string value, ConsoleColor labelColor = ConsoleColor.Cyan)
    {
        var tmp = Console.ForegroundColor;
        Console.ForegroundColor = labelColor;
        Console.Write(label + ": ");
        Console.ForegroundColor = tmp;
        Console.WriteLine(value);
    }

    public static void WriteMessageForFile(string fileName, string message, ConsoleColor fileColor = ConsoleColor.Green)
    {
        var tmp = Console.ForegroundColor;
        Console.ForegroundColor = fileColor;
        Console.Write($"{fileName}: ");
        Console.ForegroundColor = tmp;
        Console.WriteLine(message);
    }
}
