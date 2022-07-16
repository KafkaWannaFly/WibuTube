using Microsoft.Extensions.Logging;

namespace UnitTest
{
    public class DownloadBinaryTest
    {
        [Fact]
        public async Task DefaultDownloadTest()
        {
            WibuTube wibuTubeConverter = new();
            await wibuTubeConverter.DownloadFfmpegBinaryIfNotExist();

            bool isDir = Directory.Exists(WibuTube.FfmpegBinaryFolder);
            Assert.True(isDir);
        }

        [Fact]
        public async Task ChangeFFmpegDirTest()
        {
            WibuTube.FfmpegBinaryFolder = "FFmpegBinary";
            WibuTube wibuTubeConverter = new();
            await wibuTubeConverter.DownloadFfmpegBinaryIfNotExist();

            bool isDir = Directory.Exists("FFmpegBinary");
            Assert.True(isDir);
        }
    }
}