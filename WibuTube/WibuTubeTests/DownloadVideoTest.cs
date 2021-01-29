using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WibuTube;

namespace WibuTubeTests
{
    [TestClass]
    public class DownloadVideoTest
    {
        WibuTubeConverter wibuTubeConverter = new();
        [TestMethod]
        public async Task DownloadFromYouTubeTest()
        {
            // Most Heartfelt Music: "Dream Dancer" by Ben Crosland
            string url = "https://www.youtube.com/watch?v=_z411ShLWHI&list=PLFJRIUbt-ELjQZqVbzvCGerxXgjGp_Ww5&index=12";
            var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);

            Assert.AreEqual(File.Exists(mp4.FullName), true);
        }

        [TestMethod]
        public void DownloadUnExistedUrlTest()
        {
            string url = "tmp/Most Heartfelt Music: \"Dream Dancer\" by Ben Crosland.mp4";

            Exception exception = Assert.ThrowsException<UriFormatException>(() => wibuTubeConverter.DownloadVideoAsync(url).GetAwaiter().GetResult());
        }

        [TestMethod]
        public async Task NotYouTubeUrl()
        {
            string url = "https://docs.microsoft.com/";
            //var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);

            //Assert.ThrowsException<ArgumentException>(async () => 
            //    await wibuTubeConverter.DownloadVideoAsync(url));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => wibuTubeConverter.DownloadVideoAsync(url));
        }
    }
}
