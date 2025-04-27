using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace SharedLibrary.Components;


public class FileInput<TValue> : Input<TValue> where TValue : FileModel
{
    [Parameter]
    public bool Multiple { get; set; } = false;

    [Parameter]
    public FileType FileType { get; set; }

    protected override void BuildComponentAttributes(RenderTreeBuilder builder, ref int seq)
    {
        builder.AddAttribute(seq++, "type", InputType.File);

        builder.AddAttribute(seq++, "multiple", Multiple);

        switch (FileType)
        {
            case FileType.Archive:
                builder.AddAttribute(seq++, "accept", ".zip,.rar,.7z");
                break;
            default:
                break;
        }

        builder.AddAttribute(seq, "onchange", EventCallback.Factory.Create<ChangeEventArgs>
            (this, async args => await HandleFileChange(args)));
    }

    private async Task HandleFileChange(ChangeEventArgs args)
    {
        var filePath = args.Value;

        if (!Multiple)
        {
            var file = await JsInterop.GetLocalFile(inputElement);

            if (file is not null)
            {
                if (currentValue is null)
                {
                    currentValue = Activator.CreateInstance<TValue>();
                }
                currentValue.FileName = file.Name;
                currentValue.Size = file.Size;
                currentValue.File = file;

                await ValueChanged.InvokeAsync(currentValue);
            }
        }
        else
        {

            var files = await JsInterop.GetLocalFiles(inputElement);
        }
    }
}

public class FileModel
{
    public string? FileName { get; set; }

    public long Size { get; set; }

    public string FormatSize => FormatFileSize(Size);

    public IBrowserFile? File { get; set; }

    private static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 Bytes";
        const int scale = 1024;
        string[] units = { "Bytes", "KB", "MB", "GB", "TB" };
        int digitGroups = (int)(Math.Log(bytes) / Math.Log(scale));
        return $"{bytes / Math.Pow(scale, digitGroups):F2} {units[digitGroups]}";
    }
}

public enum FileType
{
    Image,
    Video,
    Audio,
    Document,
    Archive
}

