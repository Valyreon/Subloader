using System.CommandLine;
using OpenSubtitlesSharp;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class DirectoryCommand : ICommand
{
    private static readonly IReadOnlyList<string> subtitleExtensions = new string[] { "srt", "sub", "mpl", "webvtt", "dfxp", "txt" };

    public Command BuildCommand()
    {
        var pathOption = new Option<DirectoryInfo>(
            aliases: new string[] { "--path", "-p" },
            () => new DirectoryInfo("."),
            description: "The folder to scan for video files and download subtitles.");

        var languageOption = new Option<string>(
            aliases: new string[] { "--lang", "-l" },
            () => "en",
            description: "Specify desired language of the subtitles.");

        var recursiveOption = new Option<bool>(
            aliases: new string[] { "--recursive", "-r" },
            () => true,
            description: "Specify whether you want to download subtitles for all files in the directory tree or only root.");

        var overwriteOption = new Option<bool>(
            aliases: new string[] { "--overwrite", "-o" },
            () => false,
            description: "Specify whether you want to override already present subtitle files.");

        var extensionOption = new Option<string>(
            aliases: new string[] { "--ext", "-e" },
            () => "avi|mkv|mp4",
            description: "Specify for which extensions to scan for. Separate extensions with '|'. Default value is 'avi|mkv|mp4'.");

        var usernameOption = new Option<string>(
            aliases: new string[] { "--user", "--username", "-u" },
            description: "If specified, you will be prompted to enter your password. " +
            "Command will login and use your token for the entire operation after which it will log you out.");

        var dirDownload = new Command("dir", "Download subtitles for video files in a directory.")
        {
            pathOption,
            languageOption,
            recursiveOption,
            overwriteOption,
            extensionOption,
            usernameOption
        };

        dirDownload.SetHandler((DirectoryInfo path, bool recursive, bool overwrite, string exts, string language, string username)
            => DownloadSubtitlesForDirectory(path, recursive, overwrite, exts, language, username), pathOption, recursiveOption, overwriteOption, extensionOption, languageOption, usernameOption);

        return dirDownload;
    }

    private static async Task DownloadSubtitlesForDirectory(DirectoryInfo path, bool recursive, bool overwrite, string exts, string language, string username)
    {
        if (!path.Exists)
        {
            ConsoleHelper.WriteExceptionMessage("Specified directory does not exist.");
            return;
        }

        Session session = new();
        if(!string.IsNullOrWhiteSpace(username))
        {
            Console.Write("Password: ");
            var password = Helper.GetPassword();

            if(string.IsNullOrWhiteSpace(password))
            {
                ConsoleHelper.WriteExceptionMessage("Password is empty.");
                return;
            }

            using var client = new OpenSubtitlesClient(Constants.APIKey);
            var loginInfo = await client.LoginAsync(username, password);
            session.Username = username;
            session.Token = loginInfo.Token;
            session.RemainingDownloads = loginInfo.User.RemainingDownloads;
            session.BaseUrl = loginInfo.BaseUrl;
            session.Level = loginInfo.User.Level;
            session.AllowedDownloads = loginInfo.User.AllowedDownloads;
        }

        ConsoleHelper.WriteLine("Login success.", ConsoleColor.Green);
        Console.WriteLine("Remaining downloads: " + session.RemainingDownloads);

        var extensions = exts.Split('|').Select(e => "." + e).ToList();

        Console.WriteLine("Scanning files...");
        var files = GetFilePaths(path.FullName, recursive, extensions, overwrite);
        Console.WriteLine($"Found {files.Count} files. Starting downloads...");
        foreach (var filePath in files)
        {
            var success = await Helper.DownloadSubtitlesForFile(new FileInfo(filePath), language, true, overwrite, session);

            if (!success)
            {
                return;
            }
        }

        if(session != null)
        {
            Console.WriteLine("Remaining downloads for today: " + session.RemainingDownloads);
        }
    }

    private static IReadOnlyList<string> GetFilePaths(string sourcePath, bool recursiveScan, IReadOnlyList<string> extensions, bool overwrite)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new ArgumentException(nameof(sourcePath));
        }

        var filesToScan = new List<string>();

        var directories = new Stack<string>();
        directories.Push(sourcePath);

        while (directories.Any())
        {
            var currentDir = directories.Pop();
            filesToScan.AddRange(Directory.GetFiles(currentDir)
                .Where(f => extensions.Contains(Path.GetExtension(f))
                    && (overwrite || !subtitleExtensions.Any(e => File.Exists(Path.ChangeExtension(f, e))))));

            if (recursiveScan)
            {
                foreach (var subDir in Directory.GetDirectories(currentDir))
                {
                    directories.Push(subDir);
                }
            }
        }

        return filesToScan;
    }
}
