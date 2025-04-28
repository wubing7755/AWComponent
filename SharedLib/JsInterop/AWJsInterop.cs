using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using SharedLibrary.Components;
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
        await module.InvokeVoidAsync("testConnection");
    }

    public async Task<UpFileModel> GetLocalFile(ElementReference inputElement)
    {
        var module = await moduleTask.Value;

        // 使用工厂函数创建实例
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);

        // 获取文件属性
        var browserFile = await module.InvokeAsync<UpFileModel>("getFileAttributes", fileHandler);

        // 获取流引用
        var objRef = await module.InvokeAsync<IJSStreamReference>("getFileStreamReference", fileHandler);

        browserFile.JsStreamReference = objRef;

        // 释放 JS 对象引用
        await fileHandler.DisposeAsync();

        return browserFile;
    }

    public async Task<IEnumerable<UpFileModel>> GetLocalFiles(ElementReference inputElement)
    {
        var module = await moduleTask.Value;

        // 使用工厂函数创建实例
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);
        
        // 获取文件属性
        var browserFiles = await module.InvokeAsync<UpFileModel[]>("getFileAttributes", fileHandler);


        // 释放 JS 对象引用
        await fileHandler.DisposeAsync();

        return browserFiles;
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
