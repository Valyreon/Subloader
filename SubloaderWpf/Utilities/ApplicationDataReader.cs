using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SubloaderWpf.Models;

namespace SubloaderWpf.Utilities;

public static class ApplicationDataReader
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public static event Action Saved;

    public static async Task SaveSettingsAsync(ApplicationSettings settings)
    {
        await semaphore.WaitAsync();
        try
        {
            var path = GetConfigPath();
            var json = JsonSerializer.Serialize(settings
#if DEBUG
                , new JsonSerializerOptions { WriteIndented = true }
#else
                , new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault }
#endif
                );
            await File.WriteAllTextAsync(path, json);
            Saved?.Invoke();
        }
        catch (Exception)
        {
        }

        semaphore.Release();
    }

    public static async Task<ApplicationSettings> LoadSettingsAsync()
    {
        var path = GetConfigPath();

        if (!File.Exists(path))
        {
            return new ApplicationSettings().Initialize();
        }

        await semaphore.WaitAsync();
        var text = await File.ReadAllTextAsync(path);
        semaphore.Release();
        try
        {
            return JsonSerializer.Deserialize<ApplicationSettings>(text).Initialize();
        }
        catch (Exception)
        {
        }

        return new ApplicationSettings().Initialize();
    }

    public static ApplicationSettings LoadSettings()
    {
        var path = GetConfigPath();

        if (!File.Exists(path))
        {
            return new ApplicationSettings().Initialize();
        }

        semaphore.Wait();
        var text = File.ReadAllText(path);
        semaphore.Release();
        try
        {
            return JsonSerializer.Deserialize<ApplicationSettings>(text).Initialize();
        }
        catch (Exception)
        {
        }

        return new ApplicationSettings().Initialize();
    }

    private static string GetConfigPath()
    {
#if PORTABLE_RELEASE
        var path = "subLoadConfig.json";
#else
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"Subloader\config.json");
        var dirFolder = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(dirFolder))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
#endif

        return path;
    }
}
