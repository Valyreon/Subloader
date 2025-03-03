using System.CommandLine;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class DirectoryCommand : ICommand
{
    private static readonly IReadOnlyList<string> subtitleExtensions = ["srt", "sub", "mpl", "webvtt", "dfxp", "txt"];

    public Command BuildCommand()
    {
        var pathOption = new Option<DirectoryInfo>(
            aliases: ["--path", "-p"],
            () => new DirectoryInfo("."),
            description: "The folder to scan for video files and download subtitles.");

        var languageOption = new Option<string>(
            aliases: ["--lang", "-l"],
            () => "en",
            description: "Specify desired language of the subtitles.");

        var recursiveOption = new Option<bool>(
            aliases: ["--recursive", "-r"],
            description: "If true, subfolders will also be scanned for matching files.");

        var overwriteOption = new Option<bool>(
            aliases: ["--overwrite", "-o"],
            description: "Specify whether you want to override already present subtitle files.");

        var extensionOption = new Option<string>(
            aliases: ["--ext", "-e"],
            () => "avi|mkv|mp4",
            description: "Specify for which video file extensions to scan for. Separate extensions with '|'.");

        var usernameOption = new Option<string>(
            aliases: ["--user", "--username", "-u"],
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

        dirDownload.SetHandler(DownloadSubtitlesForDirectory, pathOption, recursiveOption, overwriteOption, extensionOption, languageOption, usernameOption);
        return dirDownload;
    }

    private static async Task DownloadSubtitlesForDirectory(DirectoryInfo path, bool recursive, bool overwrite, string exts, string language, string username)
    {
        if (!path.Exists)
        {
            ConsoleHelper.WriteExceptionMessage("Specified directory does not exist.");
            return;
        }

        var session = string.IsNullOrWhiteSpace(username) ? new() : await Helper.Login(username);

        var extensions = exts.Split('|').Select(e => "." + e).ToList();

        Console.WriteLine("Scanning files...");
        var files = GetFilePaths(path.FullName, recursive, extensions, overwrite);

        if(files.Count == 0)
        {
            Console.WriteLine("No matching files found for specified directory.");
        }
        else
        {
            Console.WriteLine($"Found {files.Count} files. Starting downloads...");
            foreach (var filePath in files)
            {
                var success = await Helper.DownloadSubtitlesForFile(new FileInfo(filePath), language, true, overwrite, session);

                if (!success)
                {
                    return;
                }
            }

            if (session != null)
            {
                Console.WriteLine("Remaining downloads for today: " + session.RemainingDownloads);
            }
        }

        await Helper.Logout(session);
    }

    private static List<string> GetFilePaths(string sourcePath, bool recursiveScan, IReadOnlyList<string> extensions, bool overwrite)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new ArgumentException("Source path must be specified.", nameof(sourcePath));
        }

        var filesToScan = new List<string>();

        var directories = new Stack<string>();
        directories.Push(sourcePath);

        while (directories.Count != 0)
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
