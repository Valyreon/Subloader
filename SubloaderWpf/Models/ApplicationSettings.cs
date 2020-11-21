using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubloaderWpf.Models
{
    public class ApplicationSettings
    {
        private static ApplicationSettings instance;

        private bool isByNameChecked;
        private bool isByHashChecked = true;

        public bool IsByNameChecked
        {
            get => isByNameChecked;

            set
            {
                isByNameChecked = value;
                IsDirty = true;
            }
        }

        public bool IsByHashChecked
        {
            get => isByHashChecked;

            set
            {
                isByHashChecked = value;
                IsDirty = true;
            }
        }

        [JsonIgnore]
        public bool IsDirty { get; set; }

        public IList<SubtitleLanguage> WantedLanguages { get; set; }

        private ApplicationSettings(List<SubtitleLanguage> langWant) => WantedLanguages = langWant;

        public ApplicationSettings() { }

        public static ApplicationSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    Load();
                }
                return instance;
            }
        }

        public static void Load() => instance = LoadApplicationSettings();

        private static ApplicationSettings LoadApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.json");
            return LoadApplicationSettings(pathToDefaultConfig);
        }

        private static ApplicationSettings LoadApplicationSettings(string path)
        {
            if (!File.Exists(path))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Create(path).Close();
            }

            using var fileStream = new FileStream(path, FileMode.Open);
            using var reader = new StreamReader(fileStream);

            try
            {
                var x = JsonSerializer.Deserialize<ApplicationSettings>(reader.ReadToEnd());

                if (x != null)
                {
                    x.IsDirty = false;
                    return x;
                }
            }
            catch (Exception)
            {

            }

            var ret = new ApplicationSettings(new List<SubtitleLanguage>())
            {
                IsDirty = true
            };
            return ret;
        }

        private static void WriteApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.json");

            WriteApplicationSettings(pathToDefaultConfig);
        }

        private static void WriteApplicationSettings(string path)
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(path));

            using var fileStream = new FileStream(path, FileMode.Create);
            using var writer = new StreamWriter(fileStream);

            var json = JsonSerializer.Serialize(Instance);
            Instance.IsDirty = false;
            writer.Write(json);
        }

        public void Save() => WriteApplicationSettings();

        public void SaveIfDirty()
        {
            if (Instance.IsDirty)
            {
                Save();
            }
        }
    }
}
