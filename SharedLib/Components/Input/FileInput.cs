using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Components;

public class FileInput<TValue> : Input<TValue> where TValue : UpFileModel
{
    [Parameter]
    public bool Multiple { get; set; } = false;

    [Parameter]
    public MIMEType MIMEType { get; set; }

    [Parameter]
    public EventCallback<IEnumerable<TValue>> MultipleValueChanged { get; set; }

    protected override void BuildComponentAttributes(RenderTreeBuilder builder, ref int seq)
    {
        builder.AddAttribute(seq++, "type", InputType.File);

        builder.AddAttribute(seq++, "multiple", Multiple);

        switch (MIMEType)
        {
            case MIMEType.Zip:
                builder.AddAttribute(seq++, "accept", ".zip,.rar,.7z");
                break;
            default:
                break;
        }

        builder.AddAttribute(seq, "onchange", EventCallback.Factory.Create<ChangeEventArgs>
            (this, async () => await HandleFileChange()));
    }

    private async Task HandleFileChange()
    {
        if (!Multiple)
        {
            var file = await JsInterop.GetLocalFile(inputElement);

            if (file is not null)
            {
                if (currentValue is null)
                {
                    currentValue = Activator.CreateInstance<TValue>();
                }
                currentValue.FileName = file.FileName;
                currentValue.Size = file.Size;
                currentValue.LastModified = file.LastModified;
                currentValue.ContentType = file.ContentType;
                
                if(file.JsStreamReference is not null)
                {
                    currentValue.JsStreamReference = file.JsStreamReference;
                }

                await ValueChanged.InvokeAsync(currentValue);
            }
        }
        else
        {
            var files = await JsInterop.GetLocalFiles(inputElement);
            var values = new List<TValue>();

            foreach(var file in files)
            {
                var model = Activator.CreateInstance<TValue>();

                model.FileName = file.FileName;
                model.Size = file.Size;
                model.LastModified = file.LastModified;
                model.ContentType = file.ContentType;

                if (file.JsStreamReference is not null)
                {
                    model.JsStreamReference = file.JsStreamReference;
                }
                values.Add(model);
            }

            await MultipleValueChanged.InvokeAsync(values);
        }
    }
}

public class UpFileModel
{
    public UpFileModel()
    {

    }

    public UpFileModel(
        string fileName, 
        long size, 
        DateTimeOffset lastModified, 
        string contentType,
        IJSStreamReference? jsFileReference = null)
    {
        FileName = fileName;
        Size = size;
        LastModified = lastModified;
        ContentType = contentType;
        JsStreamReference = jsFileReference;
    }

    public string? FileName { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public long Size { get; set; }

    public string FormatSize => FormatFileSize(Size);

    private static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 Bytes";
        const int scale = 1024;
        string[] units = { "Bytes", "KB", "MB", "GB", "TB" };
        int digitGroups = (int)(Math.Log(bytes) / Math.Log(scale));
        return $"{bytes / Math.Pow(scale, digitGroups):F2} {units[digitGroups]}";
    }

    [NotNull]
    public string? ContentType { get; set; }

    public IJSStreamReference? JsStreamReference { get; set; }

    public MIMEType Type => GetFileContentType();

    private MIMEType GetFileContentType()
    {
        if (string.IsNullOrWhiteSpace(ContentType))
            return MIMEType.Unknown;

        var contentType = ContentType.ToLowerInvariant();

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

public enum MIMEType
{
    Unknown,
    Zip,
    Rar,
    SevenZip,
    Gzip,
    Tar,
    PDF,
    Word,
    Excel,
    PowerPoint,
    Text,
    CSV,
    JPEG,
    PNG,
    GIF,
    SVG,
    WebP,
    MP3,
    MP4,
    AVI
}
