using Microsoft.AspNetCore.SignalR;

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
        // 使用连接ID隔离目录
        var fileDir = Path.Combine(_uploadRootDir, $"{Context.ConnectionId}_{Path.GetFileNameWithoutExtension(fileName)}");
        Directory.CreateDirectory(fileDir);

        var chunkPath = Path.Combine(fileDir, $"{chunkIndex}.tmp");
        await File.WriteAllBytesAsync(chunkPath, chunkData);

        if (Directory.GetFiles(fileDir, "*.tmp").Length == totalChunks)
        {
            await MergeChunks(fileDir, fileName, totalChunks);
        }
    }

    // 带缓冲的合并方法
    private static async Task MergeChunks(string dir, string fileName, int totalChunks)
    {
        using var output = File.Create(Path.Combine(dir, fileName));
        for (int i = 0; i < totalChunks; i++)
        {
            var chunkPath = Path.Combine(dir, $"{i}.tmp");

            using (var input = File.OpenRead(chunkPath))
            {
                await input.CopyToAsync(output);
            }
                
            File.Delete(chunkPath);
        }
    }
}
