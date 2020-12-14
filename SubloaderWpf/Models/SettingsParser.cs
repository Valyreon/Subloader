using System;
using System.IO;
using System.Text.Json;

namespace SubloaderWpf.Models
{
    public static class SettingsParser
    {
        public static event Action Saved;

        public static void Save(Settings settings)
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appDataFolder, @"SubLoader\config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using var fileStream = new FileStream(path, FileMode.Create);
            using var writer = new StreamWriter(fileStream);

            var json = JsonSerializer.Serialize(settings);
            writer.Write(json);
            Saved?.Invoke();
        }

        public static Settings Load()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appDataFolder, @"SubLoader\config.json");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Create(path).Close();
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
    }
}
