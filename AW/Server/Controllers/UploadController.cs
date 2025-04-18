using Microsoft.AspNetCore.Mvc;

namespace AW.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly string _tempPath = Path.GetTempPath();

    [HttpPost("chunk")]
    public async Task<IActionResult> UploadChunk(
        IFormFile chunck,
        int chunkIndex,
        int totalChunks,
        string fileName)
    {
        var tempDir = Path.Combine(_tempPath, fileName);
        Directory.CreateDirectory(tempDir);

        var chunkPath = Path.Combine(tempDir, $"{chunkIndex}.tmp");
        using (var stream = System.IO.File.Create(chunkPath))
        {
            await chunck.CopyToAsync(stream);
        }

        if (AllChunksUploaded(tempDir, totalChunks))
        {
            await MergeChunks(tempDir, fileName);
            return Ok(new { completed = true });
        }

        return Ok(new { completed = false });
    }

    private bool AllChunksUploaded(string dir, int total)
        => Directory.GetFiles(dir).Length == total;

    private async Task MergeChunks(string dir, string fileName)
    {
        var finalPath = Path.Combine("wwwroot/uploads", fileName);
        using (var output = System.IO.File.Create(finalPath))
        {
            for (int i = 0; i < Directory.GetFiles(dir).Length; i++)
            {
                var chunkPath = Path.Combine(dir, $"{i}.tmp");
                using (var input = System.IO.File.OpenRead(chunkPath))
                {
                    await input.CopyToAsync(output);
                }
                System.IO.File.Delete(chunkPath);
            }
        }
        Directory.Delete(dir);
    }
}
