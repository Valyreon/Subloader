using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubloaderWpf.Models
{
    public class ApplicationSettings
    {
        private static ApplicationSettings instance;

        public static ApplicationSettings GetInstance()
        {
            if (instance == null)
            {
                Refresh();
            }
            return instance;
        }

        public static void Refresh() => instance = LoadApplicationSettings();

        private List<SubtitleLanguage> wantedLanguages;
        public List<SubtitleLanguage> WantedLanguages
        {
            get => wantedLanguages;
            set
            {
                wantedLanguages = value;
                WriteApplicationSettings();
            }
        }

        private ApplicationSettings(List<SubtitleLanguage> langWant) => wantedLanguages = langWant;

        private static ApplicationSettings LoadApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            return LoadApplicationSettings(pathToDefaultConfig);
        }

        private static ApplicationSettings LoadApplicationSettings(string path)
        {
            var langs = new List<SubtitleLanguage>();

            if (!File.Exists(path))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Create(path).Close();
            }

            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var lang = SubtitleLanguage.AllLanguages.Where(s => s.Code == line).SingleOrDefault();
                    if (lang != null)
                    {
                        langs.Add(lang);
                    }
                }
            }

            return new ApplicationSettings(langs);
        }

        private static void WriteApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            WriteApplicationSettings(pathToDefaultConfig);
        }

        private static void WriteApplicationSettings(string path)
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(path));
            using var fileStream = new FileStream(path, FileMode.Create);
            using var writer = new StreamWriter(fileStream);
            foreach (var x in GetInstance().WantedLanguages)
            {
                writer.WriteLine(x.Code);
            }
        }
    }
}
