using MediaDownloader.Domain.Classes;

namespace MediaDownloader.Infrastructure.Files;

public sealed class TempFile : IDisposable
{
  public string Path { get; }

  public string FileName => System.IO.Path.GetFileName(Path);

  public long Length => new FileInfo(Path).Length;

  public readonly MediaMetadata _metadata;

  public TempFile(string path, MediaMetadata metadata)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(path);

    if (!File.Exists(path))
      throw new FileNotFoundException("Temporary file was not found.", path);

    Path = path;
    _metadata = metadata;
  }

  public FileStream OpenRead()
  {
    return new FileStream(
      Path,
      FileMode.Open,
      FileAccess.Read,
      FileShare.Read,
      bufferSize: 64 * 1024,
      useAsync: true);
  }

  public void Delete()
  {
    if (File.Exists(Path))
      File.Delete(Path);
  }

  public void Dispose()
  {
    Delete();
  }
}