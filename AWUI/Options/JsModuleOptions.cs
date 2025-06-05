using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AWUI.Options;

/// <summary>
/// JavaScript Module configuration options (loaded from appsettings.json)
/// JavaScript模块配置选项（从 appsettings.json 加载）
/// </summary>
/// <remarks>
/// Used to centrally manage paths for frontend JavaScript modules
/// 用于集中管理前端JavaScript模块的路径配置
/// </remarks>
public class JsModuleOptions
{
    /// <summary>
    /// Configuration section name (matches the node name in appsettings.json)
    /// 配置节名称（对应 appsettings.json 中的节点名）
    /// </summary>
    public const string SectionName = "JsModules";

    [JsonPropertyName("Version")]
    [RegularExpression(@"^\d+\.\d+\.\d+$")]
    public string? Version { get; set; }

    [JsonPropertyName("Modules")]
    public List<JsModuleOption> Modules { get; set; } = new ();
}

public class JsModuleOption
{
    [Required]
    [JsonPropertyName("Name")]
    public string Name { get; set; } = default!;

    [Required]
    [JsonPropertyName("Path")]
    public string Path { get; set; } = default!;

    [JsonPropertyName("Enable")]
    public bool Enable { get; set; } = true;
}