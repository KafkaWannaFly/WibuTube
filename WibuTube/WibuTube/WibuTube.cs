using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using VideoLibrary;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

/// <summary>
/// This class require <see cref="https://ffmpeg.org/">FFmpeg</see> to work with. </br>
/// It would automatically download to <c>ffmpegBinaryFolder</c> folder if not found </br>
/// Check <c>isReady</c> before hand for sure
/// </summary>
public class WibuTube
{
    private readonly VideoClient clientServices;
    private readonly Client<YouTubeVideo> clientRequest;

    /// <summary>
    /// Default path to save FFpmeg binary into
    /// </summary>
    static string ffmpegBinaryFolder = "FFmpeg";

    public static string FfmpegBinaryFolder { get => ffmpegBinaryFolder; set => ffmpegBinaryFolder = value; }

    public WibuTube()
    {
        clientRequest = Client.For(YouTube.Default);
        clientServices = new VideoClient();
    }

    /// <summary>
    /// Some field are derived from IDisposable so they need to be cleaned
    /// </summary>
    ~WibuTube()
    {
        clientRequest.Dispose();
        clientServices.Dispose();
    }

    /// <summary>
    /// Download video from Youtube
    /// </summary>
    /// <param name="uri">URL of video on YouTube</param>
    /// <param name="folderToSaveIn">Video will be saved to '/temp' if not define</param>
    /// <returns>Object that contain infomation about downloaded file</returns>
    /// <exception cref="UriFormatException">Provided string is not an URL</exception>
    public async Task<FileInfo> DownloadVideoAsync(string uri, string folderToSaveIn = "tmp/")
    {
        try
        {
            Uri tmpUri;
            if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out tmpUri))
            {
                var msg = $"{nameof(DownloadVideoAsync)}: Bad URL!";
                throw new UriFormatException(msg);
            }

            var req = WebRequest.CreateHttp(uri);
            req.Method = "HEAD";

            var res = (HttpWebResponse)(await req.GetResponseAsync());
            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpListenerException((int)res.StatusCode, $"{nameof(DownloadVideoAsync)}: Bad URL!");
            }

            if (!Directory.Exists(folderToSaveIn))
            {
                Directory.CreateDirectory(folderToSaveIn);
            }

            YouTubeVideo youTubeVideo = await clientRequest.GetVideoAsync(uri);

            //Because Stream is implemented IDisposable, we must call Dispose directly or indirectly
            using (Stream sourceStream = await youTubeVideo.StreamAsync())
            using (Stream destinationStream = File.OpenWrite(folderToSaveIn + youTubeVideo.FullName))
                await sourceStream.CopyToAsync(destinationStream);

            return new FileInfo(folderToSaveIn + youTubeVideo.FullName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Convert video from <c>source</c> to <c>dest</c>
    /// </summary>
    /// <param name="source">Path to video</param>
    /// <param name="dest">Path to output audio</param>
    /// <returns>Info about output audio</returns>
    /// <exception cref="FileNotFoundException">Can't find <c>source</c></exception>
    /// <exception cref="ArgumentException">End time is less then start time</exception>
    public async Task<FileInfo> ConvertVideoToMp3Async(string source, string dest,
        int start = 0, int duration = 0)
    {
        await DownloadFfmpegBinaryIfNotExist();

        var funcName = nameof(ConvertVideoToMp3Async);
        if (!File.Exists(source))
        {
            throw new FileNotFoundException($"{funcName}: Can't find {source}");
        }

        if (duration < start || duration < 0 || start < 0)
        {
            throw new ArgumentException($"{funcName}: End time is less then start time");
        }

        if (File.Exists(dest))
        {
            return new FileInfo(dest);
        }

        if (start == 0 && duration == 0)
        {
            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(source, dest);
            await conversion.Start();
        }
        else
        {
            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(source, dest);
            conversion.SetSeek(TimeSpan.FromSeconds(start));
            conversion.SetOutputTime(TimeSpan.FromSeconds(duration));

            await conversion.Start();
        }

        return new FileInfo(dest);
    }

    /// <summary>
    /// Get a snapshot of video at a specific of time
    /// </summary>
    /// <param name="second">That specific of second</param>
    /// <param name="source">Path to video file</param>
    /// <param name="dest">Path to audio file</param>
    /// <returns>Info of result audio file</returns>
    /// <exception cref="ArgumentException">When <c>second</c> less than 0</exception>
    /// <exception cref="FileNotFoundException">Time value must not be negative</exception>
    public async Task<FileInfo> GetVideoSnapshotAsync(double second, string source, string dest)
    {
        await DownloadFfmpegBinaryIfNotExist();

        if (second < 0)
        {
            throw new ArgumentException($"{nameof(GetVideoSnapshotAsync)}: Time value must not be negative");
        }

        var sourceInfo = new FileInfo(source);
        if (!sourceInfo.Exists)
        {
            throw new FileNotFoundException($"{nameof(GetVideoSnapshotAsync)}: Can't find {source}");
        }

        if (File.Exists(dest))
        {
            return new FileInfo(dest);
        }

        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(sourceInfo.FullName,
                                                        dest,
                                                        TimeSpan.FromSeconds(second));
        await conversion.Start();

        return new FileInfo(dest);
    }

    /// <summary>
    /// Set cover image for the song
    /// </summary>
    /// <param name="mp3Path">Path to audio file</param>
    /// <param name="picturePath">Path to image file</param>
    /// <returns>Info of result audio</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<FileInfo> SetMp3Thumbnail(string mp3Path, string picturePath)
    {
        if (!File.Exists(mp3Path))
        {
            throw new FileNotFoundException($"{nameof(SetMp3Thumbnail)}: Can't find {mp3Path}");
        }

        if (!File.Exists(picturePath))
        {
            throw new FileNotFoundException($"{nameof(SetMp3Thumbnail)}: Can't find {picturePath}");
        }

        using var mp3 = TagLib.File.Create(mp3Path);
        var picture = new TagLib.Picture(picturePath)
        {
            Type = TagLib.PictureType.FrontCover,
            Description = "Cover",
            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
        };

        mp3.Tag.Pictures = new TagLib.Picture[] { picture };
        mp3.Save();

        Console.WriteLine($"Saved file {mp3.Name}");

        return await Task.FromResult(new FileInfo(mp3.Name));
    }

    /// <summary>
    /// Set few detail infomation of mp3 file. Cover picture is untouched
    /// </summary>
    /// <param name="mp3Path">Path to audio file</param>
    /// <param name="songDetail">Few info about the song</param>
    /// <param name="changeFileNameToTittle">Keep file name as </param>
    /// <returns>Info of result audio</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public FileInfo SetMp3DetailInfo(string mp3Path, ISong songDetail)
    {
        if (!File.Exists(mp3Path))
        {
            throw new FileNotFoundException($"{nameof(SetMp3Thumbnail)}: Can't find {mp3Path}");
        }

        using var mp3 = TagLib.File.Create(mp3Path);
        mp3.Tag.Title = songDetail.Tittle;
        mp3.Tag.Performers = songDetail.Performers;
        mp3.Tag.Album = songDetail.Album;

        mp3.Save();

        Console.WriteLine($"Saved file {mp3.Name}");
        return new FileInfo(mp3.Name);
    }

    public async Task DownloadFfmpegBinaryIfNotExist()
    {
        if (!Directory.Exists(ffmpegBinaryFolder))
        {
            Console.WriteLine($"Can't find {ffmpegBinaryFolder}. Creating one and downloading ffmpeg binary...");

            Directory.CreateDirectory(ffmpegBinaryFolder);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegBinaryFolder);

            Console.WriteLine("Download finished");
        }

        FFmpeg.SetExecutablesPath(ffmpegBinaryFolder);
    }
}