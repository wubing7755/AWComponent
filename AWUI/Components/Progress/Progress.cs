using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AWUI.Components;

public class Progress : AWComponentBase
{
    [Parameter]
    public float Value { get; set; } = 0.0f;

    [Parameter]
    public bool ShowValue { get; set; } = true;

    private string _text = string.Empty;

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, SafeAttributes);
        builder.AddContent(2, Value);

        builder.CloseElement();
    }
}
