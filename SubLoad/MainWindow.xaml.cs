using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using SubLib;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SubLoad
{
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<SubtitleEntry> Collection = new ObservableCollection<SubtitleEntry>();
        private OSIntermediary messenger = new OSIntermediary();
        private string currentPath = (System.Windows.Application.Current as App).PathArg;
        private static readonly int maxAttempts = 10;
        private List<string> languages = new List<string>();
        private bool isConfigRead = false;

        public MainWindow()
        {
            InitializeComponent();
            dataTable.ItemsSource = Collection;
            dataTable.Items.IsLiveSorting = true;
            dataTable.Items.SortDescriptions.Add(new SortDescription("Language", ListSortDirection.Ascending));
        }

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (currentPath != null)
                await ProcessFileAsync(currentPath);
        }

        private async void ChooseFileButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; "
            };
            fDialog.ShowDialog();
            try
            {
                System.IO.FileInfo fInfo = new System.IO.FileInfo(fDialog.FileName);
                Collection.Clear();
                await ProcessFileAsync(currentPath = fInfo.FullName);
            }
            catch (Exception)
            {
                statusText.Text = "Open a video file.";
            }
        }

        private async Task ProcessFileAsync (string path)
        {
            if (!isConfigRead)
                ReadConfig();
            Collection.Clear();
            statusText.Text = "Searching subtitles...";
            SearchSubtitlesResponse ssre = null;
            int nTries = 0;
            if (!messenger.IsLoggedIn)
            {
                while (!messenger.IsLoggedIn && nTries <= maxAttempts)
                {
                    try
                    {
                        await Task.Run(() => messenger.OSLogIn());
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        nTries++;
                    }
                }
            }
            nTries = 0;
            while (ssre==null && nTries <= maxAttempts)
            {
                try
                {
                    await Task.Run(() => messenger.SearchOS(path, "all", ref ssre));
                }
                catch (Exception)
                {
                }
                finally
                {
                    nTries++;
                }
            }
            
            if (ssre != null && (ssre.data == null || ssre.data.Length==0))
            {
                statusText.Text = "No subtitles found.";
                return;
            }
            else if(ssre==null)
            {
                statusText.Text = "Server error. Try refreshing.";
                return;
            }
            else
            {
                foreach (SubInfo x in ssre.data)
                {
                    if (languages.Contains(x.LanguageName.ToLower()) || languages.Count == 0)
                    {
                        App.Current.Dispatcher.Invoke(
                        () => { Collection.Add(new SubtitleEntry(x.SubFileName, x.LanguageName, Int32.Parse(x.IDSubtitleFile), x.SubFormat)); }
                        );
                        await Task.Run(() => Thread.Sleep(20));
                    }
                }
                if(Collection.Count>0)
                    statusText.Text = "Select a subtitle and click Download.";
                else
                    statusText.Text = "No subtitles found.";
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
                    languages.Add(line.ToLower());
                }
            }
            catch (Exception)
            {

            }
        }

        private async void DownloadButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            SubtitleEntry selected = (SubtitleEntry)dataTable.SelectedItem;
            try
            {
                if (selected != null)
                {
                    statusText.Text = "Downloading...";
                    byte[] subtitleStream = null;
                    int nTries = 0;
                    while (subtitleStream==null && nTries <= maxAttempts)
                    {
                        try
                        {
                            await Task.Run(() => messenger.DownloadSubtitle(selected.GetSubtitleFileID(), ref subtitleStream));
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            nTries++;
                        }
                    }
                    
                    if (subtitleStream != null)
                    {
                        File.WriteAllBytes(Path.ChangeExtension(currentPath, selected.GetFormat()), subtitleStream);
                        statusText.Text = "Subtitle downloaded.";
                    } 
                    else
                    {
                        statusText.Text = "Error while downloading.";
                    }
                }
            }
            catch (Exception)
            {
                statusText.Text = "Error while downloading. Try again.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                this.Hide();
                messenger.OSLogOut();
            }
            catch (Exception)
            {
                
            }
        }

        private async void RefreshButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (currentPath != null)
                await ProcessFileAsync(currentPath);
        }

        private void TableRowNewSelect(object sender, RoutedEventArgs e)
        {
            statusText.Text = "Select a subtitle and click Download.";
        }
    }
}
