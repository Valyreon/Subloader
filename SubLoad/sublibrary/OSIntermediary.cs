namespace SubLib
{
    using CookComputing.XmlRpc;
    using System;
    using System.IO;
    using System.IO.Compression;

    public class OSIntermediary
    {
        private const string UserAgent = "SubLoad v1";
        private readonly ISubRPC proxy = XmlRpcProxyGen.Create<ISubRPC>();
        private LogInResponse logInInfo = null;

        public bool IsLoggedIn
        {
            get
            {
                return this.logInInfo != null;
            }
        }

        public void OSLogIn()
        {
            if (this.logInInfo != null)
            {
                this.OSLogOut();
            }

            this.logInInfo = this.proxy.LogIn(string.Empty, string.Empty, "en", UserAgent);
        }

        public void SearchOS(string path, string languages, ref SearchSubtitlesResponse res)
        {
            res = this.proxy.SearchSubtitles(this.logInInfo.token, new MovieInfo[] { new MovieInfo(path, languages) });
        }

        public void OSLogOut()
        {
            this.proxy.LogOut(this.logInInfo.token);
            this.logInInfo = null;
        }

        public void DownloadSubtitle(int subtitle_id, ref byte[] x)
        {
            DownloadSubtitleResponse response = this.proxy.DownloadSubtitles(this.logInInfo.token, new int[] { subtitle_id });
            x = Decompress(Convert.FromBase64String(response.data[0].data));
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
    }
}