using Microsoft.JSInterop;
using SharedLibrary.Services;
using System.Text;
using System.Xml.Serialization;

namespace SharedLibrary.Utils;

public static class XmlWriter
{
    private static Lazy<Task<IJSObjectReference>> _jsModuleTask { get; set; }

    public static async Task Initialize(IJSRuntime jsRuntime)
    {
        _jsModuleTask = new Lazy<Task<IJSObjectReference>>(() =>
                    jsRuntime.InvokeAsync<IJSObjectReference>(
                        "import",
                        "./_content/SharedLibrary/js/SharedLib.js"
                    ).AsTask()
                );
    }

    public static async Task<Result> WriteToFile<T>(T obj, string? fileName = null)
    {
        try
        {
            var jsModule = await _jsModuleTask.Value;
            Console.WriteLine("当前工作目录：" + Environment.CurrentDirectory);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "AWtemp.xml";
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, obj);
            memoryStream.Position = 0;

            // 转换为 UTF-8 文本
            var xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());

            await jsModule.InvokeVoidAsync("downloadFile", fileName, xmlString, "application/xml");

            return Result.Ok;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"权限不足：{ex.Message}");
            return Result.NotOk;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO错误：{ex.Message}");
            return Result.NotOk;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"序列化失败： {ex.Message}");
            return Result.NotOk;
        }
    }
}
