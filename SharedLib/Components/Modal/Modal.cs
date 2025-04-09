using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace SharedLibrary.Components;

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

    [Parameter, NotNull, EditorRequired]
    public Func<Task>? OnClose { get; set; }

    private bool IsVisible { get; set; } = false;

    protected sealed override string BaseCssClass => "aw-modal";

    protected virtual string ModalClass => BuildCssClass();

    protected virtual string ModalStyle => BuildStyle();

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (!IsVisible) return;

        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style",
            "position: fixed;" +
            "top: 0;" +
            "left: 0;" +
            "width: 100vw;" +
            "height: 100vh;" +
            "display: flex;" +
            "justify-content: center;" +
            "align-items: center;" +
            "z-index: 1000;");
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style",
            "width: 50vw;" +
            "height: 50vh;" +
            "background: #282c34;" +
            "display: flex;" +
            "flex-direction: column;" +
            "border-radius: 8px;");
        builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        // Header
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style",
            "height: 15%;" +
            "display: flex;" +
            "justify-content: flex-start;" +
            "align-items: center;" +
            "padding: 0 1.5rem;" +
            "gap: 1rem;" +
            "border-bottom: 1px solid #3d434d;");
        builder.AddContent(seq++, HeaderTemplate);
        builder.CloseElement();

        // Body
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style",
            "height: 70%;" +
            "overflow-y: auto;" +
            "min-height: 100px;" +
            "padding: 1.5rem;" +
            "color: white;" +
            "flex-grow: 1;");
        builder.AddAttribute(seq++, "class", CssClass);
        builder.AddMultipleAttributes(seq++, AdditionalAttributes);
        builder.AddContent(seq++, BodyContent);
        builder.CloseElement();

        // Footer
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style",
            "height: 15%;" +
            "display: flex;" +
            "justify-content: flex-end;" +
            "align-items: center;" +
            "padding: 0 1.5rem;" +
            "gap: 1rem;" +
            "border-top: 1px solid #3d434d;");
        builder.AddContent(seq++, FooterTemplate);

        // Close Button
        builder.OpenElement(seq++, "button");
        builder.AddAttribute(seq++, "type", "button");
        builder.AddAttribute(seq++, "style",
            "background: #61afef;" +
            "cursor: pointer;" +
            "margin-right: 20px;" +
            "margin-top: 2px;" +
            "height: 35px;" +
            "width: 110px;" +
            "display: float;");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, OnCloseClick));
        builder.AddContent(seq, "Close Dialog");
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
            await OnClose();
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        switch (arg.Key)
        {
            case "escape":
                await OnClose();
                break;
        }
    }
}