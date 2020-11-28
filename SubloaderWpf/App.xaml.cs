using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                TextBox.MouseDoubleClickEvent,
                new RoutedEventHandler(SelectAllText));

            if (e.Args.Length > 0)
            {
                PathArg = e.Args[0];
            }

            mutex = new Mutex(true, "valyreon.subloader", out var isOnlyInstance);
            if (!isOnlyInstance && !string.IsNullOrWhiteSpace(PathArg))
            {
                InstanceMediator.SendArgumentToRunningInstance(PathArg);

                Cleanup();
                Environment.Exit(0);
            }

            InstanceMediator = new InstanceMediator();
            InstanceMediator.StartListening();

            base.OnStartup(e);
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focused, give it the focus and
                    // stop further processing of this click event.
                    _ = textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            ApplicationSettings.Instance.SaveIfDirty();
            Cleanup();
        }

        private void Cleanup()
        {
            InstanceMediator?.StopListening();
            mutex.ReleaseMutex();
            mutex.Dispose();
        }
    }
}
