using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

public class Input : AWComponentBase
{
    [Parameter]
    public InputType Type { get; set; }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "div");

        builder.OpenElement(seq++, "input");
        builder.AddAttribute(seq++, "type", Type);
        builder.CloseElement();

        builder.CloseElement();
    }
}

public enum InputType
{
    Button,
    Checkbox,
    Color,
    Date,
    DatetimeLocal,
    Email,
    File,
    Hidden,
    Image,
    Month,
    Number,
    Password,
    Radio,
    Range,
    Reset,
    Search,
    Submit,
    Tel,
    Text,
    Time,
    Url,
    Week
}
