using System;
using System.Threading;
using System.Windows;
using SubloaderWpf.Models;
using SubloaderWpf.Utilities;

namespace SubloaderWpf
{
    public partial class App : Application
    {
        private static Mutex mutex;
        public static InstanceMediator InstanceMediator { get; private set; }

        public string PathArg { get; set; }

        protected override void OnStartup(StartupEventArgs e)
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

            base.OnStartup(e);
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            ApplicationSettings.Instance.SaveIfDirty();
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
