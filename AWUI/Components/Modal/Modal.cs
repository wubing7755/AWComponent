using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace AWUI.Components;

public class ModalDialog : AWComponentBase
{
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public RenderFragment? BodyContent { get; set; }

    [Parameter]
    public bool ShowConfirmButton { get; set; } = true;

    [Parameter]
    public bool ShowCancelButton { get; set; } = true;

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter, EditorRequired]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnConfirm { get; set; }

    private bool IsVisible { get; set; } = false;

    protected sealed override string BaseCssClass => "aw-modal";

    protected virtual string ModalClass => BuildCssClass();

    protected virtual string ModalStyle => BuildStyle();

    private readonly string _btnStyle = 
            "background: #61afef;" +
            "cursor: pointer;" +
            "margin-right: 20px;" +
            "margin-top: 2px;" +
            "height: 35px;" +
            "width: 110px;" +
            "display: float;";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (!IsVisible) return;

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "style",
            "position: fixed;" +
            "top: 0;" +
            "left: 0;" +
            "width: 100vw;" +
            "height: 100vh;" +
            "display: flex;" +
            "justify-content: center;" +
            "align-items: center;" +
            "z-index: 1000;");
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "style",
            "width: 50vw;" +
            "height: 50vh;" +
            "background: #282c34;" +
            "display: flex;" +
            "flex-direction: column;" +
            "border-radius: 8px;");
        builder.AddAttribute(4, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        // Header
        builder.OpenElement(5, "div");
        builder.AddAttribute(6, "style",
            "height: 15%;" +
            "display: flex;" +
            "justify-content: flex-start;" +
            "align-items: center;" +
            "padding: 0 1.5rem;" +
            "gap: 1rem;" +
            "border-bottom: 1px solid #3d434d;");
        builder.AddContent(7, HeaderTemplate);
        builder.CloseElement();

        // Body
        builder.OpenElement(8, "div");
        builder.AddAttribute(9, "style",
            "height: 70%;" +
            "overflow-y: auto;" +
            "min-height: 100px;" +
            "padding: 1.5rem;" +
            "color: white;" +
            "flex-grow: 1;");
        builder.AddAttribute(10, "class", CssClass);
        builder.AddMultipleAttributes(11, AdditionalAttributes);
        builder.AddContent(12, BodyContent);
        builder.CloseElement();

        // Footer
        builder.OpenElement(13, "div");
        builder.AddAttribute(14, "style",
            "height: 15%;" +
            "display: flex;" +
            "justify-content: flex-end;" +
            "align-items: center;" +
            "padding: 0 1.5rem;" +
            "gap: 1rem;" +
            "border-top: 1px solid #3d434d;");
        builder.AddContent(15, FooterTemplate);

        // Close Button
        builder.OpenElement(16, "button");
        builder.AddAttribute(17, "type", "button");
        builder.AddAttribute(18, "style", _btnStyle);
        builder.AddAttribute(19, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, OnCloseClick));
        builder.AddContent(20, "Close");
        builder.CloseElement();

        // Confirm Button
        builder.OpenElement(21, "button");
        builder.AddAttribute(22, "type", "button");
        builder.AddAttribute(23, "style", _btnStyle);
        builder.AddAttribute(24, "onclick", EventCallback.Factory.Create(this, OnConfirmClick));
        builder.AddContent(25, "Confirm");
        builder.CloseElement();

        builder.CloseElement();

        builder.CloseElement();
        builder.CloseElement();
    }

    public void SetVisible(bool isVisible)
    {
        IsVisible = isVisible;
        StateHasChanged();
    }

    private async Task OnCloseClick(MouseEventArgs args)
    {
        if (ShowCloseButton)
        {
            await OnClose.InvokeAsync();
        }
    }

    private async Task OnConfirmClick()
    {
        if (OnConfirm.HasDelegate)
        {
            await OnConfirm.InvokeAsync("confirmed");
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        switch (arg.Key)
        {
            case "escape":
                await OnClose.InvokeAsync();
                break;
        }
    }
}