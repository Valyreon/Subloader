using SubLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLoad.Models
{
    public class OpenSubtitlesWorker
    {
        public async Task<List<SubtitleEntry>> Search(string path)
        {
            SearchSubtitlesResponse ssre = null;

            OSIntermediary messenger = new OSIntermediary();
            await messenger.OSLogIn();
            ssre = await messenger.SearchOS(path, "all");
            await Task.Run(() => messenger.OSLogOut());


            if (ssre == null)
            {
                return null;
            }
            else if (ssre != null && (ssre.data == null || ssre.data.Length == 0))
            {
                return new List<SubtitleEntry>();
            }
            else
            {
                List<SubtitleEntry> result = new List<SubtitleEntry>();
                foreach (var x in ssre.data)
                {
                    var settings = ApplicationSettings.GetInstance();
                    if (settings.WantedLanguages == null || settings.WantedLanguages.Count == 0 || settings.WantedLanguages.Where((subLang) => subLang.Name == x.LanguageName).Any())
                    {
                        result.Add(new SubtitleEntry(x.SubFileName, x.LanguageName, int.Parse(x.IDSubtitleFile), x.SubFormat));
                    }
                }
                return result;
            }
        }

        public async Task<bool> Download(SubtitleEntry selected, string destination)
        {
            if (selected != null)
            {
                OSIntermediary messenger = new OSIntermediary();
                await messenger.OSLogIn();
                byte[] subtitleStream = await messenger.DownloadSubtitle(selected.GetSubtitleFileID());
                await Task.Run(() => messenger.OSLogOut());
                if (subtitleStream != null)
                {
                    File.WriteAllBytes(destination, subtitleStream);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }
    }
}

