namespace MediaDownloader.Youtube;

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
    var metadataParts = metadataPayload.StandardOutput.Split('|');

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
}
