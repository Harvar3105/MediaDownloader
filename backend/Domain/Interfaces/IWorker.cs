using MediaDownloader.Domain.Classes;

namespace MediaDownloader.Domain.Interfaces;

public interface IWorker
{
  Task<AbstractMediaFile> GetMediaMetadataAsync(string url, CancellationToken cancellationToken = default);
  Task DownloadMediaAsync(string url, string outputPath, CancellationToken cancellationToken = default);
}