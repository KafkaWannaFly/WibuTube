using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ConvertTest
    {
        readonly WibuTube wibuTubeConverter = new();

        private string videoPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland.mp4";
        private string audioPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland 0-0.mp3";
        private string thumbnailPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland-0.jpg";

        private string url = "https://www.youtube.com/watch?v=_z411ShLWHI&list=PLFJRIUbt-ELjQZqVbzvCGerxXgjGp_Ww5&index=12";

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 10)]
        public async Task ExtractAudioTest(int _start, int _duration)
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            var songPath = Path.GetFileNameWithoutExtension(videoPath);
            songPath = $"tmp/{songPath} {_start}-{_duration}.mp3";

            var mp3 = await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songPath, start: _start, duration: _duration);

            Assert.NotNull(mp3);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(20000)]
        [InlineData(-10)]
        public async Task GetThumbnailTest(int atSecond)
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            var imgPath = Path.GetFileNameWithoutExtension(videoPath);
            imgPath = $"tmp/{imgPath}-{atSecond}.jpg";

            if (atSecond < 0 || atSecond > 2000)
            {
                await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await wibuTubeConverter.GetVideoSnapshotAsync(atSecond, videoPath, imgPath);
                });
            }
            else
            {
                var imgFileInfo = await wibuTubeConverter.GetVideoSnapshotAsync(atSecond, videoPath, imgPath);
                Assert.NotNull(imgFileInfo);
            }
        }

        [Fact]
        public async Task SetMp3ThumbnailTest()
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            if (!File.Exists(audioPath))
            {
                var songName = Path.GetFileNameWithoutExtension(videoPath);
                songName = $"tmp/{songName} 0-0.mp3";
                await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songName);

                audioPath = songName;
            }

            var imgPath = Path.GetFileNameWithoutExtension(videoPath);
            imgPath = $"tmp/{imgPath}-{10}.jpg";
            var imgInfo = await wibuTubeConverter.GetVideoSnapshotAsync(10, videoPath, imgPath);

            var mp3 = await wibuTubeConverter.SetMp3Thumbnail(audioPath, imgPath);

            Assert.NotNull(mp3);
        }

        [Fact]
        public async Task SetSongDetailTest()
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            if (!File.Exists(audioPath))
            {
                var songName = Path.GetFileNameWithoutExtension(videoPath);
                songName = $"tmp/{songName} 0-0.mp3";
                await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songName);

                audioPath = songName;
            }

            if (!File.Exists(thumbnailPath))
            {
                var imgPath = Path.GetFileNameWithoutExtension(videoPath);
                imgPath = $"tmp/{imgPath}-{10}.jpg";
                var imgInfo = await wibuTubeConverter.GetVideoSnapshotAsync(10, videoPath, imgPath);
            }

            var mp3Info = wibuTubeConverter.SetMp3DetailInfo(audioPath, new Song
            {
                Tittle = "Dream Dancer",
                Album = "HDSoundi",
                Performers = new[] { "Ben Crosland" }
            });

            Assert.NotNull(mp3Info);
        }
    }
}
