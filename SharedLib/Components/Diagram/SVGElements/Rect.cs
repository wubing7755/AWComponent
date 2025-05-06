using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

public class Rect : DraggableSVGElement
{
    [CascadingParameter]
    public Diagram Diagram { get; set; }

    [Parameter]
    public string Fill { get; set; } = "red";

    protected override void OnInitialized()
    {
        Diagram.AddSVGElement(this);
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (IsDeleted) return;

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
