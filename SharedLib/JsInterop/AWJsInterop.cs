using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using SharedLibrary.Models;
using SharedLibrary.Options;

namespace SharedLibrary.JsInterop;

public interface IJsInterop
{

}

public class AWJsInterop : IJsInterop, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public AWJsInterop(IJSRuntime jsRuntime, IOptions<JsModuleOptions> options)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", options.Value.SharedLib).AsTask());
    }

    public async Task TestConnection()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("testConnection");
    }

    public async Task<UpFileInfo> GetLocalFile(ElementReference inputElement)
    {
        var module = await moduleTask.Value;
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);
        var upFileInfo = await module.InvokeAsync<UpFileInfo>("getFilesInfo", fileHandler);

        var base64 = await module.InvokeAsync<string>("getFilesContent", fileHandler);
        var stream = new MemoryStream(Convert.FromBase64String(base64));
        upFileInfo.SetFileStream(stream);

        await fileHandler.DisposeAsync();
        return upFileInfo;
    }

    public async Task<IEnumerable<UpFileInfo>> GetLocalFiles(ElementReference inputElement)
    {
        var module = await moduleTask.Value;
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);
        var upFileInfos = await module.InvokeAsync<UpFileInfo[]>("getFilesInfo", fileHandler);

        var base64s = await module.InvokeAsync<string[]>("getFilesContent", fileHandler);

        for (int i = 0; i < upFileInfos.Length && i < base64s.Length; i++)
        {
            var bytes = Convert.FromBase64String(base64s[i]);
            var stream = new MemoryStream(bytes);
            upFileInfos[i].SetFileStream(stream);
        }

        await fileHandler.DisposeAsync();
        return upFileInfos;
    }

    public async Task DownloadFileAsync(string filename, byte[] data, string? mimeType = null)
    {
        var module = await moduleTask.Value;

        await module.InvokeVoidAsync("downloadFile", filename, data, mimeType);
    }

    public async Task DownloadTextAsync(string filename, string text, string? mimeType = null)
    {
        var module = await moduleTask.Value;

        await module.InvokeVoidAsync("downloadFile", filename, text, mimeType);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"模块释放失败: {ex.Message}");
        }
    }
}
