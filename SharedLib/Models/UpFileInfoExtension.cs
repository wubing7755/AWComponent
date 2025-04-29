
using SharedLibrary.Enums;

namespace SharedLibrary.Models;

public static class UpFileInfoExtension
{
    public static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 Bytes";
        const int scale = 1024;
        string[] units = { "Bytes", "KB", "MB", "GB", "TB" };
        int digitGroups = (int)(Math.Log(bytes) / Math.Log(scale));
        return $"{bytes / Math.Pow(scale, digitGroups):F2} {units[digitGroups]}";
    }

    public static MIMEType GetFileMIMEType(this UpFileInfo upFileInfo)
    {
        if (string.IsNullOrWhiteSpace(upFileInfo.ContentType))
            return MIMEType.Unknown;

        var contentType = upFileInfo.ContentType.ToLowerInvariant();

        return contentType switch
        {
            "application/x-zip-compressed" or
            "application/zip" => MIMEType.Zip,
            "application/x-rar-compressed" or
            "application/vnd.rar" => MIMEType.Rar,
            "application/x-7z-compressed" => MIMEType.SevenZip,
            "application/gzip" => MIMEType.Gzip,
            "application/x-tar" => MIMEType.Tar,

            "application/pdf" => MIMEType.PDF,
            "application/msword" => MIMEType.Word,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => MIMEType.Word,
            "application/vnd.ms-excel" => MIMEType.Excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => MIMEType.Excel,
            "application/vnd.ms-powerpoint" => MIMEType.PowerPoint,
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => MIMEType.PowerPoint,
            "text/plain" => MIMEType.Text,
            "text/csv" => MIMEType.CSV,

            "image/jpeg" or "image/jpg" => MIMEType.JPEG,
            "image/png" => MIMEType.PNG,
            "image/gif" => MIMEType.GIF,
            "image/svg+xml" => MIMEType.SVG,
            "image/webp" => MIMEType.WebP,

            "audio/mpeg" => MIMEType.MP3,
            "video/mp4" => MIMEType.MP4,
            "video/x-msvideo" => MIMEType.AVI,

            _ => MIMEType.Unknown
        };
    }
}
