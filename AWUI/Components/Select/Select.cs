﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AWUI.Components;

public class Select : AWComponentBase
{
    [Parameter, NotNull, EditorRequired]
    public IEnumerable<string>? Options { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string>? ValueChanged { get; set; }

    [Parameter]
    public string? Placeholder { get; set; } = "Please choose an option";

    [Parameter]
    public RenderFragment<string>? OptionTemplate { get; set; }

    protected sealed override string BaseClass => "aw-select";

    private void OnValueChanged(ChangeEventArgs args)
    {
        var value = args.Value?.ToString();
        // re assignment
        Value = value;
        if (ValueChanged.HasValue)
        {
            ValueChanged.Value.InvokeAsync(value);
        }
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "select");

        builder.AddAttribute(2, "class", ComputedClass);
        builder.AddAttribute(3, "style", ComputedStyle);
        builder.AddAttribute(4, "role", "select");

        builder.AddAttribute(5, "value", Value);
        builder.AddAttribute(6, "onchange", OnValueChanged);

        builder.OpenElement(7, "option");
        builder.AddAttribute(8, "value", "");
        builder.AddAttribute(9, "selected", string.IsNullOrEmpty(Value));

        builder.AddContent(10, Placeholder);
        builder.CloseElement();

        if (Options.Any())
        {
            int seq = 11;
            foreach (var option in Options)
            {
                builder.OpenRegion(seq);
                builder.OpenElement(0, "option");
                builder.AddAttribute(1, "value", option);
                builder.AddAttribute(2, "selected", option == Value);

                if (OptionTemplate is not null)
                {
                    builder.AddContent(3, OptionTemplate(option));
                }
                else
                {
                    builder.AddContent(5, option);
                }

                builder.CloseElement();
                builder.CloseRegion();

                seq++;
            }
        }

        builder.CloseElement();

        builder.CloseElement();

    }
}
