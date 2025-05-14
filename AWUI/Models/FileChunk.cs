namespace AWUI.Models;

public class FileChunk
{
    public int Index { get; }
    public int Total { get; }
    public string Data { get;}
    public long Size { get; }
    public bool IsLastChunk { get; }
}

public class FileContent
{
    public string? FileData { get; }    // 未分块的文件数据

    public List<FileChunk>? FileChunks { get; }
}

public class FileContentResult
{
    public bool Success { get; }
    public string? FileName { get; }
    public long? FileSize { get; }
    public FileContent? FileContent { get; }
}
