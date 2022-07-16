using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class DownloadVideoTest
    {
        readonly WibuTube wibuTubeConverter = new();
        [Fact]
        public async Task DownloadFromYouTubeTest()
        {
            // Most Heartfelt Music: "Dream Dancer" by Ben Crosland
            string url = "https://www.youtube.com/watch?v=_z411ShLWHI&list=PLFJRIUbt-ELjQZqVbzvCGerxXgjGp_Ww5&index=12";
            var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);

            Assert.True(File.Exists(mp4.FullName));
        }

        [Fact]
        public void DownloadUnExistedUrlTest()
        {
            string url = "tmp/Most Heartfelt Music: \"Dream Dancer\" by Ben Crosland.mp4";

            Exception exception = Assert.Throws<UriFormatException>(() => wibuTubeConverter.DownloadVideoAsync(url).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task NotYouTubeUrl()
        {
            string url = "https://docs.microsoft.com/";
            //var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);

            //Assert.ThrowsException<ArgumentException>(async () => 
            //    await wibuTubeConverter.DownloadVideoAsync(url));

            await Assert.ThrowsAsync<ArgumentException>(async () => await wibuTubeConverter.DownloadVideoAsync(url));
        }
    }
}
