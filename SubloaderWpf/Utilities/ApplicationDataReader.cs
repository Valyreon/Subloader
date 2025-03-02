using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SubloaderWpf.Models;

namespace SubloaderWpf.Utilities;

public static class ApplicationDataReader
{
#if DEBUG
    private static readonly JsonSerializerOptions jsonIndentedOptions = new() { WriteIndented = true };
#endif
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public static event Action Saved;

    private static readonly Lazy<string> ConfigPath = new(GetConfigPath);

    public static async Task SaveSettingsAsync(ApplicationSettings settings)
    {
        await semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(settings
#if DEBUG
                , jsonIndentedOptions
#endif
                );
            await File.WriteAllTextAsync(ConfigPath.Value, json);
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            await Logger.LogExceptionAsync(ex);
        }
        finally
        {
            semaphore.Release();

        }
    }

    public static async Task<ApplicationSettings> LoadSettingsAsync()
    {
        if (!File.Exists(ConfigPath.Value))
        {
            return new ApplicationSettings();
        }

        await semaphore.WaitAsync();
        try
        {
            var text = await File.ReadAllTextAsync(ConfigPath.Value);
            return JsonSerializer.Deserialize<ApplicationSettings>(text);
        }
        catch (Exception ex)
        {
            await Logger.LogExceptionAsync(ex);
        }
        finally
        {
            semaphore.Release();
        }

        return new ApplicationSettings();
    }

    public static ApplicationSettings LoadSettings()
    {
        if (!File.Exists(ConfigPath.Value))
        {
            return new ApplicationSettings();
        }

        semaphore.Wait();
        try
        {
            var text = File.ReadAllText(ConfigPath.Value);
            return JsonSerializer.Deserialize<ApplicationSettings>(text);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
        finally
        {
            semaphore.Release();
        }

        return new ApplicationSettings();
    }

    private static string GetConfigPath()
    {
#if PORTABLE_RELEASE || PORTABLE_DEBUG
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
