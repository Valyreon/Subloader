using System.CommandLine;
using OpenSubtitlesSharp;
using SubloaderCLI.Interfaces;

namespace SubloaderCLI.Commands;
public class LanguagesCommand : ICommand
{
    public Command BuildCommand()
    {
        var searchOption = new Option<string>(
            aliases: new string[] { "--search", "-s" },
            description: "Token to be used for searching the languages. Language names will be in English. For example \"Norwegian\". Search will be case insensitive.");

        var languages = new Command("languages", "Get language codes for all available languages on OpenSubtitles.")
        {
            searchOption
        };

        languages.SetHandler((string search) => GetLanguagesAsync(search), searchOption);

        return languages;
    }

    private static async Task GetLanguagesAsync(string search)
    {
        using var client = new OpenSubtitlesClient(Constants.APIKey);

        var languages = await client.GetLanguagesAsync();

        if(!string.IsNullOrWhiteSpace(search))
        {
            languages = languages.Where(l => l.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        if (!languages.Any())
        {
            ConsoleHelper.WriteLine("No language found for your search token.");
            return;
        }

        foreach(var lang in languages)
        {
            ConsoleHelper.WriteLine(lang.Code, lang.Name, ConsoleColor.Green);
        }
    }
}
