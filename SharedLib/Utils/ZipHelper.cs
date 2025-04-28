using Microsoft.JSInterop;
using SharedLibrary.Components;
using System.IO.Compression;
using System.Text;

namespace SharedLibrary.Utils;

public class ZipHelper
{
    public async Task DecodingZip(UpFileModel fileModel)
    {
        var name = fileModel.FileName;

        // 读取文件内容
        var fileRef = fileModel.JsStreamReference;

        // 传入 CancellationTokenSource，设置超时为 1 秒
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        // 打开流并直接复制到内存流（单次操作）
        using var memoryStream = new MemoryStream();
        await(await fileRef.OpenReadStreamAsync(512000, cts.Token)).CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        // 解析 ZIP 文件
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, leaveOpen: true);

        // 存储解压后的文件内容
        var content = await Task.WhenAll(
            archive.Entries
                .Where(entry => !string.IsNullOrEmpty(entry.Name))
                .Select(async entry =>
                {
                    await using var entryStream = entry.Open();
                    using var ms = new MemoryStream();
                    await entryStream.CopyToAsync(ms);
                    return (entry.FullName, Content: ms.ToArray());
                }));

        var contentDict = content.ToDictionary(x => x.FullName, x => x.Content);

        // 更合理的输出方式
        foreach (var item in contentDict)
        {
            Console.WriteLine($"File: {item.Key}");
            Console.WriteLine($"Size: {item.Value.Length} bytes");

            // 如果是文本文件（根据扩展名判断）
            if (item.Key.EndsWith(".txt") || item.Key.EndsWith(".csv") ||
                item.Key.EndsWith(".json") || item.Key.EndsWith(".xml"))
            {
                Console.WriteLine("Content:");
                Console.WriteLine(Encoding.UTF8.GetString(item.Value));
            }
            Console.WriteLine(new string('-', 40));
        }
    }
}
