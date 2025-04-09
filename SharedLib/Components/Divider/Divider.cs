using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

/// <summary>
/// 分隔器
/// </summary>
public sealed class Divider : AWComponentBase
{
    [CascadingParameter]
    public string? CascadingStyle { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Style ??= CascadingStyle ??= "display: block; height: 30px;";
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style", Style);

        if (AdditionalAttributes is not null)
        {
            builder.AddMultipleAttributes(seq++, AdditionalAttributes);
        }

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
        builder.AddAttribute(seq++, "stroke", "green");
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
