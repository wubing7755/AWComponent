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

    protected sealed override string BaseClass => "aw-btn";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "button");
        
        builder.AddMultipleAttributes(1, SafeAttributes);
        builder.AddAttribute(2, "class", ComputedClass);
        builder.AddAttribute(3, "style", ComputedStyle);
        builder.AddAttribute(4, "role", "button");

        if (IsDisabled)
        {
            builder.AddAttribute(5, "aria-disabled", "true");
            builder.AddAttribute(6, "disabled");
        }

        builder.AddAttribute(7, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async _ =>
        {
            await HandleClick();
        }));

        builder.AddAttribute(8, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, async _ =>
        {
            await HandleMouseEnter();
        }));

        builder.AddContent(9, ChildContent);

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
