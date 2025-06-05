namespace AWUI.Interfaces;

public interface IJSInterop : IAsyncDisposable
{
    /// <summary>
    /// 调用指定JS模块的方法（无返回值）
    /// </summary>
    Task InvokeVoidAsync(string moduleName, string functionName, params object?[]? args);

    /// <summary>
    /// 调用指定JS模块的方法（带返回值）
    /// </summary>
    Task<TValue> InvokeAsync<TValue>(string moduleName, string functionName, params object?[]? args);
}

