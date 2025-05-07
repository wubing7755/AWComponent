using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Components;

public class Rect : DraggableSvgElement
{
    public double Width { get; set; } = 20;

    public double Height { get; set; } = 20;

    public string Fill { get; set; } = "red";

    [Inject]
    public IDiagramService DiagramService { get; set; } = null!;

    public override void Render(RenderTreeBuilder builder)
    {
        if(!DiagramService.Contains(this))
        {
            DiagramService.Add(this);
        }

        int seq = 25;
        builder.OpenElement(seq++, "rect");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            { "x", X },
            { "y", Y },
            { "width", Width},
            { "height", Height },
            { "fill", Fill }
        });
        builder.AddElementReferenceCapture(seq, eRef => elementRef = eRef);
        builder.CloseElement();
    }
}
