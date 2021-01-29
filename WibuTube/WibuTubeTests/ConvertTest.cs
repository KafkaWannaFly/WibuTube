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
    public class ConvertTest
    {
        WibuTubeConverter wibuTubeConverter = new();

        private string videoPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland.mp4";
        private string audioPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland 0-0.mp3";
        private string thumbnailPath = "tmp/Most Heartfelt Music_ _Dream Dancer_ by Ben Crosland-0.jpg";

        private string url = "https://www.youtube.com/watch?v=_z411ShLWHI&list=PLFJRIUbt-ELjQZqVbzvCGerxXgjGp_Ww5&index=12";

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 10)]
        public async Task ExtractAudioTest(int _start, int _duration)
        {
            if(!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            var songPath = Path.GetFileNameWithoutExtension(videoPath);
            songPath = $"tmp/{songPath} {_start}-{_duration}.mp3";

            var mp3 = await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songPath, start: _start, duration: _duration);

            Assert.IsNotNull(mp3);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(10)]
        [DataRow(20000)]
        [DataRow(-10)]
        public async Task GetThumbnailTest(int atSecond)
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            var imgPath = Path.GetFileNameWithoutExtension(videoPath);
            imgPath = $"tmp/{imgPath}-{atSecond}.jpg";

            if(atSecond < 0 || atSecond > 2000)
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                    { 
                        await wibuTubeConverter.GetVideoSnapshotAsync(atSecond, videoPath, imgPath); 
                    });
            }
            else
            {
                var imgFileInfo = await wibuTubeConverter.GetVideoSnapshotAsync(atSecond, videoPath, imgPath);
                Assert.IsNotNull(imgFileInfo);
            }
        }

        [TestMethod]
        public async Task SetMp3ThumbnailTest()
        {
            if (!File.Exists(videoPath))
            {
                var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);
                videoPath = mp4.FullName;
            }

            if(!File.Exists(audioPath))
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

            Assert.IsNotNull(mp3);
        }

        [TestMethod]
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

            if(!File.Exists(thumbnailPath))
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

            Assert.IsNotNull(mp3Info);
        }
    }
}
