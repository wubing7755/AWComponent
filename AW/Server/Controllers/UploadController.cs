using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace AW.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    // 文件上传根目录
    private readonly string _uploadRootDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    [HttpPost("chunk")]
    public async Task<IActionResult> UploadChunk([FromForm] string fileName, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] IFormFile chunkData)
    {
        var fileDir = Path.Combine(_uploadRootDir, fileName.Split(".")[0]);
        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        var chunkFilePath = Path.Combine(fileDir, $"{chunkIndex}.tmp");
        using (var stream = System.IO.File.Create(chunkFilePath))
        {
            await chunkData.CopyToAsync(stream);
        }

        if (Directory.GetFiles(fileDir, "*.tmp").Length == totalChunks)
        {
            await MergeChunks(fileDir, fileName, totalChunks);

            var mergeFilePath = Path.Combine(fileDir, fileName);
            using var stream = System.IO.File.OpenRead(mergeFilePath);
            using var sha = SHA256.Create();
            var hashBytes = await Task.Run(() => sha.ComputeHash(stream));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            var hashFilePath = Path.ChangeExtension(mergeFilePath, ".hash");
            await System.IO.File.WriteAllTextAsync(hashFilePath, hashString);

            return Ok(new { completed = true });
        }

        return Ok(new { completed = false });
    }

    private async Task MergeChunks(string dir, string fileName, int totalChunks)
    {
        using (var output = System.IO.File.Create(Path.Combine(_uploadRootDir, fileName.Split(".")[0], fileName)))
        {
            for (int i = 0; i < totalChunks; i++)
            {
                var chunkPath = Path.Combine(dir, $"{i}.tmp");

                using (var input = System.IO.File.OpenRead(chunkPath))
                {
                    await input.CopyToAsync(output);
                }

                System.IO.File.Delete(chunkPath);
            }
        }
    }
}
