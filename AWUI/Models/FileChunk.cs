namespace AWUI.Models;

public class FileChunk
{
    public int Index { get; set; }
    public int Total { get; set; }
    public string Data { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class FileContentResult
{
    public bool IsChunked { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? Data { get; set; }
    public List<FileChunk>? FileChunks { get; set; }

    public bool? IsExLimit { get; set; }
}
