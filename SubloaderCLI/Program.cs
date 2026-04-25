using System.CommandLine;
using OpenSubtitlesSharp;
using SubloaderCLI;

var rootCommand = new RootCommand(
    "Subloader CLI application. It uses OpenSubtitles REST API to download subs for your video files.");

var forceDefaultApiUrlOption = new Option<bool>("--force_default_api_url", "-fdu")
{
    Description = "If flag is true, app will use the default api url for searching even if user is VIP. This is for VIP users that have a specific issue."
};

var usernameOption = new Option<string>("--user", "--username", "-u")
{
    Description = "Username for login. If specified, password will need to be input as well. " +
            "Command will login and use your token for the entire operation after which it will log you out."
};

var passwordOption = new Option<string>("--password", "-pass")
{
    Description = "Password to use for login with the provided username. If not specified, you will be prompted on run."
};

foreach (var cmd in Helper.GetImplementedCommands())
{
    var builtCommand = cmd.BuildCommand();
    builtCommand.Add(forceDefaultApiUrlOption);
    if (cmd.SupportsLogin)
    {
        builtCommand.Add(usernameOption);
        builtCommand.Add(passwordOption);
    }
    rootCommand.Subcommands.Add(builtCommand);
}

rootCommand.Add(forceDefaultApiUrlOption);
rootCommand.Add(usernameOption);
rootCommand.Add(passwordOption);

var parseResult = rootCommand.Parse(args);
if (parseResult.Errors.Count == 0)
{
    GlobalOptions.ForceDefaultApiUrl = parseResult.GetValue(forceDefaultApiUrlOption);

    var username = parseResult.GetValue(usernameOption);
    var password = parseResult.GetValue(passwordOption);

    // if password is provided with arguments but not username
    if (!string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(username))
    {
        ConsoleHelper.WriteLine("You must provide both username and password for login.", ConsoleColor.Red);
        return 1;
    }

    // if username is provided
    if (!string.IsNullOrWhiteSpace(username))
    {
        // if password is not provided with arguments prompt for it
        if (string.IsNullOrWhiteSpace(password))
        {
            password = Helper.GetPassword();

            if(string.IsNullOrWhiteSpace(password))
            {
                ConsoleHelper.WriteLine("Password is invalid.", ConsoleColor.Red);
                return 1;
            }
        }

        try
        {
            GlobalOptions.Session = await Helper.Login(username, password);
        }
        catch (RequestFailedException rfe)
        {
            ConsoleHelper.WriteLine(rfe.Message, ConsoleColor.Red);
            return 1;
        }
    }
}

try
{
    return parseResult.Invoke();
}
catch (Exception e)
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
}

return 1;
