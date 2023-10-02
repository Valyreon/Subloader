using System;
using System.Threading;
using System.Windows;
using SubloaderWpf.Services;
using SubloaderWpf.Utilities;
using SubloaderWpf.ViewModels;
using SubloaderWpf.Views;

namespace SubloaderWpf;

public partial class App : Application
{
    private static Mutex mutex;
    public static readonly string VersionTag = "v1.6.0";
    public static string APIKey { get; private set; } = "";
    public static InstanceMediator InstanceMediator { get; private set; }
    public string PathArg { get; set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        CheckMutex(e);

        var settings = await ApplicationDataReader.LoadSettingsAsync();
        var openSubtitlesService = new OpenSubtitlesService(settings);
        MainWindow = new TheWindow
        {
            DataContext = new TheWindowViewModel(settings, openSubtitlesService)
        };

        MainWindow.Show();
    }

    private void ApplicationExit(object sender, ExitEventArgs e)
    {
        Cleanup();
    }

    private void CheckMutex(StartupEventArgs e)
    {
        if (e.Args.Length > 0)
        {
            PathArg = e.Args[0];
        }

        try
        {
            mutex = new Mutex(true, "valyreon.subloader", out var isOnlyInstance);
            if (!isOnlyInstance)
            {
                if (!string.IsNullOrWhiteSpace(PathArg))
                {
                    InstanceMediator.SendArgumentToRunningInstance(PathArg);
                }

                Cleanup();
                Environment.Exit(0);
            }

            InstanceMediator = new InstanceMediator();
            InstanceMediator.StartListening();
        }
        catch
        {
            Cleanup();
        }
    }

    private static void Cleanup()
    {
        InstanceMediator?.StopListening();
        mutex?.ReleaseMutex();
        mutex?.Dispose();
    }
}
