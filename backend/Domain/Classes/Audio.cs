using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Domain.Classes;

public class Audio : AbstractMediaFile
{
  public required EAudioExtension Extension { get; set; }
  public required float Bitrate { get; set; }
  public required int DurationSec { get; set; }
}
