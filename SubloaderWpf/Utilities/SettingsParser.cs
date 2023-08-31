using System;
using System.IO;
using System.Text.Json;

namespace SubloaderWpf.Utilities
{
    public static class SettingsParser
    {
        public static event Action Saved;

        public static void Save(Settings settings)
        {
            try
            {
                var path = GetConfigPath();

                using var fileStream = new FileStream(path, FileMode.Create);
                using var writer = new StreamWriter(fileStream);

                var json = JsonSerializer.Serialize(settings);
                writer.Write(json);
                Saved?.Invoke();
            }
            catch (Exception)
            {
            }
        }

        public static Settings Load()
        {
            var path = GetConfigPath();

            if (!File.Exists(path))
            {
                return new Settings();
            }

            using var fileStream = new FileStream(path, FileMode.Open);
            using var reader = new StreamReader(fileStream);

            try
            {
                var settingsJson = JsonSerializer.Deserialize<Settings>(reader.ReadToEnd());

                if (settingsJson != null)
                {
                    return settingsJson;
                }
            }
            catch (Exception)
            {
            }

            return new Settings();
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
}
