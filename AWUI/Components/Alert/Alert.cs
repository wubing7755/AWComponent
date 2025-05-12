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

        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "aw-alert");
        builder.AddAttribute(seq++, "style", $"background-color: {ColorHelper.ConvertToString(Type)}");
        builder.AddAttribute(seq++, "role", "alert");

        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async () => {
            if (OnClose is not null)
            {
                RequestRenderOnNextEvent();
                await OnClose.Invoke();
            }
        }));

        builder.AddContent(seq, ChildContent);
        builder.CloseElement();
    }
}
