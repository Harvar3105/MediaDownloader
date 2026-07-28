namespace MediaDownloader.Youtube;

using MediaDownloader.Domain.Classes;
using MediaDownloader.Domain.Enums;
using VideoLibrary;
using Vid = Domain.Classes.Video;

public class YoutubeDownloader
{
  private readonly YouTube _downloader = YouTube.Default;
  public async Task<Vid> GetVideo(string link, EVideoResolution resolution)
  {
    var streams = await _downloader.GetAllVideosAsync(link);
    var video = streams.FirstOrDefault(v => v.Resolution == (int) resolution);
    if (video == null)
    {
      throw new ArgumentException($"Video with resolution {resolution} not found.");
    }
    
    var metadata = new MediaMetadata
    {
      Title = video.Title,
      Author = video.Info.Author,
      FullName = video.FullName,
      FileSize = video.ContentLength,
    };

    return new Vid
    {
      Metadata = metadata,
      DurationSec = video.Info.LengthSeconds,
      Extension = EnumHelpers.GetVideoExtension(video.FileExtension),
      Resolution = (EVideoResolution) video.Resolution,
      Content = await video.GetBytesAsync(),
    };
  }

  public async Task<Audio> GetAudio(string link)
  {
    var streams = await _downloader.GetAllVideosAsync(link);
    var audio = streams.First(s => s.AdaptiveKind == AdaptiveKind.Audio);
    if (audio == null)
    {
      throw new ArgumentException($"Audio not found.");
    }

    return new Audio
    {
      Metadata = new MediaMetadata
      {
        Title = audio.Title,
        Author = audio.Info.Author,
        FullName = audio.FullName,
        FileSize = audio.ContentLength,
      },
      Extension = EnumHelpers.GetAudioExtension(audio.FileExtension),
      DurationSec = audio.Info.LengthSeconds,
      Bitrate = audio.AudioBitrate,
      Content = audio.GetBytes(),
    };
  }
}
