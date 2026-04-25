using System.CommandLine;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class FileCommand : ICommand
{
    public bool SupportsLogin => true;

    public Command BuildCommand()
    {
        var pathOption = new Option<FileInfo>("--path", "-p")
        {
            Description = "The file to download subtitle for."
        };

        var languageOption = new Option<string>("--lang", "-l")
        {
            DefaultValueFactory = r => "en",
            Description = "Specify desired language of the subtitle."
        };

        var fileDownload = new Command("file", "Download subtitle for single file based on hash.")
        {
            pathOption,
            languageOption,
        };

        fileDownload.SetAction(pr =>
            DownloadSubtitlesForFile(
                pr.GetValue(pathOption),
                pr.GetValue(languageOption)));

        return fileDownload;
    }

    private static async Task DownloadSubtitlesForFile(FileInfo path, string language)
    {
        if (!path.Exists)
        {
            ConsoleHelper.WriteLine("Specified file does not exist.");
            return;
        }

        var session = GlobalOptions.Session;

        await Helper.DownloadSubtitlesForFile(path, language, session: session);

        await Helper.Logout(session);
    }
}
