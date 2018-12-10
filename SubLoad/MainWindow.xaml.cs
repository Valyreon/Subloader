namespace SubLoad
{
    using Microsoft.Win32;
    using SubLib;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed partial class MainWindow : Window
    {
        private const int MaxAttempts = 10;

        private OSIntermediary messenger = new OSIntermediary();
        private string currentPath = (Application.Current as App).PathArg;
        private List<string> languages = new List<string>();
        private bool isConfigRead = false;
        private ObservableCollection<SubtitleEntry> collection = new ObservableCollection<SubtitleEntry>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.dataTable.Items.IsLiveSorting = true;
            this.dataTable.Items.SortDescriptions.Add(new SortDescription("Language", ListSortDirection.Ascending));
        }

        public ObservableCollection<SubtitleEntry> Collection
        {
            get
            {
                return this.collection;
            }
        }

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (this.currentPath != null)
            {
                await this.ProcessFileAsync(this.currentPath);
            }
        }

        private async void ChooseFileButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileChooseDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; "
            };
            fileChooseDialog.ShowDialog();
            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileChooseDialog.FileName);
                this.Collection.Clear();
                await this.ProcessFileAsync(this.currentPath = fileInfo.FullName);
            }
            catch (Exception)
            {
                this.statusText.Text = "Open a video file.";
            }
        }

        private async Task ProcessFileAsync(string path)
        {
            if (!this.isConfigRead)
            {
                this.ReadConfig();
            }

            this.Collection.Clear();
            this.statusText.Text = "Searching subtitles...";
            SearchSubtitlesResponse ssre = null;
            int numberOfTries = 0;
            if (!this.messenger.IsLoggedIn)
            {
                while (!this.messenger.IsLoggedIn && numberOfTries <= MaxAttempts)
                {
                    try
                    {
                        await Task.Run(() => this.messenger.OSLogIn());
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        numberOfTries++;
                    }
                }
            }

            numberOfTries = 0;
            while (ssre == null && numberOfTries <= MaxAttempts)
            {
                try
                {
                    await Task.Run(() => this.messenger.SearchOS(path, "all", ref ssre));
                }
                catch (Exception)
                {
                }
                finally
                {
                    numberOfTries++;
                }
            }

            if (ssre != null && (ssre.data == null || ssre.data.Length == 0))
            {
                this.statusText.Text = "No subtitles found.";
                return;
            }
            else if (ssre == null)
            {
                this.statusText.Text = "Server error. Try refreshing.";
                return;
            }
            else
            {
                foreach (SubInfo x in ssre.data)
                {
                    if (this.languages.Contains(x.LanguageName.ToLower()) || this.languages.Count == 0)
                    {
                        App.Current.Dispatcher.Invoke(
                        () => { this.Collection.Add(new SubtitleEntry(x.SubFileName, x.LanguageName, int.Parse(x.IDSubtitleFile), x.SubFormat)); });
                        await Task.Run(() => Thread.Sleep(20));
                    }
                }

                if (this.Collection.Count > 0)
                {
                    this.statusText.Text = "Select a subtitle and click Download.";
                }
                else
                {
                    this.statusText.Text = "No subtitles found.";
                }
            }
        }

        private void ReadConfig()
        {
            try
            {
                string line;
                StreamReader cfgfile = new StreamReader((Registry.CurrentUser.OpenSubKey("Software\\Subtitle Loader").GetValue(null) as string) + "\\lang.cfg");
                while ((line = cfgfile.ReadLine()) != null)
                {
                    this.languages.Add(line.ToLower());
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }

        private async void DownloadButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            SubtitleEntry selected = (SubtitleEntry)this.dataTable.SelectedItem;
            try
            {
                if (selected != null)
                {
                    this.statusText.Text = "Downloading...";
                    byte[] subtitleStream = null;
                    int numberOfTries = 0;
                    while (subtitleStream == null && numberOfTries <= MaxAttempts)
                    {
                        try
                        {
                            await Task.Run(() => this.messenger.DownloadSubtitle(selected.GetSubtitleFileID(), ref subtitleStream));
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            numberOfTries++;
                        }
                    }

                    if (subtitleStream != null)
                    {
                        File.WriteAllBytes(Path.ChangeExtension(this.currentPath, selected.GetFormat()), subtitleStream);
                        this.statusText.Text = "Subtitle downloaded.";
                    }
                    else
                    {
                        this.statusText.Text = "Error while downloading.";
                    }
                }
            }
            catch (Exception)
            {
                this.statusText.Text = "Error while downloading. Try again.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                this.Hide();
                this.messenger.OSLogOut();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        private async void RefreshButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (this.currentPath != null)
            {
                await this.ProcessFileAsync(this.currentPath);
            }
        }

        private void TableRowNewSelect(object sender, RoutedEventArgs e) => this.statusText.Text = "Select a subtitle and click Download.";
    }
}