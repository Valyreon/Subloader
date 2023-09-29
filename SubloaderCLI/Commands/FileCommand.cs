using System.CommandLine;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class FileCommand : ICommand
{
    public Command BuildCommand()
    {
        var pathOption = new Option<FileInfo>(
        aliases: new string[] { "--path", "-p" },
        description: "The file to download subtitle for.");

        var languageOption = new Option<string>(
        aliases: new string[] { "--lang", "-l" },
        () => "eng",
        description: "Specify desired language of the subtitle.");

        var fileDownload = new Command("file", "Download subtitle for single file based on hash.")
        {
            pathOption,
            languageOption
        };

        fileDownload.SetHandler((FileInfo path, string language) => Helper.DownloadSubtitlesForFile(path, language), pathOption, languageOption);
        return fileDownload;
    }
}
