using System.Text;

namespace AW.Server.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LoggingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;

        // 日志目录路径
        var logDirectory = Path.Combine(env.ContentRootPath, "uploads");

        if(!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 日志文件路径
        _logFilePath = Path.Combine(logDirectory, "log.txt");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} | " +
                        $"{context.Request.Method} | " +
                        $"{context.Request.Path}{context.Request.QueryString}\n";
        try
        {
            await _semaphore.WaitAsync();
            await File.AppendAllTextAsync(_logFilePath, logMessage, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write to log file: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }

        await _next(context);
    }
}
