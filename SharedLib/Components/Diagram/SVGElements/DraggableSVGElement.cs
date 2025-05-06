using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedLibrary.Models;

namespace SharedLibrary.Components;

public abstract class DraggableSVGElement : AWComponentBase
{
    protected ElementReference inputElement;
    protected DotNetObjectReference<DraggableSVGElement>? DotNetHelper;

    [Parameter] 
    public double X { get; set; }

    [Parameter] 
    public double Y { get; set; }

    [Parameter]
    public double Width { get; set; }

    [Parameter]
    public double Height { get; set; }

    [Parameter] 
    public EventCallback<Point> OnPositionChanged { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DotNetHelper = DotNetObjectReference.Create(this);

            if(DotNetHelper is not null)
            {
                await JsInterop.InitializeDraggableSVGElement(inputElement, DotNetHelper, X, Y);
            }
        }
    }

    protected override void DisposeManagedResources()
    {
        DotNetHelper?.Dispose();
    }

    [JSInvokable]
    public virtual async Task UpdatePosition(double x, double y)
    {
        X = x;
        Y = y;

        if (OnPositionChanged.HasDelegate)
        {
            await OnPositionChanged.InvokeAsync(new Point(X, Y));
        }

        StateHasChanged();
    }
}
