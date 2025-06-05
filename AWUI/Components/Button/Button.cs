using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using AWUI.Events;
using System.Diagnostics.CodeAnalysis;

namespace AWUI.Components;

public class Button : AWComponentBase
{
    /**
     * 小知识：隐式子内容
     * Blazor 会自动将组件标签内的内容（如文本、HTML 或其他组件）赋值给它
     */
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter, NotNull]
    public Func<MouseEventArgs, Task>? OnClick { get; set; }

    [Parameter]
    public Action<MouseEventArgs>? OnMouseEnter { get; set; }

    [Parameter]
    public Action<MouseEventArgs>? OnMouseLeave { get; set; }

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

        builder.AddAttribute(7, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async(args) =>
        {
            await HandleClick(args);
        }));

        builder.AddAttribute(8, "onmouseenter", OnMouseEnter);
        builder.AddAttribute(9, "onmouseleave", OnMouseLeave);
        builder.AddContent(10, ChildContent);

        builder.CloseElement();
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        if (OnClick is not null)
        {
            await OnClick.Invoke(args);
        }

        EventBus.Publish<ButtonClickedEvent>(new(this.Id.ToString()));
    }
}
