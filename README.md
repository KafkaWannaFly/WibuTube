# WibuTube Package

This a a nuget package from [WibuTubeConverter]([tarzanchemgio/WibuTubeConverter: An UWP app that converts Youtube video into Mp3 (github.com)](https://github.com/tarzanchemgio/WibuTubeConverter)) which provides functionalities that do exactly what `WibuTubeConvert` is able to. But this package is .NET5 instead of UWP like `WibuTubeConverter`.

## Examples

### Download YouTube video

```c#
WibuTubeConverter wibuTubeConverter = new();

// Most Heartfelt Music: "Dream Dancer" by Ben Crosland
string url = "https://www.youtube.com/watch?v=_z411ShLWHI&list=PLFJRIUbt-ELjQZqVbzvCGerxXgjGp_Ww5&index=12";

// Video is saved into "tmp/" folder in working directory by default
var mp4 = await wibuTubeConverter.DownloadVideoAsync(url);

// You can specify your prefered location
var mp4 = await wibuTubeConverter.DownloadVideoAsync(url, "TheGloryHole/");
```

### Convert `mp4` to `mp3`

```c#
var songPath = Path.GetFileNameWithoutExtension(mp4.FullName);
songPath = $"tmp/{songName}.mp3";

var mp3 = await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songPath);

// You can also choose to convert a part of video
int start = 0;
int duration = 60; // Seconds
var mp3 = await wibuTubeConverter.ConvertVideoToMp3Async(videoPath, songPath, start, duration);
```

### Get thumbnail from video

```c#
int atSecond = 25;
string imgPath = "tmp/my-song-cover.jpg";
var imgFileInfo = await wibuTubeConverter.GetVideoSnapshotAsync(atSecond, mp4.FullName, imgPath);
```

### Set thumbnail image for `mp3`

```c#
var mp3 = await wibuTubeConverter.SetMp3Thumbnail(songPath, imgPath);
```

### Set a few more details for `mp3`

```c#
var mp3Info = wibuTubeConverter.SetMp3DetailInfo(songPath, new Song
            {
                Tittle = "Dream Dancer",
                Album = "HDSoundi",
                Performers = new[] { "Ben Crosland" }
            });
```

