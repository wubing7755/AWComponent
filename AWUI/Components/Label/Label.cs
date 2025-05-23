﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Enums;
using AWUI.Events;
using AWUI.Utils;

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

    protected override void OnInitialized()
    {
        EventBus.Subscribe<ButtonClickedEvent>(e =>
        {
            RequestRenderOnNextEvent();
            HandleButtonClick(e);
        });

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

                RequestRenderOnNextEvent();

                /*
                    取消订阅，只有第一次单击按钮才会触发事件
                 */
                EventBus.Unsubscribe<ButtonClickedEvent>(HandleButtonClick);
            });
        }, 3000);
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "style", $"color: {ColorHelper.ConvertToString(ColorType)};");
        RenderFilteredAttributes(builder, 2);

        if (ChildContent is not null)
        {
            builder.AddContent(3, ChildContent);
        }
        else
        {
            builder.AddContent(4, Text);
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
