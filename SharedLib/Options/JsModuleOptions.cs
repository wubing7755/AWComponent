using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Options;

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

    /// <summary>
    /// 共享库模块路径/URL
    /// Shared library module path/URL
    /// </summary>
    public string SharedLib { get; set; } = string.Empty;
}
