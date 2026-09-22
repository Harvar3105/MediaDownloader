using MediaDownloader.Domain.Enums;

namespace MediaDownloader.Application.MIME;

public static class VideoMimeTypeParser
{
    public static string Parse(EExtension extension)
    {
        return extension switch
        {
            EExtension.Mp4 => "video/mp4",
            EExtension.Webm => "video/webm",
            EExtension.Mkv => "video/x-matroska",
            EExtension.Avi => "video/x-msvideo",
            EExtension.Mov => "video/quicktime",
            EExtension.Flv => "video/x-flv",
            EExtension.Mpeg => "video/mpeg",

            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };
    }
}