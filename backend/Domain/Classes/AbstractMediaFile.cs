namespace MediaDownloader.Domain.Classes;

public abstract class AbstractMediaFile
{
  public required MediaMetadata Metadata { get; set; }
  public required byte[] Content { get; set; }
}