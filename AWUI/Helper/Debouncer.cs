namespace AWUI.Helper;

/// <summary>
/// Provides debounce functionality for high-frequency events.
/// 使用CancellationTokenSource实现防抖模式
/// </summary>
/// /// <remarks>
/// Usage example (使用示例):
/// <code>
/// var debouncedSave = Debouncer.Execute(SaveData, 1000);
/// input.OnChange += debouncedSave;
/// </code>
/// </remarks>
public static class Debouncer
{
    /// <summary>
    /// Wraps an action to prevent frequent execution.
    /// </summary>
    /// <param name="action">Target action</param>
    /// <param name="milliseconds">Minimum interval between executions</param>
    public static Action Execute(Action action, int milliseconds)
    {
        var obj = new object();
        CancellationTokenSource? cts = null;

        return () =>
        {
            lock (obj)
            {
                cts?.Cancel();
                cts?.Dispose();

                cts = new CancellationTokenSource();

                Task.Delay(milliseconds, cts.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled || cts.Token.IsCancellationRequested) return;

                    action.Invoke();
                }, TaskScheduler.Default);
            }
        };
    }

    /// <inheritdoc cref="Execute(Action, int)"/>
    public static Action<T> Execute<T>(Action<T> action, int milliseconds)
    {
        var obj = new object();
        CancellationTokenSource? cts = null;
        T? lastArg = default;

        return arg =>
        {
            lock (obj)
            {
                lastArg = arg;
                cts?.Cancel();
                cts?.Dispose();

                cts = new CancellationTokenSource();

                Task.Delay(milliseconds, cts.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled || cts.Token.IsCancellationRequested) return;

                    action.Invoke(lastArg);
                }, TaskScheduler.Default);
            }
        };
    }
}
