using AWUI.JsInterop;
using System.Text;
using System.Xml.Serialization;

namespace AWUI.Helper;

public class XmlWriter
{
    private readonly AWJsInterop _jsInterop;

    public XmlWriter(AWJsInterop jsInterop)
    {
        _jsInterop = jsInterop;
    }

    public async Task<Result> WriteToFile<T>(T obj, string? fileName = null)
    {
        try
        {
            if (_jsInterop is null)
            {
                throw new InvalidOperationException("JsInterop 未初始化");
            }

            Console.WriteLine("当前工作目录：" + Environment.CurrentDirectory);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "AWTemp.xml";
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, obj);
            memoryStream.Position = 0;

            // 转换为 UTF-8 文本
            var xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());

            await _jsInterop.DownloadTextAsync(fileName, xmlString, "application/xml");

            return Result.Ok;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"序列化失败： {ex.Message}");
            return Result.NotOk;
        }
    }
}
