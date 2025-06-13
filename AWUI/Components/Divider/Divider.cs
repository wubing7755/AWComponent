using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Enums;

namespace AWUI.Components;

/// <summary>
/// 分隔器
/// </summary>
public class Divider : AWComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public ColorType Color { get; set; }

    [Parameter]
    public GraphType GraphType { get; set; } = GraphType.Line;

    [Parameter]
    public float Height { get; set; } = 30.0f;

    protected sealed override string BaseClass => "aw-divider";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        CssVariables["--divider-height"] = Height + "px";

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", ComputedStyle);

        if (ChildContent is not null)
        {
            builder.OpenElement(3, "div");
            builder.AddAttribute(4, "style", "text-align:center;");
            builder.OpenElement(5, "span");
            builder.AddAttribute(6, "style",
                "display: inline-block; text-align: center; line-height: 1em; font-size: white-space: nowrap; overflow: hidden;");

            builder.AddContent(7, ChildContent);

            builder.CloseElement();
            builder.CloseElement();
        }

        builder.CloseElement();
    }
}
