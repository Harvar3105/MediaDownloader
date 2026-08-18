namespace MediaDownloader.Domain.Enums;

public static class EnumHelpers
{
  public static EImageExtension GetImageExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      "png" => EImageExtension.Png,
      "jpg" => EImageExtension.Jpg,
      "jpeg" => EImageExtension.Jpeg,
      "webp" => EImageExtension.WebP,
      "gif" => EImageExtension.Gif,
      "svg" => EImageExtension.Svg,
      _ => throw new ArgumentException($"Unknown image extension: {extension}")
    };
  }

  public static string GetImageExtensionString(EImageExtension extension)
  {
    return extension switch
    {
      EImageExtension.Png => "png",
      EImageExtension.Jpg => "jpg",
      EImageExtension.Jpeg => "jpeg",
      EImageExtension.WebP => "webp",
      EImageExtension.Gif => "gif",
      EImageExtension.Svg => "svg",
      _ => throw new ArgumentException($"Unknown image extension: {extension}")
    };
  }

  public static EVideoExtension GetVideoExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      "mp4" => EVideoExtension.Mp4,
      "webm" => EVideoExtension.WebM,
      _ => throw new ArgumentException($"Unknown video extension: {extension}")
    };
  }

  public static string GetVideoExtensionString(EVideoExtension extension)
  {
    return extension switch
    {
      EVideoExtension.Mp4 => "mp4",
      EVideoExtension.WebM => "webm",
      _ => throw new ArgumentException($"Unknown video extension: {extension}")
    };
  }

  public static EAudioExtension GetAudioExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      "mp3" => EAudioExtension.mp3,
      "mp4" => EAudioExtension.mp4,
      "wav" => EAudioExtension.wav,
      _ => throw new ArgumentException($"Unknown audio extension: {extension}")
    };
  }

  public static string GetAudioExtensionString(EAudioExtension extension)
  {
    return extension switch
    {
      EAudioExtension.mp3 => "mp3",
      EAudioExtension.mp4 => "mp4",
      EAudioExtension.wav => "wav",
      _ => throw new ArgumentException($"Unknown audio extension: {extension}")
    };
  }
}