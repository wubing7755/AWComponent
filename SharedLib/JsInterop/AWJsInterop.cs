using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedLibrary.Services;

namespace SharedLibrary.JsInterop;

public class AWJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public AWJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SharedLibrary/js/SharedLib.js").AsTask());
    }

    public async void SayHello()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("SayHello");
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

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
