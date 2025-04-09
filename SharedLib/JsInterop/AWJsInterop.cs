using Microsoft.JSInterop;

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

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
