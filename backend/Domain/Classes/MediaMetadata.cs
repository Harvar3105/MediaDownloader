namespace MediaDownloader.Domain.Classes;

public class MediaMetadata
{
  public required string Title { get; set; }
  public required string FullName { get; set; }
  public string? Author { get; set; }
  public long? FileSize { get; set; }
}
