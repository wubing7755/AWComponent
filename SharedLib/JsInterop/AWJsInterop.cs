using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using SharedLibrary.Options;
using SharedLibrary.Services;

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
        await module.InvokeVoidAsync("TestConnection");
    }

    public async Task<IBrowserFile> GetLocalFile(ElementReference inputElement)
    {
        var module = await moduleTask.Value;

        var result = await module.InvokeAsync<IJSObjectReference>("GetLocalFile", inputElement);
        var browserFile = await result.InvokeAsync<BrowserFile>("getAttributes");

        browserFile.JsFileReference = result;

        return browserFile;
    }

    public async Task<IEnumerable<IBrowserFile>> GetLocalFiles(ElementReference inputElement)
    {
        var module = await moduleTask.Value;

        return await module.InvokeAsync<IEnumerable<IBrowserFile>>("GetLocalFiles", inputElement);
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
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
