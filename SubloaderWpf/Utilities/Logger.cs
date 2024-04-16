using System;
using System.IO;
using System.Threading.Tasks;

namespace SubloaderWpf.Utilities;
public static class Logger
{
    public static Task LogExceptionAsync(Exception exception)
    {
#if PORTABLE_RELEASE || PORTABLE_DEBUG
        return Task.CompletedTask;
#else
        var (path, text) = GetLogParameters(exception);
        return File.WriteAllTextAsync(path, text);
#endif
    }

    public static void LogException(Exception exception)
    {
#if PORTABLE_RELEASE || PORTABLE_DEBUG
        return Task.CompletedTask;
#else
        var (path, text) = GetLogParameters(exception);
        File.WriteAllText(path, text);
#endif
    }

    private static (string Path, string Text) GetLogParameters(Exception exception)
    {
        var logDirectory = GetLogsDirectory();
        var currentTimeKey = DateTime.UtcNow.ToString("yyyyMMddTHHmmss_fff");
        var path = Path.Combine(logDirectory.FullName, currentTimeKey + "_" + exception.GetType().Name + ".txt");
        var text = $"{exception.Message}\n\n{exception}";
        return (path, text);
    }

    public static DirectoryInfo GetLogsDirectory()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"Subloader\Logs");

        return !Directory.Exists(path)
            ? Directory.CreateDirectory(path)
            : new DirectoryInfo(path);
    }
}
