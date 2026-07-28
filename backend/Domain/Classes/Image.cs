using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Domain.Classes;

public class Image : AbstractMediaFile
{
  public required EImageExtension Extension { get; set; }
  public required int Width { get; set; }
  public required int Height { get; set; }
}