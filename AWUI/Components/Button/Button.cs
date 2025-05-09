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

    protected sealed override string BaseCssClass => "aw-btn";

    protected virtual string ButtonClass => BuildCssClass();

    protected virtual string ButtonStyle => BuildStyle();

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "button");


        builder.AddMultipleAttributes(seq++, SafeAttributes);
        builder.AddAttribute(seq++, "class", ButtonClass);
        builder.AddAttribute(seq++, "style", ButtonStyle);
        builder.AddAttribute(seq++, "role", "button");

        if (Disabled)
        {
            builder.AddAttribute(seq++, "aria-disabled", "true");
            builder.AddAttribute(seq++, "disabled", "true");
        }

        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async(args) =>
        {
            await HandleClick(args);
        }));

        builder.AddAttribute(seq++, "onmouseenter", OnMouseEnter);
        builder.AddAttribute(seq++, "onmouseleave", OnMouseLeave);
        RenderFilteredAttributes(builder, seq++);

        builder.AddContent(seq, ChildContent);
        builder.CloseElement();
    }

    protected override bool IsAttributeAllowed(string attributeName)
    {
        // 允许 aria-* 和无障碍属性
        if (attributeName.StartsWith("aria-", StringComparison.OrdinalIgnoreCase) ||
            attributeName.Equals("role", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        // 允许父类允许的属性和以下按钮专用属性
        return base.IsAttributeAllowed(attributeName) ||
               attributeName.Equals("type", StringComparison.OrdinalIgnoreCase) ||
               attributeName.Equals("autofocus", StringComparison.OrdinalIgnoreCase);
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        RequestRenderOnNextEvent();

        if (OnClick is not null)
        {
            await OnClick.Invoke(args);
        }
        EventBus.Publish<ButtonClickedEvent>(new(this.ObjectId.ToString()));
    }
}
