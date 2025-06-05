namespace AWUI.Services;

/// <summary>
/// 组件状态上下文
/// </summary>
public class ComponentContext
{
    public Guid Id { get; }

    public int RenderCount { get; set; }
}
