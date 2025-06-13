using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Enums;
using Microsoft.AspNetCore.Components.Web;
using AWUI.Helper;

namespace AWUI.Components;

public class Alert : AWComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public ColorType Type { get; set; } = ColorType.Green;

    protected sealed override string BaseClass => "aw-alert";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", $"background-color: {ColorHelper.ConvertToString(Type)}" + ComputedStyle);
        builder.AddAttribute(3, "role", "alert");

        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async _ => {
            await HandleClose();
        }));

        if(ChildContent is not null)
        {
            builder.AddContent(5, ChildContent);
        }

        builder.CloseElement();
    }

    private async Task HandleClose()
    {
        if(OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
        }
    }
}
