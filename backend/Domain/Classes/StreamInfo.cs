using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Domain.Classes;

public class StreamInfo
{
  public string Id { get; set; }  = null!;
  public EVideoExtension VideoExtension { get; set; }
  public string Resolution { get; set; } = null!;
  public string AudioCodec { get; set; } = null!;
  public string VideoCodec { get; set; } = null!;
  public long? TotalBitrate { get; set; }
  public long? VideoBitrate { get; set; }
  public long? AudioBitrate { get; set; }

  public override string ToString()
  {
    return $"{Id}\t{VideoExtension}\t{Resolution}\t|\t{TotalBitrate}\t|\t{VideoCodec}\t{VideoBitrate}\t|\t{AudioCodec}\t{AudioBitrate}";
  }
}