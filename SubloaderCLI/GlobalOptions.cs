namespace SubloaderCLI;
public static class GlobalOptions
{
    public static bool ForceDefaultApiUrl { get; set; }
    public static Session Session { get; set; } = new();
}
