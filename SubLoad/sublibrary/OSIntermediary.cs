using System;
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;
using System.IO.Compression;
using System.IO;

namespace SubLib
{
    public class OSIntermediary
    {
        static string userAgent = "SubLoad v1";
        ISubRPC proxy = XmlRpcProxyGen.Create<ISubRPC>();
        private LogInResponse logInInfo = null;

        public bool IsLoggedIn
        {
            get
            {
                return logInInfo != null;
            }
        }

        public void OSLogIn()
        {
            if (logInInfo != null)
                OSLogOut();
            logInInfo = proxy.LogIn("", "", "en", userAgent);
        }

        public void SearchOS(string path, string languages, ref SearchSubtitlesResponse res)
        {
            res = proxy.SearchSubtitles(logInInfo.token, new MovieInfo[] { new MovieInfo(path,languages)});
        }

        public void OSLogOut()
        {
            proxy.LogOut(logInInfo.token);
            logInInfo = null;
        }

        public void DownloadSubtitle(int subtitle_id, ref byte[] x)
        {
            DownloadSubtitleResponse response = proxy.DownloadSubtitles(logInInfo.token, new int[] { subtitle_id });
            x = Decompress(Convert.FromBase64String(response.data[0].data));
        }

        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
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
