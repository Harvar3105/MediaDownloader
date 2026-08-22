namespace MediaDownloader.Application;

using System.Globalization;
using MediaDownloader.Domain.Classes;
using MediaDownloader.Domain.Enums;
using MediaDownloader.Runners;
using Microsoft.Extensions.Logging;

public class VideoAndAudioDownloader
{
  private readonly YtdlpController _downloader;
  private readonly string[] NecessaryArguments = new[] { "-q", "-o", "-", "--js-runtime", "node" };
  private readonly ILogger<VideoAndAudioDownloader> _logger;

  public VideoAndAudioDownloader(ILogger<VideoAndAudioDownloader> logger, YtdlpController downloader)
  {
    _logger = logger;
    _downloader = downloader;
  }

  public async Task<Video> GetVideoAsync(string link, EVideoResolution resolution, EVideoExtension format)
  {
    string[] streamParams = ["-S", $"res:{(int) resolution}", link, "--remux-video", format.ToString().ToLower()];
    var videoBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());
    _logger.LogInformation($"Successfully downloaded video with resolution {resolution} and format {format}");
    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "--skip-download", "--print", "%(title)s|%(uploader)s|%(duration)s|%(filesize,filesize_approx)s|%(height)s", }
      .Concat(streamParams).ToArray());
    var metadataParts = metadataPayload.StandardOutput.Trim().Split('|');

    var metadata = new MediaMetadata
    {
      Title = metadataParts[0],
      FullName = $"{metadataParts[0]}.{format.ToString().ToLower()}",
      Author = metadataParts[1],
      FileSize = long.Parse(metadataParts[3]),
    };

    _logger.LogInformation($"Successfully got info for: {metadata.Title}");

    return new Video
    {
      Metadata = metadata,
      Extension = format,
      Content = videoBytes,
      DurationSec = int.Parse(metadataParts[2]),
      Resolution = (EVideoResolution) int.Parse(metadataParts[4]),
    };
  }

  public async Task<Audio> GetAudioAsync(string link, EAudioExtension format)
  {
    string[] streamParams = ["-f", format.ToString().ToLower(), link];
    var audioBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "--skip-download", "--print", "%(title)s|%(ext)s|%(uploader)s|%(duration)s|%(filesize,filesize_approx)s|%(tbr)s", }
      .Concat(streamParams).ToArray());
    var metadataParts = metadataPayload.StandardOutput.Trim().Split('|');

    var metadata = new MediaMetadata
    {
      Title = metadataParts[0],
      FullName = $"{metadataParts[0]}.{metadataParts[1]}",
      Author = metadataParts[2],
      FileSize = long.Parse(metadataParts[4]),
    };

    _logger.LogInformation($"Successfully got info for: {metadata.Title}");
    _logger.LogInformation($"Parts: {String.Join(" | ", metadataParts) + "END"}");

    return new Audio
    {
      Metadata = metadata,
      Bitrate = float.Parse(metadataParts[5], CultureInfo.InvariantCulture),
      Extension = Enum.Parse<EAudioExtension>(metadataParts[1].Trim(), ignoreCase: true),
      Content = audioBytes,
      DurationSec = int.Parse(metadataParts[3]),
    };
  }
}
