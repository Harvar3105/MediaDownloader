namespace MediaDownloader.Application;

using System.Globalization;
using System.Text.RegularExpressions;
using MediaDownloader.Domain.Classes;
using MediaDownloader.Domain.Enums;
using MediaDownloader.Runners;
using Microsoft.Extensions.Logging;

public class VideoAndAudioDownloader
{
  private static readonly Regex FileDataRegex = new(
    @"^(?<id>\S+)\s+(?<extension>\S+)\s+(?<resolution>.+)$",
    RegexOptions.Compiled);
  private static readonly Regex TotalMetaRegex = new(
    @"^(?<bitrate>\d+(?:[.,]\d+)?[kKmMgG]?)\s+\S+",
    RegexOptions.Compiled);
  private static readonly Regex CodecMetaRegex = new(
    @"^(?<videoCodec>\S+(?:\s+only)?)(?:\s+(?<videoBitrate>\d+(?:[.,]\d+)?[kKmMgG]?))?\s+(?<audioCodec>\S+(?:\s+only)?)(?:\s+(?<audioBitrate>\d+(?:[.,]\d+)?[kKmMgG]?))?",
    RegexOptions.Compiled);
  private readonly YtdlpController _downloader;
  private readonly string[] NecessaryArguments = new[] { "-q", "-o", "-", "--js-runtime", "node" };
  private readonly ILogger<VideoAndAudioDownloader> _logger;

  public VideoAndAudioDownloader(ILogger<VideoAndAudioDownloader> logger, YtdlpController downloader)
  {
    _logger = logger;
    _downloader = downloader;
  }

  public async Task<StreamInfo[]> GetStreamsInfoAsync(string link)
  {
    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "-q", "--skip-download", "--list-formats", link });
    var rows = metadataPayload.StandardOutput.Trim().Split('\n').Skip(2);
    var result = new List<StreamInfo>();
    foreach (string row in rows.Where(row => !string.IsNullOrWhiteSpace(row)))
    {
      string[] sections = row.Split(['|', '│'], StringSplitOptions.TrimEntries);
      if (sections.Length != 3)
      {
        _logger.LogWarning("Skipping an unrecognized format row: {Row}", row);
        continue;
      }

      var fileData = FileDataRegex.Match(sections[0]);
      var totalMeta = TotalMetaRegex.Match(sections[1]);
      var codecMeta = CodecMetaRegex.Match(sections[2]);
      if (!fileData.Success || !codecMeta.Success ||
          !Enum.TryParse<EVideoExtension>(fileData.Groups["extension"].Value, true, out var extension))
      {
        _logger.LogWarning("Skipping an unrecognized format row: {Row}", row);
        continue;
      }

      result.Add(new StreamInfo
      {
        Id = fileData.Groups["id"].Value,
        VideoExtension = extension,
        Resolution = fileData.Groups["resolution"].Value,
        TotalBitrate = totalMeta.Success ? ParseBitrate(totalMeta.Groups["bitrate"].Value) : null,
        VideoCodec = codecMeta.Groups["videoCodec"].Value,
        VideoBitrate = ParseBitrate(codecMeta.Groups["videoBitrate"].Value),
        AudioCodec = codecMeta.Groups["audioCodec"].Value,
        AudioBitrate = ParseBitrate(codecMeta.Groups["audioBitrate"].Value),
      });
    }

    return result.ToArray();
  }

  private static long? ParseBitrate(string value)
  {
    var match = Regex.Match(value, @"^(?<value>\d+(?:[.,]\d+)?)(?<unit>[kKmMgG]?)$");
    if (!match.Success || !double.TryParse(match.Groups["value"].Value.Replace(',', '.'), NumberStyles.Float,
          CultureInfo.InvariantCulture, out var bitrate))
    {
      return null;
    }

    return match.Groups["unit"].Value.ToLowerInvariant() switch
    {
      "m" => (long) Math.Round(bitrate * 1000),
      "g" => (long) Math.Round(bitrate * 1_000_000),
      _ => (long) Math.Round(bitrate),
    };
  }

  public async Task<MediaMetadata> GetMediaMetadataAsync(string link, string format)
  {
    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "--skip-download", "--print", "%(title)s|%(uploader)s|%(duration)s|%(filesize,filesize_approx)s", link });
    var metadataParts = metadataPayload.StandardOutput.Trim().Split('|');

    return new MediaMetadata
    {
      Title = metadataParts[0],
      Author = metadataParts[1],
      FullName = $"{metadataParts[0]}.{format.ToString().ToLower()}",
      FileSize = long.Parse(metadataParts[3]),
      DurationSec = int.Parse(metadataParts[2])
    };
  }

  public async Task<VideoFile> GetVideoAsync(string link, EVideoResolution resolution, EVideoExtension format)
  {
    string[] streamParams = ["-S", $"res:{(int) resolution}", link, "--remux-video", format.ToString().ToLower()];
    var videoBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadata = await GetMediaMetadataAsync(link, format.ToString());

    return new VideoFile
    {
      Metadata = metadata,
      Extension = format,
      Content = videoBytes,
      Resolution = resolution,
    };
  }

  public async Task<VideoFile> GetVideoByIdAsync(string link, string id, EVideoResolution resolution, EVideoExtension format)
  {
    string[] streamParams = ["-f", id, link, "--remux-video", format.ToString().ToLower()];

    var videoBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadata = await GetMediaMetadataAsync(link, format.ToString());

    return new VideoFile
    {
      Metadata = metadata,
      Extension = format,
      Content = videoBytes,
      Resolution = resolution,
    };
  }

  public async Task<AudioFile> GetAudioAsync(string link, EAudioExtension format)
  {
    string[] streamParams = ["-f", format.ToString().ToLower(), link];
    var audioBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadata = await GetMediaMetadataAsync(link, format.ToString());

    return new AudioFile
    {
      Metadata = metadata,
      Extension = format,
      Content = audioBytes,
    };
  }

  public async Task<AudioFile> GetAudioByIdAsync(string link, string id, EAudioExtension format)
  {
    string[] streamParams = ["-f", id, link];
    var audioBytes = await _downloader.RunBytesAsync(arguments: NecessaryArguments.Concat(streamParams).ToArray());

    var metadata = await GetMediaMetadataAsync(link, format.ToString());

    return new AudioFile
    {
      Metadata = metadata,
      Extension = format,
      Content = audioBytes,
    };
  }
}
