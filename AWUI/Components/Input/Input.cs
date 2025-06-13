using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using AWUI.Enums;
using AWUI.Events;
using AWUI.Helper;

namespace AWUI.Components;

public class Input<TValue> : AWComponentBase
{
    protected ElementReference inputElement;

    protected TValue? currentValue;

    [Parameter]
    public InputType Type { get; set; } = InputType.Text;

    [Parameter]
    public AutocompleteType Autocomplete { get; set; } = AutocompleteType.Off;

    [Parameter]
    public bool AutoFocus { get; set;} = false;

    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<TValue> OnBlur { get; set; }

    [Parameter]
    public EventCallback<TValue> OnEnter { get; set; }

    [Parameter]
    public EventCallback<ElementReference> InputRefChanged { get; set; }

    protected override Task OnParametersSetAsync()
    {
        currentValue = Value;
        return base.OnParametersSetAsync();
    } 

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (AutoFocus)
        {
            await inputElement.FocusAsync();
        }
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "class", ComputedClass);
        builder.AddAttribute(2, "style", ComputedStyle);
        builder.AddAttribute(3, "role", "input");

        BuildComponentAttributes(builder);

        // add element reference
        builder.AddElementReferenceCapture(11, async capturedRef =>
        {
            inputElement = capturedRef;

            if (InputRefChanged.HasDelegate)
            {
                await InputRefChanged.InvokeAsync(capturedRef);
            }
        });


        builder.CloseElement();
    }

    protected virtual void BuildComponentAttributes(RenderTreeBuilder builder)
    {
        builder.AddAttribute(4, "type", Type);
        builder.AddAttribute(5, "value", BindConverter.FormatValue(currentValue));

        builder.AddAttribute(6, "oninput", EventCallback.Factory.CreateBinder<TValue>(
                    this, __value => currentValue = __value, currentValue ?? default(TValue)!));

        builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, async _ =>
        {
            await HandleBlur(currentValue);
        }));

        builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, async args =>
        {
            await HandleEnter(currentValue, args);
        }));

        builder.AddAttribute(9, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, async _ =>
        {
            await ValueChanged.InvokeAsync(currentValue);
        }));

        builder.AddAttribute(10, "autocomplete", AutocompleteFactory.GetAutocomplete(Autocomplete));
    }

    private async Task HandleBlur(TValue? value)
    {
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync(value ?? default(TValue)!);
        }
    }

    private async Task HandleEnter(TValue? value, KeyboardEventArgs args)
    {
        switch(args.Key)
        {
            case "Enter":
                if (OnEnter.HasDelegate)
                {
                    await OnEnter.InvokeAsync(value ?? default(TValue)!);
                }
                break;
            default:
#if DEBUG
                EventBus.Publish<InputKeyEvent> (new(this.Id.ToString(), args.Key));
#endif
                break;
        }
    }
}
