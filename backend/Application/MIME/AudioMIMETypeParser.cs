using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Application.MIME;

public static class AudioMimeTypeParser
{
  public static string Parse(EExtension extension)
  {
    return extension switch
    {
      EExtension.Aac => "audio/aac",
      EExtension.Alac => "audio/alac",
      EExtension.Flac => "audio/flac",
      EExtension.M4a => "audio/mp4",
      EExtension.Mp3 => "audio/mpeg",
      EExtension.Opus => "audio/opus",
      EExtension.Vorbis => "audio/ogg",

      _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
    };
  }
}