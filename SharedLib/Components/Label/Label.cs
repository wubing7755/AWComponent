

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharedLibrary.Events;

namespace SharedLibrary.Components;

public class Label : AWComponentBase
{
    [Parameter]
    public string? Text { get; set; }

    protected override void OnInitialized()
    {
        EventBus.Subscribe<ButtonClickedEvent>(HandleButtonClick);

        _deb = Debounce(() =>
        {
            InvokeAsync(() =>
            {
                Text = $"{_time} - {_id}";
                StateHasChanged();
            });
        }, 3000);
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;
        builder.OpenElement(seq++, "label");
        builder.AddContent(seq++, Text);
        builder.CloseElement();
    }

    private Action _deb;

    private DateTime _time;
    private string _id;

    private void HandleButtonClick(ButtonClickedEvent e)
    {
        _time = e.ClickTime;
        _id = e.ButtonId!;

        _deb?.Invoke();
    }
}
