namespace SubLib
{
    using CookComputing.XmlRpc;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    public class OSIntermediary :IDisposable
    {
        private const string UserAgent = "SubLoad v1";
        private const int MaxAttempts = 10;
        private readonly ISubRPC proxy = XmlRpcProxyGen.Create<ISubRPC>();
        private LogInResponse logInInfo = null;

        public OSIntermediary()
        {

        }

        public bool IsLoggedIn
        {
            get
            {
                return this.logInInfo != null;
            }
        }

        public async Task OSLogIn()
        {
            await Task.Run(() =>
            {
                int numberOfTries = 0;
                while (!this.IsLoggedIn && numberOfTries <= MaxAttempts)
                {
                    try
                    {
                        this.logInInfo = this.proxy.LogIn(string.Empty, string.Empty, "en", UserAgent);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        numberOfTries++;
                    }
                }
            });
            if(this.IsLoggedIn == false)
            {
                throw new Exception("Can't connect to OpenSubtitles.");
            }
        }

        public async Task<SearchSubtitlesResponse> SearchOS(string path, string languages)
        {
            SearchSubtitlesResponse response = null;
            await Task.Run(() =>
            {
                int numberOfTries = 0;
                while (response == null && numberOfTries <= 10)
                {
                    try
                    {
                        response = this.proxy.SearchSubtitles(this.logInInfo.token, new MovieInfo[] { new MovieInfo(path, languages) });
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        numberOfTries++;
                    }
                }
            });
            return response;
        }

        public void OSLogOut()
        {
            this.proxy.LogOut(this.logInInfo.token);
            this.logInInfo = null;
        }

        public async Task<byte[]> DownloadSubtitle(int subtitle_id)
        {
            byte[] subtitleStream = null;
            await Task.Run(() =>
            {
                int numberOfTries = 0;
                while (subtitleStream == null && numberOfTries <= 10)
                {
                    try
                    {
                        DownloadSubtitleResponse response = this.proxy.DownloadSubtitles(this.logInInfo.token, new int[] { subtitle_id });
                        subtitleStream = Decompress(Convert.FromBase64String(response.data[0].data));
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        numberOfTries++;
                    }
                }
            });
            return subtitleStream;
        }

        private static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int Size = 4096;
                byte[] buffer = new byte[Size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, Size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public void Dispose()
        {
            this.OSLogOut();
        }
    }
}