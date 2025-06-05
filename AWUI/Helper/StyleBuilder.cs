using System.Text;

namespace AWUI.Helper;

/// <summary>
/// 构建Style字符串
/// </summary>
public sealed class StyleBuilder
{
    private readonly StringBuilder _buffer = new(32);
    private bool _hasContent;

    public static StyleBuilder Default => new();

    /// <summary>
    /// 添加Style
    /// </summary>
    public StyleBuilder AddStyle(string? cls)
    {
        if (string.IsNullOrWhiteSpace(cls))
            return this;

        if (_hasContent)
            _buffer.Append(';');

        _buffer.Append(cls.Trim());
        _hasContent = true;

        return this;
    }

    /// <summary>
    /// 添加条件Style
    /// </summary>
    public StyleBuilder AddStyle(string? cls, bool when)
    {
        return when ? AddStyle(cls) : this;
    }

    /// <summary>
    /// 添加条件Style
    /// </summary>
    public StyleBuilder AddStyle(string? cls, Func<bool> when)
    {
        return when() ? AddStyle(cls) : this;
    }

    /// <summary>
    /// 从字典属性中添加Style
    /// </summary>
    public StyleBuilder AddStyleFromAttributes(IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes == null || !attributes.TryGetValue("style", out var classObj))
            return this;

        return AddStyle(classObj.ToString());
    }

    /// <summary>
    /// 生成最终Style字符串
    /// </summary>
    public string Build()
    {
        return _hasContent ? _buffer.ToString() : string.Empty;
    }

    /// <summary>
    /// 隐式转换为string
    /// </summary>
    public static implicit operator string?(StyleBuilder builder) => builder?.Build();
}
