
using Microsoft.AspNetCore.Components.Forms;

namespace SharedLibrary.Models;

public class UpFileInfo : IBrowserFile, ICloneable
{
    public UpFileInfo() { }

    public UpFileInfo(string fileName, long size, DateTimeOffset lastModified, string contentType)
    {
        Name = fileName;
        Size = size;
        LastModified = lastModified;
        ContentType = contentType;
    }

    public string? Name { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public long Size { get; set; }

    public string? ContentType { get; set; }

    private Stream? _stream;

    public void SetFileStream(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream), "提供的文件流不能为空。");

        if (!_stream.CanRead)
        {
            throw new ArgumentException("提供的流必须是可读取的。", nameof(stream));
        }
    }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        if (Size > maxAllowedSize)
        {
            throw new IOException($"文件大小 {Size} 字节，超出最大允许的 {maxAllowedSize} 字节。");
        }

        if (_stream is null)
        {
            throw new InvalidOperationException("文件流尚未初始化，请先调用 SetFileStream(Stream) 方法绑定文件流。");
        }

        if (!_stream.CanRead)
        {
            throw new IOException("文件流不可读取。");
        }

        _stream.Position = 0; // 读之前确保回到开头
        return _stream;
    }

    public UpFileInfo Clone()
    {
        var cloned = new UpFileInfo
        {
            Name = Name,
            LastModified = LastModified,
            ContentType = ContentType,
            Size = Size,
            _stream = _stream,
        };

        return cloned;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
