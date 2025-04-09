using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SharedLibrary.Components;

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

    protected sealed override string BaseCssClass => "aw-select";

    protected virtual string SelectClass => BuildCssClass();

    protected virtual string SelectStyle => BuildStyle();

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
        int seq = 0;

        builder.OpenElement(seq++, "div");

        builder.OpenElement(seq++, "select");

        builder.AddMultipleAttributes(seq++, SafeAttributes);
        builder.AddAttribute(seq++, "class", SelectClass);
        builder.AddAttribute(seq++, "style", SelectStyle);

        builder.AddAttribute(seq++, "value", Value);
        builder.AddAttribute(seq++, "onchange", OnValueChanged);

        builder.OpenElement(seq++, "option");
        builder.AddAttribute(seq++, "value", "");
        builder.AddAttribute(seq++, "selected", string.IsNullOrEmpty(Value));
        builder.AddContent(seq++, Placeholder);
        builder.CloseElement();

        if (Options.Any())
        {
            foreach (var option in Options)
            {
                builder.OpenElement(seq++, "option");
                builder.AddAttribute(seq++, "value", option);
                builder.AddAttribute(seq++, "selected", option == Value);

                if (OptionTemplate is not null)
                {
                    builder.AddContent(seq++, OptionTemplate(option));
                }
                else
                {
                    builder.AddContent(seq++, option);
                }

                builder.CloseElement();
            }
        }

        builder.CloseElement();


        builder.CloseElement();

    }
}
