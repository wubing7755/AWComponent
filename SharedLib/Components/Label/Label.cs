using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharedLibrary.Enums;
using SharedLibrary.Events;

namespace SharedLibrary.Components;

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

    protected override void OnInitialized()
    {
        EventBus.Subscribe<ButtonClickedEvent>(HandleButtonClick);

        _deb = Debounce(() =>
        {
            InvokeAsync(() =>
            {
                if(ChildContent is not null)
                {
                    ChildContent = builder =>
                    {
                        builder.AddContent(0, $"{_time} - {_id}");
                    };
                }
                else
                {
                    Text = $"{_time} - {_id}";
                }

                /*
                    取消订阅，只有第一次单击按钮才会触发事件
                 */
                EventBus.Unsubscribe<ButtonClickedEvent>(HandleButtonClick);
                StateHasChanged();
            });
        }, 3000);
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;
        builder.OpenElement(seq++, "label");

        var colorStyle = ColorType switch
        {
            ColorType.Blue => "Blue",
            ColorType.Red => "Red",
            ColorType.Green => "Green",
            ColorType.Yellow => "Yellow",
            ColorType.Orange => "Orange",
            ColorType.Purple => "Purple",
            ColorType.Black => "Black",
            ColorType.White => "White",
            ColorType.Gray => "Gray",
            ColorType.Brown => "Brown",
            ColorType.Cyan => "Cyan",
            ColorType.Magenta => "Magenta",
            _ => "Black" // Default color
        };

        builder.AddAttribute(seq++, "style", $"color: {colorStyle};");

        if (ChildContent is not null)
        {
            builder.AddContent(seq, ChildContent);
        }
        else
        {
            builder.AddContent(seq, Text);
        }

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
