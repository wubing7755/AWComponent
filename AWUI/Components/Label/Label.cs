using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Enums;
using AWUI.Events;
using AWUI.Helper;

namespace AWUI.Components;

public class Label : AWComponentBase
{
    /**
     *  Set the priority of ChildContent higher than that of Text. 
     */
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public ColorType ColorType { get; set; }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", $"color: {ColorHelper.ConvertToString(ColorType)};" + ComputedStyle);
        builder.AddAttribute(3, "role", "label");

        if (ChildContent is not null)
        {
            builder.AddContent(4, ChildContent);
        }
        else
        {
            builder.AddContent(4, Text);
        }

        builder.CloseElement();
    }

#if DEBUG

    private Action? _deb = null;

    private string _btnText = string.Empty;
    private string _inputText = string.Empty;

    protected override void OnInitialized()
    {
        EventBus.Subscribe<ButtonClickedEvent>(e =>
        {
            MarkForRenderOnNextEvent();
            HandleButtonClick(e);
        });

        EventBus.Subscribe<InputKeyEvent>(e =>
        {
            MarkForRenderOnNextEvent();
            HandleInputKey(e);
        });

        _deb = Debounce(() =>
        {
            InvokeAsync(() =>
            {
                if (!string.IsNullOrEmpty(_inputText))
                {
                    Text = _inputText;
                }
                else
                {
                    Text = _btnText;
                }

                MarkForRenderOnNextEvent();

                StateHasChanged();

                EventBus.Unsubscribe<ButtonClickedEvent>(HandleButtonClick);
                EventBus.Unsubscribe<InputKeyEvent>(HandleInputKey);
            });
        }, 3000);
    }

    private void HandleButtonClick(ButtonClickedEvent e)
    {
        _btnText = $"{e.ButtonId} - {e.ClickTime}";

        _deb?.Invoke();
    }

    private void HandleInputKey(InputKeyEvent e)
    {
        _inputText = $"{e.Id} - {e.Key}";

        _deb?.Invoke();
    }

#endif
}
