using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Events;
using Microsoft.AspNetCore.Components.Web;

namespace AWUI.Components;

public class Button : AWComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public EventCallback OnMouseLeave { get; set; }

    [Parameter]
    public string FontColor { get; set; } = "black";

    protected sealed override string BaseClass => "aw-btn";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        CssVariables["--btn-fontColor"] = FontColor;

        builder.OpenElement(0, "button");
        
        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", ComputedStyle);
        builder.AddAttribute(3, "role", "button");

        if (IsDisabled)
        {
            builder.AddAttribute(4, "aria-disabled", "true");
            builder.AddAttribute(5, "disabled");
        }

        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async _ =>
        {
            await HandleClick();
        }));

        builder.AddAttribute(5, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, async _ =>
        {
            await HandleMouseEnter();
        }));

        builder.AddContent(6, ChildContent);

        builder.CloseElement();
    }

    private async Task HandleClick()
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }

#if DEBUG
        EventBus.Publish<ButtonClickedEvent>(new(this.Id.ToString()));
#endif
    }

    private async Task HandleMouseEnter()
    {
        if(OnMouseLeave.HasDelegate)
        {
            await OnMouseLeave.InvokeAsync();
        }
    }
}
