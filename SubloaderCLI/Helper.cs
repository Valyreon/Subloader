using System.Reflection;
using Fastenshtein;
using OpenSubtitlesSharp;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI;
public static class Helper
{
    internal static async Task<bool> DownloadSubtitlesForFile(FileInfo path, string language, bool folderMode = false, bool overwrite = true, Session session = null)
    {
        try
        {
            using var client = new OpenSubtitlesClient(Constants.APIKey, session?.Token);
            var results = await client.SearchAsync(path.FullName, new SearchParameters
            {
                Languages = new List<string> { language }
            });

            if (results == null)
            {
                Console.WriteLine("Server error. Try later.");
                return false;
            }
            else if (results.Items.Count == 0)
            {
                if (folderMode)
                {
                    ConsoleHelper.WriteMessageForFile(path.Name, "No subtitles found.");
                }
                else
                {
                    Console.WriteLine("No subtitles found for specified file.");
                }
                return true;
            }

            var leven = new Levenshtein(Path.GetFileNameWithoutExtension(path.Name));
            var levenResults = results.Items.Select(ResultItem => (ResultItem, leven.DistanceFrom(ResultItem.Information.Release))).OrderBy(i => i.Item2).Select(i => i.ResultItem);
            foreach (var item in levenResults)
            {
                var downloadInfo = await client.GetDownloadInfoAsync(item.Information.Files[0].FileId.Value);
                var extension = Path.GetExtension(downloadInfo.FileName);
                var destination = Path.ChangeExtension(path.FullName, extension);

                if(File.Exists(destination) && !overwrite)
                {
                    return true;
                }

                File.WriteAllBytes(destination, await GetRawFileAsync(downloadInfo.Link));

                if (session != null)
                {
                    session.RemainingDownloads = downloadInfo.Remaining;
                }
                ConsoleHelper.WriteLine(path.Name, "Subtitle downloaded.", ConsoleColor.Green);
                return true;
            }

            ConsoleHelper.WriteMessageForFile(path.Name, "No valid subtitles found.", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteExceptionMessage(ex.Message);
            return false;
        }

        return true;
    }

    private static async Task<byte[]> GetRawFileAsync(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsByteArrayAsync()
            : null;
    }

    public static List<ICommand> GetImplementedCommands()
    {
        // Get all types in the assembly
        var assembly = Assembly.GetExecutingAssembly();
        var typesInAssembly = assembly.GetTypes();

        // Find types that implement ICommand
        var implementingTypes = typesInAssembly.Where(
            type => typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface);

        // Create instances of implementing types
        var instances = new List<ICommand>(5);

        foreach (var implementingType in implementingTypes)
        {
            if (Activator.CreateInstance(implementingType) is ICommand instance)
            {
                instances.Add(instance);
            }
        }

        return instances;
    }

    public static string GetPassword()
    {
        var password = "";
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey(true);

            // Ignore any key that isn't a printable character or Enter.
            if (char.IsControl(keyInfo.KeyChar) && keyInfo.Key != ConsoleKey.Enter)
            {
                continue;
            }

            // Handle backspace (remove the last character).
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
            }
            else if (keyInfo.Key != ConsoleKey.Enter)
            {
                password += keyInfo.KeyChar;
            }
        }
        while (keyInfo.Key != ConsoleKey.Enter);

        Console.WriteLine();

        return password;
    }

    public static async Task<Session> Login(string username)
    {
        Console.Write("Password: ");
        var password = GetPassword();

        if (string.IsNullOrWhiteSpace(password))
        {
            ConsoleHelper.WriteExceptionMessage("Password is empty.");
            return null;
        }

        using var client = new OpenSubtitlesClient(Constants.APIKey);
        var loginInfo = await client.LoginAsync(username, password);

        var session = new Session
        {
            Username = username,
            Token = loginInfo.Token,
            RemainingDownloads = loginInfo.User.RemainingDownloads,
            BaseUrl = loginInfo.BaseUrl,
            Level = loginInfo.User.Level,
            AllowedDownloads = loginInfo.User.AllowedDownloads
        };

        ConsoleHelper.WriteLine("Login success.", ConsoleColor.Green);
        ConsoleHelper.WriteLine("Remaining downloads: ", session.RemainingDownloads.ToString(), ConsoleColor.Magenta);
        ConsoleHelper.WriteLine("Level: ", session.Level,  ConsoleColor.Magenta);
        Console.WriteLine();

        return session;
    }
}
