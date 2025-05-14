namespace AWUI.Models;

public class FileMetaData : ICloneable
{
    public FileMetaData() { }

    public FileMetaData(string fileName, long size, DateTimeOffset lastModified, string contentType)
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

    public FileMetaData Clone()
    {
        var cloned = new FileMetaData
        {
            Name = Name,
            LastModified = LastModified,
            ContentType = ContentType,
            Size = Size
        };

        return cloned;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
