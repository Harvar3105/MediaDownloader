namespace MediaDownloader.Domain.Enums;

public static class EnumHelpers
{
  public static EImageExtension GetImageExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      ".png" => EImageExtension.Png,
      ".jpg" => EImageExtension.Jpg,
      ".jpeg" => EImageExtension.Jpeg,
      ".webp" => EImageExtension.WebP,
      ".gif" => EImageExtension.Gif,
      ".svg" => EImageExtension.Svg,
      _ => throw new ArgumentException($"Unknown image extension: {extension}")
    };
  }

  public static string GetImageExtensionString(EImageExtension extension)
  {
    return extension switch
    {
      EImageExtension.Png => ".png",
      EImageExtension.Jpg => ".jpg",
      EImageExtension.Jpeg => ".jpeg",
      EImageExtension.WebP => ".webp",
      EImageExtension.Gif => ".gif",
      EImageExtension.Svg => ".svg",
      _ => throw new ArgumentException($"Unknown image extension: {extension}")
    };
  }

  public static EVideoExtension GetVideoExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      ".mp4" => EVideoExtension.Mp4,
      ".webm" => EVideoExtension.WebM,
      _ => throw new ArgumentException($"Unknown video extension: {extension}")
    };
  }

  public static string GetVideoExtensionString(EVideoExtension extension)
  {
    return extension switch
    {
      EVideoExtension.Mp4 => ".mp4",
      EVideoExtension.WebM => ".webm",
      _ => throw new ArgumentException($"Unknown video extension: {extension}")
    };
  }

  public static EAudioExtension GetAudioExtension(string extension)
  {
    return extension.Trim().ToLower() switch
    {
      ".mp3" => EAudioExtension.Mp3,
      ".mp4" => EAudioExtension.Mp4,
      ".wav" => EAudioExtension.Wav,
      _ => throw new ArgumentException($"Unknown audio extension: {extension}")
    };
  }

  public static string GetAudioExtensionString(EAudioExtension extension)
  {
    return extension switch
    {
      EAudioExtension.Mp3 => ".mp3",
      EAudioExtension.Mp4 => ".mp4",
      EAudioExtension.Wav => ".wav",
      _ => throw new ArgumentException($"Unknown audio extension: {extension}")
    };
  }

  // public static EVideoResolution GetVideoResolution(int resolution)
  // {
  //   return resolution switch
  //   {
  //     144 => EVideoResolution.P144,
  //     240 => EVideoResolution.P240,
  //     360 => EVideoResolution.P360,
  //     480 => EVideoResolution.P480,
  //     720 => EVideoResolution.P720,
  //     1080 => EVideoResolution.P1080,
  //     1440 => EVideoResolution.P1440,
  //     2160 => EVideoResolution.P2160,
  //     _ => throw new ArgumentException($"Unknown video resolution: {resolution}")
  //   };
  // }

  // public static int GetVideoResolutionInt(EVideoResolution resolution)
  // {
  //   return resolution switch
  //   {
  //     EVideoResolution.P144 => 144,
  //     EVideoResolution.P240 => 240,
  //     EVideoResolution.P360 => 360,
  //     EVideoResolution.P480 => 480,
  //     EVideoResolution.P720 => 720,
  //     EVideoResolution.P1080 => 1080,
  //     EVideoResolution.P1440 => 1440,
  //     EVideoResolution.P2160 => 2160,
  //     _ => throw new ArgumentException($"Unknown video resolution: {resolution}")
  //   };
  // }
}