using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using SubLib;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SubLoad
{
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<SubtitleEntry> Collection = new ObservableCollection<SubtitleEntry>();
        private OSIntermediary messenger = new OSIntermediary();
        private string currentPath = (System.Windows.Application.Current as App).PathArg;
        
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
            OpenFileDialog fDialog = new OpenFileDialog
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

        private async System.Threading.Tasks.Task ProcessFileAsync (string path)
        {
            Collection.Clear();
            statusText.Text = "Searching subtitles...";
            SearchSubtitlesResponse ssre = null;
            try
            {
                if(!messenger.IsLoggedIn)
                    await Task.Run(() => messenger.OSLogIn());
                await Task.Run(()=> messenger.SearchOS(path, "all", ref ssre));
            } 
            catch (CookComputing.XmlRpc.XmlRpcServerException)
            {
                statusText.Text = "Server error. Try to refresh.";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Unexpected error");
                statusText.Text = "Unexpected error.";
            }
            if (ssre != null && (ssre.data == null || ssre.data.Length==0))
            {
                statusText.Text = "No subtitles found.";
                return;
            }
            else if(ssre==null)
            {
                return;
            }
            else
            {
                foreach (SubInfo x in ssre.data)
                {
                    Collection.Add(new SubtitleEntry(x.SubFileName, x.LanguageName, Int32.Parse(x.IDSubtitleFile), x.SubFormat));
                }
                statusText.Text = "Select a subtitle and click Download.";
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
                    await Task.Run(()=>messenger.DownloadSubtitle(selected.GetSubtitleFileID(), ref subtitleStream));
                    if (subtitleStream != null)
                    {
                        ByteArrayToFile(Path.ChangeExtension(currentPath, selected.GetFormat()), subtitleStream);
                        statusText.Text = "Subtitle downloaded.";
                    } 
                    else
                    {
                        statusText.Text = "Error downloading.";
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Write(byteArray, 0, byteArray.Length);
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Unexpected error");
                return false;
            }
        }

        private void TableRowNewSelect(object sender, RoutedEventArgs e)
        {
            statusText.Text = "Select a subtitle and click Download.";
        }
    }
}
