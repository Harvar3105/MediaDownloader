namespace MediaDownloader.Application;

using System.Globalization;
using System.Text.RegularExpressions;
using MediaDownloader.Domain.Classes;
using MediaDownloader.Domain.Enums;
using MediaDownloader.Infrastructure.Files;
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
          !Enum.TryParse<EExtension>(fileData.Groups["extension"].Value, true, out var extension))
      {
        _logger.LogWarning("Skipping an unrecognized format row: {Row}", row);
        continue;
      }

      var resolution = fileData.Groups["resolution"].Value;
      if (resolution.Equals("unknown")) continue;

      result.Add(new StreamInfo
      {
        Id = fileData.Groups["id"].Value,
        Extension = extension,
        Resolution = resolution,
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
      "m" => (long)Math.Round(bitrate * 1000),
      "g" => (long)Math.Round(bitrate * 1_000_000),
      _ => (long)Math.Round(bitrate),
    };
  }

  public async Task<MediaMetadata> GetMediaMetadataAsync(string link, EExtension format, EResolution? resolution = null)
  {
    var metadataPayload = await _downloader.RunAsync(arguments: new[] {
      "--skip-download", "--print", "%(title)s|%(uploader)s|%(duration)s|%(filesize,filesize_approx)s", link });
    var metadataParts = metadataPayload.StandardOutput.Trim().Split('|');

    _logger.LogInformation($"Incoming metadata: {string.Join(", ", metadataParts)}");

    long fileSize;
    int durationSec;

    return new MediaMetadata
    {
      Title = metadataParts[0],
      Author = metadataParts[1],
      FullName = $"{metadataParts[0]}.{format.ToString().ToLower()}",
      FileSize =  long.TryParse(metadataParts[3], out fileSize) ? fileSize : null,
      DurationSec = int.TryParse(metadataParts[2], out durationSec) ? durationSec : null,
      Resolution = resolution,
      Extension = format
    };
  }

  public async Task<TempFile> GetVideoByIdAsync(string link, string id, EResolution resolution, EExtension format)
  {
    string[] streamParams = ["-f", $"{id}+bestaudio", link, "--remux-video", format.ToString().ToLower()];
    var metadata = await GetMediaMetadataAsync(link: link, format: format, resolution: resolution);
    return await GenerateFile(streamParams, metadata);
  }

  public async Task<TempFile> GetAudioByIdAsync(string link, string id, EExtension format)
  {
    string[] streamParams = ["-f", id, link];
    var metadata = await GetMediaMetadataAsync(link, format);
    return await GenerateFile(streamParams, metadata);
  }

  private async Task<TempFile> GenerateFile(string[] streamParams, MediaMetadata metadata)
  {
    var path = Path.GetTempPath();
    var fullPath = $"{path}/{metadata.FullName}";
    var args = ArgumentFactory.GetArguments(fullPath);
    await _downloader.RunBytesAsync(arguments: args.Concat(streamParams).ToArray());
    return new TempFile(fullPath, metadata);
  }
}
