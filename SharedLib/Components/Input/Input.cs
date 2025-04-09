using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

public class Input : AWComponentBase
{


    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "div");

        builder.OpenElement(seq++, "input");

        builder.CloseElement();

        builder.CloseElement();
    }
}
