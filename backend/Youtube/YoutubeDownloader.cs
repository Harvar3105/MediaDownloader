namespace MediaDownloader.Youtube;

using System.Globalization;
using MediaDownloader.Application;
using MediaDownloader.Domain.Classes;
using MediaDownloader.Domain.Enums;
using Microsoft.Extensions.Logging;

public class YoutubeDownloader
{
  private readonly YtdlpController _downloader;
  private readonly string[] NecessaryArguments = new[] { "-q", "-o", "-", "--js-runtime", "node" };
  private readonly ILogger<YoutubeDownloader> _logger;

  public YoutubeDownloader(ILogger<YoutubeDownloader> logger, YtdlpController downloader)
  {
    _logger = logger;
    _downloader = downloader;
  }

  public async Task<Video> GetVideoAsync(string link, EVideoResolution resolution)
  {
    string[] streamParams = ["-S", $"res:{(int) resolution}", link];
    var videoBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "--skip-download", "--print", "%(title)s|%(ext)s|%(uploader)s|%(duration)s|%(filesize,filesize_approx)s|%(height)s", }
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

    return new Video
    {
      Metadata = metadata,
      Extension = EnumHelpers.GetVideoExtension(metadataParts[1]),
      Content = videoBytes,
      DurationSec = int.Parse(metadataParts[3]),
      Resolution = (EVideoResolution) int.Parse(metadataParts[5]),
    };
  }

  public async Task<Audio> GetAudioAsync(string link, EAudioExtension format)
  {
    string[] streamParams = ["-f", format.ToString(), link];
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
      Extension = EnumHelpers.GetAudioExtension(metadataParts[1]),
      Content = audioBytes,
      DurationSec = int.Parse(metadataParts[3]),
    };
  }
}
