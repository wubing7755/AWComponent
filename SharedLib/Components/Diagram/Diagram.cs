using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components.Diagram;

public class Diagram : AWComponentBase
{
    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq, "svg");
        builder.CloseElement();
    }
}
