using System.CommandLine;
using F23.StringSimilarity;
using SuppliersLibrary.OpenSubtitles;

namespace SubloaderCLI;

public static class Program
{
    private static readonly IReadOnlyList<string> subtitleExtensions = new List<string>
    {
        ".srt",
        ".sub",
        ".ssa",
        ".smi",
        ".vtt",
    };

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Subloader CLI application. It uses OpenSubtitles REST API to download subs for your video files. It will try to choose best sub for you.");

        rootCommand.AddCommand(BuildFileCommand());
        rootCommand.AddCommand(BuildDirectoryCommand());

        return await rootCommand.InvokeAsync(args);
    }

    internal static Command BuildDirectoryCommand()
    {
        var pathOption = new Option<DirectoryInfo>(
            aliases: new string[] { "--path", "-p" },
            () => new DirectoryInfo("."),
            description: "The folder to scan for video files and download subtitles.");

        var languageOption = new Option<string>(
            aliases: new string[] { "--lang", "-l" },
            () => "eng",
            description: "Specify desired language of the subtitle.");

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

        var dirDownload = new Command("dir", "Download subtitles for video files in a directory.")
        {
            pathOption,
            languageOption,
            recursiveOption,
            overwriteOption,
            extensionOption
        };

        dirDownload.SetHandler((DirectoryInfo path, bool recursive, bool overwrite, string exts, string language)
            => DownloadSubtitlesForDirectory(path, recursive, overwrite, exts, language), pathOption, recursiveOption, overwriteOption, extensionOption, languageOption);

        return dirDownload;
    }

    internal static Command BuildFileCommand()
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

        fileDownload.SetHandler((FileInfo path, string language) => DownloadSubtitlesForFile(path, language), pathOption, languageOption);
        return fileDownload;
    }

    internal static async Task<bool> DownloadSubtitlesForFile(FileInfo path, string language, bool folderMode = false, bool overwrite = true)
    {
        try
        {
            var client = new OpenSubtitlesClient();
            var results = await client.SearchForFileAsync(
                path.FullName,
                true,
                true,
                language,
                true);

            if (results == null)
            {
                Console.WriteLine("Server error. Try later.");
                return false;
            }
            else if (results.Count == 0)
            {
                if (folderMode)
                {
                    WriteMessageForFile(path.Name, "No subtitles found.");
                }
                else
                {
                    Console.WriteLine("No subtitles found for specified file.");
                }
                return true;
            }
            else
            {
            }

            var leven = new Levenshtein();
            var levenResults = results.Select(ResultItem => (ResultItem, leven.Distance(ResultItem.Name, path.Name))).OrderBy(i => i.Item2).Select(i => i.ResultItem);
            foreach (var item in levenResults)
            {
                var fileName = Path.ChangeExtension(path.FullName, item.Format);

                if (await client.Download(item, fileName))
                {
                    WriteMessageForFile(path.Name, "Subtitle downloaded.");
                    return true;
                }
            }

            WriteMessageForFile(path.Name, "No valid subtitles found.");
        }
        catch (Exception ex)
        {
            WriteExceptionMessage(ex.Message);
            return false;
        }

        return true;
    }

    private static async Task DownloadSubtitlesForDirectory(DirectoryInfo path, bool recursive, bool overwrite, string exts, string language)
    {
        if (!path.Exists)
        {
            WriteExceptionMessage("Specified directory does not exist.");
            return;
        }

        var extensions = exts.Split('|').Select(e => "." + e).ToList();

        Console.WriteLine("Scanning files...");
        var files = GetFilePaths(path.FullName, recursive, extensions, overwrite);
        Console.WriteLine($"Found {files.Count} files. Starting downloads...");
        foreach (var filePath in files)
        {
            var success = await DownloadSubtitlesForFile(new FileInfo(filePath), language, true, overwrite);

            if (!success)
            {
                return;
            }
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

    private static void WriteExceptionMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Error occured: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    private static void WriteMessageForFile(string fileName, string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{fileName}: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }
}
