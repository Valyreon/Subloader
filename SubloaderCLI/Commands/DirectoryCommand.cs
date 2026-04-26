using System.CommandLine;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class DirectoryCommand : ICommand
{
    private static readonly IReadOnlyList<string> subtitleExtensions = ["srt", "sub", "mpl", "webvtt", "dfxp", "txt"];

    public bool SupportsLogin => true;

    public Command BuildCommand()
    {
        var pathOption = new Option<DirectoryInfo>("--path", "-p")
        {
            Description = "The folder to scan for video files and download subtitles.",
            DefaultValueFactory = r => new DirectoryInfo(".")
        };

        var languageOption = new Option<string>("--lang", "-l")
        {
            Description = "Specify desired language of the subtitles.",
            DefaultValueFactory = r => "en"
        };

        var recursiveOption = new Option<bool>("--recursive", "-r")
        {
            Description = "If true, subfolders will also be scanned for matching files."
        };

        var overwriteOption = new Option<bool>("--overwrite", "-o")
        {
            Description = "Specify whether you want to override already present subtitle files."
        };

        var extensionOption = new Option<string>("--ext", "-e")
        {
            Description = "Specify for which video file extensions to scan for. Separate extensions with '|'.",
            DefaultValueFactory = r => "avi|mkv|mp4"
        };

        var dirDownload = new Command("dir", "Download subtitles for video files in a directory.")
        {
            pathOption,
            languageOption,
            recursiveOption,
            overwriteOption,
            extensionOption,
        };

        dirDownload.SetAction(pr =>
            DownloadSubtitlesForDirectory(
                pr.GetValue(pathOption),
                pr.GetValue(recursiveOption),
                pr.GetValue(overwriteOption),
                pr.GetValue(extensionOption),
                pr.GetValue(languageOption)));

        return dirDownload;
    }

    private static async Task DownloadSubtitlesForDirectory(DirectoryInfo path, bool recursive, bool overwrite, string exts, string language)
    {
        if (!path.Exists)
        {
            ConsoleHelper.WriteExceptionMessage("Specified directory does not exist.");
            return;
        }

        var session = GlobalOptions.Session;

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
