using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Components;

public class Rect : DraggableSvgElement
{
    public double Width { get; set; } = 20;

    public double Height { get; set; } = 20;

    public string Fill { get; set; } = "red";

    public override void Render(RenderTreeBuilder builder)
    {
        int seq = 0;
        builder.OpenElement(seq++, "rect");
        builder.AddAttribute(seq++, "x", X);
        builder.AddAttribute(seq++, "y", Y);
        builder.AddAttribute(seq++, "width", Width);
        builder.AddAttribute(seq++, "height", Height);
        builder.AddAttribute(seq++, "fill", Fill);
        builder.AddElementReferenceCapture(seq, eRef => elementRef = eRef);
        builder.CloseElement();
    }
}
