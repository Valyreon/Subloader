using System.CommandLine;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class FileCommand : ICommand
{
    public Command BuildCommand()
    {
        var pathOption = new Option<FileInfo>(
        aliases: ["--path", "-p"],
        description: "The file to download subtitle for.");

        var languageOption = new Option<string>(
        aliases: ["--lang", "-l"],
        () => "en",
        description: "Specify desired language of the subtitle.");

        var usernameOption = new Option<string>(
            aliases: ["--user", "--username", "-u"],
            description: "If specified, you will be prompted to enter your password. " +
            "Command will login and use your token for the entire operation after which it will log you out.");

        var fileDownload = new Command("file", "Download subtitle for single file based on hash.")
        {
            pathOption,
            languageOption,
            usernameOption
        };

        fileDownload.SetHandler((FileInfo path, string language, string username) => DownloadSubtitlesForFile(path, language, username), pathOption, languageOption, usernameOption);

        return fileDownload;
    }

    private static async Task DownloadSubtitlesForFile(FileInfo path, string language, string username)
    {
        if (!path.Exists)
        {
            ConsoleHelper.WriteLine("Specified file does not exist.");
            return;
        }

        var session = string.IsNullOrWhiteSpace(username) ? new() : await Helper.Login(username);

        await Helper.DownloadSubtitlesForFile(path, language, session: session);

        await Helper.Logout(session);
    }
}
