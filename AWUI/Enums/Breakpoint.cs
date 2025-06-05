namespace AWUI.Enums;

/// <summary>
/// Layout Breakpoint. Defines responsive breakpoints for UI components.
/// 布局断裂点。定义响应式布局的标准断点。
/// </summary>
/// <remarks>
/// Values represent minimum viewport widths in pixels.
/// </remarks>
public enum Breakpoint
{
    None = 0,
    ExtraSmall = 256,
    Small = 512,
    Medium = 640,
    Large = 1024,
    ExtraLarge = 2048,
    Custom
}
