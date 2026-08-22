using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Domain.Classes;

public class VideoFile : AbstractMediaFile
{
  public required EVideoExtension Extension { get; set; }
  public required int DurationSec { get; set; }
  public required EVideoResolution Resolution { get; set; }
}
