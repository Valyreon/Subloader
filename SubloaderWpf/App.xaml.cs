using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using OpenSubtitlesSharp;
using SubloaderWpf.Utilities;

namespace SubloaderWpf
{
    public partial class App : Application
    {
        private static Mutex mutex;
        public static InstanceMediator InstanceMediator { get; private set; }
        public static string APIKey { get; private set; } = "idMNeWNRIKVKlGiP8zjNyG80a4AqKYBd";

        public string PathArg { get; set; }

        public static Settings Settings { get; } = SettingsParser.Load();

        protected override async void OnStartup(StartupEventArgs e)
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

            if (Settings.AllLanguages == null || Settings.AllLanguages.Count == 0)
            {
                using var osClient = new OpenSubtitlesClient(APIKey);
                Settings.AllLanguages = await osClient.GetLanguagesAsync();

                if(Settings.WantedLanguages == null || Settings.WantedLanguages.Count == 0)
                {
                    Settings.WantedLanguages = new List<string>() { "en" };
                }
            }

            if(Settings.Formats == null || Settings.Formats.Count == 0)
            {
                using var osClient = new OpenSubtitlesClient(APIKey);
                Settings.Formats = await osClient.GetSubtitleFormatsAsync();
            }

            SettingsParser.Save(Settings);

            base.OnStartup(e);
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            InstanceMediator?.StopListening();
            mutex?.ReleaseMutex();
            mutex?.Dispose();
        }
    }
}
