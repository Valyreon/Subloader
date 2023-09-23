using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SubloaderWpf.Models;

namespace SubloaderWpf.Utilities;

public static class SettingsParser
{
    public static event Action Saved;

    public static async Task SaveAsync(ApplicationSettings settings)
    {
        try
        {
            var path = GetConfigPath();
            var json = JsonSerializer.Serialize(settings
#if DEBUG
                , new JsonSerializerOptions { WriteIndented = true }
#endif
                );
            await File.WriteAllTextAsync(path, json);
            Saved?.Invoke();
        }
        catch (Exception)
        {
        }
    }

    public static async Task<ApplicationSettings> LoadAsync()
    {
        var path = GetConfigPath();

        if (!File.Exists(path))
        {
            return new ApplicationSettings();
        }

        var text = await File.ReadAllTextAsync(path);

        try
        {
            return JsonSerializer.Deserialize<ApplicationSettings>(text);
        }
        catch (Exception)
        {
        }

        return new ApplicationSettings();
    }

    private static string GetConfigPath()
    {
#if PORTABLE_RELEASE
        var path = "subLoadConfig.json";
#else
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"SubLoader\config.json");
        var dirFolder = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(dirFolder))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
#endif

        return path;
    }
}
