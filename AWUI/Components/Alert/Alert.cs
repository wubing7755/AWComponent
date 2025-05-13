using AWUI.Enums;
using AWUI.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace AWUI.Components;

public class Alert : AWComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task>? OnClose { get; set; }

    [Parameter]
    public bool IsVisible { get; set; } = false;

    [Parameter]
    public ColorType Type { get; set; } = ColorType.Green;

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (!IsVisible) return;

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "aw-alert");
        builder.AddAttribute(2, "style", $"background-color: {ColorHelper.ConvertToString(Type)}");
        builder.AddAttribute(3, "role", "alert");

        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async () => {
            if (OnClose is not null)
            {
                RequestRenderOnNextEvent();
                await OnClose.Invoke();
            }
        }));

        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
