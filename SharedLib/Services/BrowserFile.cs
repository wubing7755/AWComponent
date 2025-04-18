using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
namespace SharedLibrary.Services;

public class BrowserFile : IBrowserFile
{
    [NotNull]
    public string? Name { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public long Size { get; set; }

    [NotNull]
    public string? ContentType { get; set; }

    public IJSObjectReference? JsFileReference { get; set; }

    Stream IBrowserFile.OpenReadStream(long maxAllowedSize, CancellationToken cancellationToken)
    {
        if (JsFileReference == null)
            throw new InvalidOperationException("JS file reference not available.");
        
        // 这里需要调用 JS 方法来获取流
        // 由于 JS 流不能直接映射到 C# Stream，我们需要使用 JS Interop 读取数据
        throw new NotImplementedException("Need to implement JS-based stream reading.");
    }
}
