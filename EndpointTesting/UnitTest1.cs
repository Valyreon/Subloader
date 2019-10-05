using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleSuppliers.OpenSubtitles;

namespace EndpointTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            OpenSubtitles supplier = new OpenSubtitles();
            var x = await supplier.SearchAsync(@"F:\Torrents\Aquaman.2018.1080p.BluRay.x264-SPARKS[rarbg]\Aquaman.2018.1080p.BluRay.x264-SPARKS.mkv", "");
        }
    }
}
