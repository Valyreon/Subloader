using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubLoad.Models
{
    public static class ApplicationSettings
    {
        public static List<SubtitleLanguage> LoadApplicationSettings()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            return LoadApplicationSettings(pathToDefaultConfig);
        }

        public static List<SubtitleLanguage> LoadApplicationSettings(string path)
        {
            List<SubtitleLanguage> langs = new List<SubtitleLanguage>();

            if (!File.Exists(path))
                return null;

            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    langs.Add(SubtitleLanguage.AllLanguages.Where(s => s.Code == line).First());
                }
            }

            return langs;
        }

        public static void WriteApplicationSettings(IEnumerable<SubtitleLanguage> list)
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pathToDefaultConfig = Path.Combine(appDataFolder, @"SubLoader\config.cfg");
            WriteApplicationSettings(pathToDefaultConfig, list);
        }

        public static void WriteApplicationSettings(string path, IEnumerable<SubtitleLanguage> list)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var fileStream = new FileStream(path, FileMode.Create))
            using(var writer = new StreamWriter(fileStream))
            {
                foreach(var x in list)
                {
                    writer.WriteLine(x.Code);
                }
            }
        }
    }
}
