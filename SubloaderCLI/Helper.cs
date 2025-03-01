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
            using var client = new OpenSubtitlesClient(Constants.APIKey, session?.Token, session.IsVIP, Constants.UserAgent);
            var results = await client.SearchAsync(path.FullName, new SearchParameters
            {
                Languages = [language]
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
            var levenResults = results.Items.Select(ResultItem => (ResultItem, leven.DistanceFrom(ResultItem.Information.Release)))
                .OrderBy(i => i.ResultItem.Information.IsHashMatch == true ? 0 : 1)
                .ThenBy(i => i.Item2)
                .Select(i => i.ResultItem);
            foreach (var item in levenResults)
            {
                var downloadInfo = await client.GetDownloadInfoAsync(item.Information.Files[0].FileId.Value);
                var extension = Path.GetExtension(downloadInfo.FileName);
                var destination = Path.ChangeExtension(path.FullName, extension);

                if (File.Exists(destination) && !overwrite)
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
        catch (RequestFailedException ex)
        {
            ConsoleHelper.WriteExceptionMessage(ex.Message);
            return false;
        }
        catch (Exception)
        {
            ConsoleHelper.WriteLine("Something is wrong with the OS server. Please try again later.");
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
        var passwordPrompt = "Password: ";
        Console.Write(passwordPrompt);

        var password = "";
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey(true);

            // Ignore any key that isn't a printable character or Enter.
            if (char.IsControl(keyInfo.KeyChar) && keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace)
            {
                continue;
            }

            // Handle backspace (remove the last character).
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                // generate stars for one less character
                var hiddenPassword = string.Empty;
                for (var i = 0; i < password.Length - 1; ++i)
                {
                    hiddenPassword += '*';
                }

                // remove last char from password
                password = password[0..^1];

                // we need to clean the line first
                Console.Write("\r");
                for (var i = 0; i < (password.Length + 1 + passwordPrompt.Length); ++i)
                {
                    Console.Write(' ');
                }

                Console.Write("\r" + passwordPrompt + hiddenPassword);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        }
        while (keyInfo.Key != ConsoleKey.Enter);

        Console.WriteLine();

        return password;
    }

    public static async Task<Session> Login(string username)
    {
        var password = GetPassword();

        if (string.IsNullOrWhiteSpace(password))
        {
            ConsoleHelper.WriteExceptionMessage("Password is empty.");
            return null;
        }

        using var client = new OpenSubtitlesClient(Constants.APIKey, null, false, Constants.UserAgent);
        var loginInfo = await client.LoginAsync(username, password);

        var session = new Session
        {
            Username = username,
            Token = loginInfo.Token,
            RemainingDownloads = loginInfo.User.RemainingDownloads,
            BaseUrl = loginInfo.BaseUrl,
            Level = loginInfo.User.Level,
            AllowedDownloads = loginInfo.User.AllowedDownloads,
            IsVIP = loginInfo.User.Vip
        };

        ConsoleHelper.WriteLine("Login success.", ConsoleColor.Green);
        ConsoleHelper.WriteLine("Level", session.Level, ConsoleColor.Magenta);
        Console.WriteLine();

        return session;
    }

    public static async Task Logout(Session session)
    {
        if (session?.Token == null)
        {
            return;
        }

        using var client = new OpenSubtitlesClient(Constants.APIKey, session.Token, session.IsVIP, Constants.UserAgent);
        await client.LogoutAsync();
        ConsoleHelper.WriteLine($"Logged out.", ConsoleColor.Green);
    }
}
