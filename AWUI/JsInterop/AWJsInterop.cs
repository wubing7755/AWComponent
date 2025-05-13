using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using AWUI.Models;
using AWUI.Options;
using System.Collections.Concurrent;
using AWUI.Interfaces;

namespace AWUI.JsInterop;

public class AWJsInterop : IJsInterop
{
    private readonly Lazy<Task<IJSObjectReference>> _awModuleTask;
    protected readonly IJSRuntime jsRuntime;
    protected readonly JsModuleOptions ops;
    private readonly ConcurrentDictionary<string, Lazy<Task<IJSObjectReference>>> _modules = new();

    public AWJsInterop(IJSRuntime jsRuntime, IOptions<JsModuleOptions> ops)
    {
        this.jsRuntime = jsRuntime;
        this.ops = ops.Value;

        _awModuleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/AWUI/js/AWUI.js").AsTask());
    }

    public async Task TestConnection()
    {
        var module = await _awModuleTask.Value;
        await module.InvokeVoidAsync("testConnection");
    }

    public async Task<TValue> InvokeAsync<TValue>(string moduleName, string functionName, params object?[]? args)
    {
        var module = await GetModuleAsync(moduleName);
        return await module.InvokeAsync<TValue>(functionName, args);
    }

    public async Task InvokeVoidAsync(string moduleName, string functionName, params object?[]? args)
    {
        var module = await GetModuleAsync(moduleName);
        await module.InvokeVoidAsync(functionName, args);
    }

    private async Task<IJSObjectReference> GetModuleAsync(string moduleName)
    {
        var lazyTask = _modules.GetOrAdd(
            moduleName,
            name =>
            {
                var config = ops.Modules.FirstOrDefault(m =>
                    string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)
                ) ?? throw new ArgumentException($"JS模块 '{name}' 未配置");
                if (!config.Enable)
                    throw new InvalidOperationException($"JS模块 '{name}' 已禁用");
                return new Lazy<Task<IJSObjectReference>>(() =>
                    jsRuntime.InvokeAsync<IJSObjectReference>("import", config.Path).AsTask()
                );
            }
        );
        return await lazyTask.Value;
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
        var module = await _awModuleTask.Value;
        var fileHandler = await module.InvokeAsync<IJSObjectReference>("createFileHandler", inputElement);
        var upFileInfo = await module.InvokeAsync<UpFileInfo>("getFilesInfo", fileHandler);

        var content = await module.InvokeAsync<FileContentResult>("getFilesContent", fileHandler);

        // 小文件预览
        if(content.IsExLimit is null)
        {
            // 处理文件内容
            if (content.IsChunked && content.FileChunks != null)
            {
                // 分块文件处理
                var combinedBytes = CombineChunks(content.FileChunks);
                upFileInfo.SetFileStream(new MemoryStream(combinedBytes));
                upFileInfo.IsChunked = true;
                upFileInfo.TotalChunks = content.FileChunks.Count;
            }
            else if (!string.IsNullOrEmpty(content.Data))
            {
                // 小文件处理
                var bytes = Convert.FromBase64String(content.Data);
                upFileInfo.SetFileStream(new MemoryStream(bytes));
                upFileInfo.IsChunked = false;
            }
        }

        await fileHandler.DisposeAsync();
        return upFileInfo;
    }

    private static byte[] CombineChunks(List<FileChunk> chunks)
    {
        // 确保按正确顺序合并
        var orderedChunks = chunks.OrderBy(c => c.Index).ToList();
        var totalSize = orderedChunks.Sum(c => Convert.FromBase64String(c.Data).Length);
        var result = new byte[totalSize];
        var offset = 0;

        foreach (var chunk in orderedChunks)
        {
            var chunkBytes = Convert.FromBase64String(chunk.Data);
            Buffer.BlockCopy(chunkBytes, 0, result, offset, chunkBytes.Length);
            offset += chunkBytes.Length;
        }

        return result;
    }

    public async Task<FileContentResult> GetLoaclFileBit(ElementReference inputElement)
    {
        var module = await _awModuleTask.Value;
        var content = await module.InvokeAsync<FileContentResult>("chunkFilesUpload", inputElement);
        return content;
    }

    public async Task<IEnumerable<UpFileInfo>> GetLocalFiles(ElementReference inputElement)
    {
        var module = await _awModuleTask.Value;
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
        var module = await _awModuleTask.Value;

        await module.InvokeVoidAsync("downloadFile", filename, data, mimeType);
    }

    public async Task DownloadTextAsync(string filename, string text, string? mimeType = null)
    {
        var module = await _awModuleTask.Value;

        await module.InvokeVoidAsync("downloadFile", filename, text, mimeType);
    }

    public async Task UploadFileAsync(ElementReference inputElement, string url)
    {
        var module = await _awModuleTask.Value;
        var chunkSize = 1024 * 100;

        await module.InvokeVoidAsync("uploadFile", inputElement, url, chunkSize);
    }

    public async Task UploadFilesAsync(ElementReference inputElement, string url)
    {
        var module = await _awModuleTask.Value;
        var chunkSize = 1024 * 1024 * 10;

        await module.InvokeVoidAsync("uploadFiles", inputElement, url, chunkSize);
    }

    public virtual async ValueTask DisposeAsync()
    {
        try
        {
            if (_awModuleTask.IsValueCreated)
            {
                var module = await _awModuleTask.Value;
                await module.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"模块释放失败: {ex.Message}");
        }
    }
}
