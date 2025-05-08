using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using SharedLibrary.Components;
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

    /// <summary>
    /// 获取单个文件信息及文件流（从浏览器本地文件系统）
    /// </summary>
    /// <param name="inputElement">文件输入框的DOM元素引用，通常对应<input type="file">元素</param>
    /// <returns>
    /// 包含文件元数据(如文件名、大小、类型等)和文件流数据的UpFileInfo对象
    /// 注意：返回的对象中包含的流需要调用方负责释放
    /// </returns>
    /// <exception cref="JSException">可能抛出JavaScript互操作相关的异常</exception>
    /// <exception cref="ArgumentException">当base64解码失败时可能抛出</exception>
    /// <remarks>
    /// 实现原理：
    /// 1. 通过JavaScript互操作获取文件引用
    /// 2. 分两步获取文件元数据和内容（避免大文件一次性加载）
    /// 3. 自动释放JavaScript端的文件引用
    /// </remarks>
    public async Task<UpFileInfo> GetLocalFile(ElementReference inputElement)
    {
        var module = await moduleTask.Value;
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);
        var upFileInfo = await module.InvokeAsync<UpFileInfo>("getFilesInfo", fileHandler);

        var base64 = await module.InvokeAsync<string>("getFilesContent", fileHandler);
        
        if(base64 is not null)
        {
            var stream = new MemoryStream(Convert.FromBase64String(base64));
            upFileInfo.SetFileStream(stream);
        }

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

    /// <summary>
    /// 通过浏览器下载文件（使用JavaScript互操作）
    /// </summary>
    /// <param name="filename">下载的文件名（包含扩展名，如："report.pdf"）</param>
    /// <param name="data">文件内容字节数组</param>
    /// <param name="mimeType">
    /// 可选的文件MIME类型（如："application/pdf"）。
    /// 如果未指定，将根据文件扩展名自动推断：
    /// - .txt → text/plain
    /// - .pdf → application/pdf
    /// - .png → image/png
    /// - 其他 → application/octet-stream
    /// </param>
    /// <returns>表示异步操作的Task</returns>
    /// <exception cref="JSException">当JavaScript互操作失败时抛出</exception>
    /// <remarks>
    /// 实现原理：
    /// 1. 调用预加载的JavaScript模块
    /// 2. 在浏览器端通过创建Blob对象实现下载
    /// 3. 自动处理内存回收（URL.revokeObjectURL）
    /// 
    /// 典型使用场景：
    /// var data = File.ReadAllBytes("report.pdf");
    /// await DownloadFileAsync("季度报告.pdf", data, "application/pdf");
    /// 
    /// 注意：
    /// - 对于大文件（>100MB）建议使用流式下载
    /// - 在移动端可能受浏览器安全限制
    /// 
    /// 下载地址：
    /// - 用户浏览器配置的默认下载目录
    /// </remarks>
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

    public async Task UploadFileAsync(ElementReference inputElement, string url)
    {
        var module = await moduleTask.Value;
        var chunkSize = 1024 * 100;

        await module.InvokeVoidAsync("uploadFile", inputElement, url, chunkSize);
    }

    public async Task UploadFilesAsync(ElementReference inputElement, string url)
    {
        var module = await moduleTask.Value;
        var chunkSize = 1024 * 1024 * 10;

        await module.InvokeVoidAsync("uploadFiles", inputElement, url, chunkSize);
    }

    /// <summary>
    /// 为SVG元素添加监听器
    /// </summary>
    /// <returns></returns>
    public async Task InitializeDraggableSVGElement<TValue>(
        ElementReference inputElement, 
        DotNetObjectReference<SvgElementBase<TValue>> dotNetObjRef,
        double x,
        double y) where TValue : DraggableSvgElementModel
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("initializeDraggableSVGElement", inputElement, dotNetObjRef, x, y);
    }

    /// <summary>
    /// 清除SVG元素的监听器
    /// </summary>
    /// <param name="inputElement"></param>
    /// <returns></returns>
    public async Task CleanUpDraggableSVGElement(ElementReference inputElement)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("cleanUpDraggableSVGElement", inputElement);
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
