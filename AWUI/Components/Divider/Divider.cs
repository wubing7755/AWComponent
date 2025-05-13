using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AWUI.Components;

/// <summary>
/// 分隔器
/// </summary>
public class Divider : AWComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected sealed override string BaseCssClass => "aw-divider";

    protected virtual string DividerClass => BuildCssClass();

    protected virtual string DividerStyle => BuildStyle();

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, SafeAttributes);
        builder.AddAttribute(2, "class", DividerClass);
        builder.AddAttribute(3, "style", DividerStyle);

        // SVG Dotted
        builder.OpenElement(4, "svg");
        builder.AddAttribute(5, "style",
            "width: 100%;" +
            "height: 30%;" +
            "aria-hidden: true;");
        builder.OpenElement(6, "line");
        builder.AddMultipleAttributes(7, new Dictionary<string, object>
        {
            { "x1", "0%" },
            { "y1", "50%" },
            { "x2", "100%" },
            { "y2", "50%" },
            { "stroke", "black" },
            { "stroke-width", "1" },
            { "stroke-dasharray", "5 3" }
        });
        builder.CloseElement();
        builder.CloseElement();

        if (ChildContent is not null)
        {
            builder.AddMarkupContent(8, "<div style=\"text-align:center;margin-top:-15px;\">");
            builder.OpenElement(9, "span");
            builder.AddContent(10, ChildContent);
            builder.CloseElement();
            builder.AddMarkupContent(11, "</div>");
        }

        builder.CloseElement();
    }
}
