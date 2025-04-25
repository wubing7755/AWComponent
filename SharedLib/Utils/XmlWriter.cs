using SharedLibrary.Services;
using System.Xml.Serialization;

namespace SharedLibrary.Utils;

public static class XmlWriter
{
    public static Result WriteToFile<T>(T obj, string? fileName = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "output/AWtemp.xml";
            }

            var dict = Path.GetDirectoryName(fileName);

            if (!string.IsNullOrEmpty(dict) && !Directory.Exists(dict))
            {
                Directory.CreateDirectory(dict);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, obj);
            }

            return Result.Ok;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"序列化失败： {ex.Message}");
            return Result.NotOk;
        }
    }
}
