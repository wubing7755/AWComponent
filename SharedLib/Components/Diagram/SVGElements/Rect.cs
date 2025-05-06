using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

public class Rect : DraggableSVGElement
{
    [Parameter]
    public string Fill { get; set; } = "red";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq, "rect");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            { "x", X },
            { "y", Y },
            { "width", Width },
            { "height", Height },
            { "fill", Fill } 
        });

        // add element reference
        builder.AddElementReferenceCapture(seq, capturedRef =>
        {
            inputElement = capturedRef;
        });

        builder.CloseElement();
    }
}
