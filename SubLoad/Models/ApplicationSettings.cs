using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubLoad.Models
{
    public class ApplicationSettings
    {
        private static ApplicationSettings _instance;

        public static ApplicationSettings GetInstance()
        {
            if (_instance == null)
            {
                Refresh();
            }
            return _instance;
        }

        public static void Refresh()
        {
            _instance = LoadApplicationSettings();
        }

        private List<SubtitleLanguage> wantedLanguages;
        public List<SubtitleLanguage> WantedLanguages
        {
            get
            {
                return wantedLanguages;
            }
            set
            {
                wantedLanguages = value;
                WriteApplicationSettings();
            }
        }

        private ApplicationSettings(List<SubtitleLanguage> langWant)
        {
            wantedLanguages = langWant;
        }

        private static ApplicationSettings LoadApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            return LoadApplicationSettings(pathToDefaultConfig);
        }

        private static ApplicationSettings LoadApplicationSettings(string path)
        {
            List<SubtitleLanguage> langs = new List<SubtitleLanguage>();

            if (!File.Exists(path))
                return null;

            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    langs.Add(SubtitleLanguage.AllLanguages.Where(s => s.Code == line).First());
                }
            }

            return new ApplicationSettings(langs);
        }

        private static void WriteApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            WriteApplicationSettings(pathToDefaultConfig);
        }

        private static void WriteApplicationSettings(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                foreach (var x in GetInstance().WantedLanguages)
                {
                    writer.WriteLine(x.Code);
                }
            }
        }
    }
}
