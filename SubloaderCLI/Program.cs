using System.CommandLine;
using OpenSubtitlesSharp;
using SubloaderCLI;

var rootCommand = new RootCommand(
    "Subloader CLI application. It uses OpenSubtitles REST API to download subs for your video files.");

foreach(var command in Helper.GetImplementedCommands().Select(c => c.BuildCommand()))
{
    rootCommand.AddCommand(command);
}

try
{
    var result = await rootCommand.InvokeAsync(args);
    Console.WriteLine();
    return result;
}
catch(RequestFailedException osException)
{
    ConsoleHelper.WriteExceptionMessage(osException.Message);
}

return 0;
