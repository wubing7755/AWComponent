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

    protected sealed override string BaseClass => "aw-divider";

    private readonly string _svgStyle = "width: 100%; height: 30%; aria-hidden: true;";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, SafeAttributes);
        builder.AddAttribute(2, "class", ComputedClass);
        builder.AddAttribute(3, "style", ComputedStyle);

        // SVG Dotted
        builder.OpenElement(4, "svg");

        builder.AddAttribute(5, "style", _svgStyle);
        builder.OpenElement(6, "line");
        builder.AddMultipleAttributes(7, new Dictionary<string, object>
        {
            { "x1", "0%" },
            { "y1", "50%" },
            { "x2", "100%" },
            { "y2", "50%" },
            { "stroke", ColorHelper.ConvertToString(Color) },
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
