using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Enums;
using AWUI.Helper;

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

        builder.OpenElement(3, "div");
        builder.AddAttribute(4, "style", $"background-color: {ColorHelper.ConvertToString(Color)};text-align:center;");
        builder.OpenElement(5, "span");
        builder.AddAttribute(6, "style", "display: inline-block; text-align: center; line-height: 1em; font-size: white-space: nowrap; overflow: hidden;");

        if (ChildContent is not null)
        {
            builder.AddContent(7, ChildContent);
        }
        else
        {
            builder.AddContent(7, string.Empty);
        }

        builder.CloseElement();
        builder.CloseElement();

        builder.CloseElement();
    }
}
