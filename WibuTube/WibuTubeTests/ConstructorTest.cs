using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using WibuTube;

namespace WibuTubeTests
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void DownloadFFmpegTest()
        {
            WibuTubeConverter wibuTubeConverter = new();

            bool isDir = Directory.Exists(WibuTubeConverter.ffmpegBinaryFolder);
            Assert.AreEqual(true, isDir);
        }

        [TestMethod]
        public void ChangeFFmpegDirTest()
        {
            WibuTubeConverter.ffmpegBinaryFolder = "FFmpegBinary";
            WibuTubeConverter wibuTubeConverter = new();

            bool isDir = Directory.Exists("FFmpegBinary");
            Assert.AreEqual(true, isDir);
        }

        
    }

}