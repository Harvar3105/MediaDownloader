using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Domain.Classes;

public class AudioFile : AbstractMediaFile
{
  public required EAudioExtension Extension { get; set; }
}
