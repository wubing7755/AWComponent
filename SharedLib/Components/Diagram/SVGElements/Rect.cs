using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

public class Rect : DraggableSVGElement
{
    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (IsDeleted) return;

        DiagramService.Add(this);

        int seq = 25;

        builder.OpenElement(seq, "rect");

        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            { "x", X },
            { "y", Y },
            { "width", Width},
            { "height", Height },
            { "fill", Fill }
        });

        // add element reference
        builder.AddElementReferenceCapture(seq, async capturedRef =>
        {
            inputElement = capturedRef;
        });

        builder.CloseElement();
    }
}
