using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SubloaderWpf.Models
{
    public class ApplicationSettings
    {
        private static ApplicationSettings instance;

        private bool isByNameChecked;
        private bool isByHashChecked = true;

        private ApplicationSettings(List<SubtitleLanguage> langWant) => WantedLanguages = langWant;

        [JsonConstructor]
        public ApplicationSettings()
        {

        }

        public static ApplicationSettings GetInstance()
        {
            if (instance == null)
            {
                Refresh();
            }
            return instance;
        }

        public static void Refresh() => instance = LoadApplicationSettings();

        public List<SubtitleLanguage> WantedLanguages { get; set; }

        public bool IsDirty { get; private set; }

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
                var x = JsonConvert.DeserializeObject<ApplicationSettings>(reader.ReadToEnd());

                if (x != null)
                {
                    x.IsDirty = false;
                    return x;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            var settings = GetInstance();
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            settings.IsDirty = false;
            writer.Write(json);
        }

        public void Save() => WriteApplicationSettings();
    }
}
