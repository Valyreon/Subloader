using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenSubtitlesSharp;
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
            return new ApplicationSettings();
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

        return new ApplicationSettings();
    }

    public static async Task<IReadOnlyList<SubtitleLanguage>> LoadLanguagesAsync()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"SubLoader\languages.json");

        if (!File.Exists(path))
        {
            return null;
        }

        await semaphore.WaitAsync();
        var text = await File.ReadAllTextAsync(path);
        semaphore.Release();
        try
        {
            return JsonSerializer.Deserialize<IReadOnlyList<SubtitleLanguage>>(text);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<IReadOnlyList<string>> LoadFormatsAsync()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"SubLoader\formats.json");

        if (!File.Exists(path))
        {
            return null;
        }

        await semaphore.WaitAsync();
        var text = await File.ReadAllTextAsync(path);
        semaphore.Release();
        try
        {
            return JsonSerializer.Deserialize<IReadOnlyList<string>>(text);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task SaveLanguagesAsync(IEnumerable<SubtitleLanguage> languages)
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"SubLoader\languages.json");

        await semaphore.WaitAsync();

        var serialized = JsonSerializer.Serialize(languages);

        semaphore.Release();
        try
        {
            File.WriteAllText(path, serialized);
        }
        catch (Exception)
        {
        }
    }

    public static async Task SaveFormatsAsync(IEnumerable<string> formats)
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(appDataFolder, @"SubLoader\formats.json");

        await semaphore.WaitAsync();

        var serialized = JsonSerializer.Serialize(formats);

        semaphore.Release();
        try
        {
            await File.WriteAllTextAsync(path, serialized);
        }
        catch (Exception)
        {
        }
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
