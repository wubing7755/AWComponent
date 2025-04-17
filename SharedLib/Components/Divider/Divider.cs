using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

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
        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddMultipleAttributes(seq++, SafeAttributes);
        builder.AddAttribute(seq++, "class", DividerClass);
        builder.AddAttribute(seq++, "style", DividerStyle);

        // SVG Dotted
        builder.OpenElement(seq++, "svg");
        builder.AddAttribute(seq++, "style",
            "width: 100%;" +
            "height: 30%;" +
            "aria-hidden: true;");
        builder.OpenElement(seq++, "line");
        builder.AddAttribute(seq++, "x1", "0%");
        builder.AddAttribute(seq++, "y1", "50%");
        builder.AddAttribute(seq++, "x2", "100%");
        builder.AddAttribute(seq++, "y2", "50%");
        builder.AddAttribute(seq++, "stroke", "black");
        builder.AddAttribute(seq++, "stroke-width", "1");
        builder.AddAttribute(seq++, "stroke-dasharray", "5 3");
        builder.CloseElement();
        builder.CloseElement();

        if (ChildContent != null)
        {
            builder.AddMarkupContent(seq++, "<div style=\"text-align:center;margin-top:-15px;\">");
            builder.OpenElement(seq++, "span");
            builder.AddContent(seq, ChildContent);
            builder.CloseElement();
            builder.AddMarkupContent(seq++, "</div>");
        }

        builder.CloseElement();
    }
}
