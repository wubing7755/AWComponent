using System.Text;

namespace AWUI.Helper;

/// <summary>
/// 构建CSS类字符串
/// </summary>
public sealed class CssBuilder
{
    private readonly StringBuilder _buffer = new(32);
    private bool _hasContent;

    public static CssBuilder Default => new();

    /// <summary>
    /// 添加CSS类
    /// </summary>
    public CssBuilder AddClass(string? cls)
    {
        if (string.IsNullOrWhiteSpace(cls))
            return this;

        if (_hasContent)
            _buffer.Append(' ');

        _buffer.Append(cls.Trim());
        _hasContent = true;

        return this;
    }

    /// <summary>
    /// 添加条件CSS类
    /// </summary>
    public CssBuilder AddClass(string? cls, bool when)
    {
        return when ? AddClass(cls) : this;
    }

    /// <summary>
    /// 添加条件CSS类
    /// </summary>
    public CssBuilder AddClass(string? cls, Func<bool> when)
    {
        return when() ? AddClass(cls) : this;
    }

    /// <summary>
    /// 从字典属性中添加CSS类
    /// </summary>
    public CssBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes == null || !attributes.TryGetValue("class", out var classObj))
            return this;

        return AddClass(classObj.ToString());
    }

    /// <summary>
    /// 生成最终CSS类字符串
    /// </summary>
    public string Build()
    {
        return _hasContent ? _buffer.ToString() : string.Empty;
    }

    /// <summary>
    /// 隐式转换为string
    /// </summary>
    public static implicit operator string?(CssBuilder builder) => builder?.Build();
}
