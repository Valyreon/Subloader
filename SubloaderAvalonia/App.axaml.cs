using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SubloaderAvalonia.Services;
using SubloaderAvalonia.Utilities;
using SubloaderAvalonia.ViewModels;
using SubloaderAvalonia.Views;

namespace SubloaderAvalonia;

public partial class App : Application
{
    private static Mutex mutex;
    public static readonly string VersionTag = "v1.6.0";
    public static string APIKey { get; private set; } = "LPV6D17NkgeAkX8r2B39Im1WrkIeErAc";
    public static InstanceMediator InstanceMediator { get; private set; }
    public string PathArg { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            CheckMutex(desktop.Args);
            var settings = await ApplicationDataReader.LoadSettingsAsync();
            var openSubtitlesService = new OpenSubtitlesService(settings);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(settings, openSubtitlesService)
            };
            desktop.MainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnApplicationExiting(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Cleanup();
    }

    private void CheckMutex(string[] args)
    {
        if (args != null && args.Length > 0)
        {
            PathArg = args[0];
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
