using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;

namespace AW.Server.Hubs;

public class FileUploadHub : Hub
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadRootDir;

    public FileUploadHub(IWebHostEnvironment env)
    {
        _env = env;
        _uploadRootDir = Path.Combine(_env.ContentRootPath, "uploads");
    }

    public async Task UploadChunk(string fileName, int chunkIndex, int totalChunks, byte[] chunkData)
    {
        try
        {
            // 创建文件临时目录（以文件名无后缀部分命名）
            var fileDir = Path.Combine(_uploadRootDir, Path.GetFileNameWithoutExtension(fileName));
            Directory.CreateDirectory(fileDir);
            // 保存分块文件
            var chunkPath = Path.Combine(fileDir, $"{chunkIndex}.tmp");
            await System.IO.File.WriteAllBytesAsync(chunkPath, chunkData);
            // 检查是否所有分块已上传完成
            var uploadedChunks = Directory.GetFiles(fileDir, "*.tmp").Length;
            if (uploadedChunks == totalChunks)
            {
                // 合并文件
                var mergeFilePath = Path.Combine(fileDir, fileName);
                await MergeChunks(fileDir, fileName, totalChunks);
                // 计算文件哈希
                var hash = await CalculateFileHash(mergeFilePath);
                // 通知客户端上传完成
                await Clients.Caller.SendAsync("UploadComplete", new
                {
                    FileName = fileName,
                    FileSize = new FileInfo(mergeFilePath).Length,
                    Hash = hash
                });
            }
            else
            {
                // 通知客户端分块上传成功
                await Clients.Caller.SendAsync("ChunkUploaded", chunkIndex);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("UploadError", ex.Message);
        }
    }
    private static async Task MergeChunks(string dir, string fileName, int totalChunks)
    {
        var mergePath = Path.Combine(dir, fileName);

        using (var output = File.Create(mergePath))
        {
            for (int i = 0; i < totalChunks; i++)
            {
                var chunkPath = Path.Combine(dir, $"{i}.tmp");
                using (var input = File.OpenRead(chunkPath))
                {
                    await input.CopyToAsync(output);
                }
                File.Delete(chunkPath); // 合并后删除分块
            }
        }
    }

    private static async Task<string> CalculateFileHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    // 客户端调用的取消上传方法
    public async Task CancelUpload(string fileName)
    {
        var fileDir = Path.Combine(_uploadRootDir, Path.GetFileNameWithoutExtension(fileName));
        if (Directory.Exists(fileDir))
        {
            Directory.Delete(fileDir, true);
            await Clients.Caller.SendAsync("UploadCancelled", fileName);
        }
    }
}
