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

var parser = new CommandLineBuilder(rootCommand)
    .UseExceptionHandler((e, context) =>
    {
        if (e is RequestFailedException rfe)
        {
            ConsoleHelper.WriteLine(rfe.Message, ConsoleColor.Red);
        }
        else
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }, errorExitCode: 1)
    .UseEnvironmentVariableDirective()
    .UseParseDirective()
    .UseSuggestDirective()
    .RegisterWithDotnetSuggest()
    .UseTypoCorrections()
    .UseParseErrorReporting()
    .CancelOnProcessTermination()
    .UseTypoCorrections()
    .UseHelp()
    .Build();

var result = await parser.InvokeAsync(args);
Console.WriteLine();
return result;
