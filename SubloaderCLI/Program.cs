using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using OpenSubtitlesSharp;
using SubloaderCLI;

var rootCommand = new RootCommand(
    "Subloader CLI application. It uses OpenSubtitles REST API to download subs for your video files.");

foreach (var command in Helper.GetImplementedCommands().Select(c => c.BuildCommand()))
{
    rootCommand.AddCommand(command);
}

var forceDefaultApiUrlOption = new Option<bool>(
            aliases: ["--force_default_api_url", "-fdu"],
            description: "If flag is true, app will use the default api url for searching even if user is VIP. This is for VIP users that have a specific issue.");

rootCommand.AddGlobalOption(forceDefaultApiUrlOption);

var parser = new CommandLineBuilder(rootCommand)
    .UseExceptionHandler((e, _) =>
    {
        if (e is RequestFailedException rfe)
        {
            ConsoleHelper.WriteLine(rfe.Message, ConsoleColor.Red);
        }
        else
        {
            ConsoleHelper.WriteLine(e.Message);
            ConsoleHelper.WriteLine(e.StackTrace);
        }
    }, errorExitCode: 1)
    .UseParseErrorReporting()
    .CancelOnProcessTermination()
    .UseHelp()
    .AddMiddleware(context =>
    {
        var globalOptionValue = context.ParseResult.GetValueForOption(forceDefaultApiUrlOption);
        GlobalOptions.ForceDefaultApiUrl = globalOptionValue;
    })
    .Build();

var result = await parser.InvokeAsync(args);
Console.WriteLine();
return result;
