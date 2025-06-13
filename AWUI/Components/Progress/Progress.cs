using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AWUI.Components;

public class Progress : AWComponentBase
{
    [Parameter]
    public float Value { get; set; } = 0.0f;

    [Parameter]
    public bool ShowValue { get; set; } = true;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter] 
    public string ColorFrom { get; set; } = "#4facfe";

    [Parameter] 
    public string ColorTo { get; set; } = "#00f2fe";

    public float Min { get; set; } = 0.0f;
    public float Max { get; set; } = 100.0f;

    protected sealed override string BaseClass => "aw-progress";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        var percentage = Math.Clamp((Value - Min) / (Max - Min) * 100, 0, 100);
        CssVariables.Clear();
        CssVariables["--progress-width"] = percentage + "%";
        CssVariables["--progress-colorFrom"] = ColorFrom;
        CssVariables["--progress-colorTo"] = ColorTo;

        builder.OpenElement(0, "div");

        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", ComputedStyle);
        builder.AddAttribute(3, "role", "progressbar");
        builder.AddAttribute(4, "aria-valuenow", Value.ToString());
        builder.AddAttribute(5, "aria-valuemin", Min.ToString());
        builder.AddAttribute(6, "aria-valuemax", Max.ToString());

        if (ChildContent is not null)
        {
            builder.AddContent(7, ChildContent);
        }
        else if(ShowValue)
        {
            builder.OpenElement(7, "span");
            builder.AddContent(8, $"{percentage:F0}%");
            builder.CloseElement();
        }

        builder.CloseElement();
    }
}
