using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedLibrary.Models;

namespace SharedLibrary.Components;

public abstract class DraggableSVGElement : AWComponentBase
{
    private bool _jsInteropInitialized;
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

    public bool IsSelected { get; set; } = false;

    public bool IsDeleted { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DotNetHelper = DotNetObjectReference.Create(this);
            try
            {
                await JsInterop.CreateSVGDragController(inputElement, DotNetHelper, X, Y);
                _jsInteropInitialized = true;
            }
            catch
            {
                DotNetHelper?.Dispose();
                DotNetHelper = null;
                throw;
            }
        }
    }

    protected override void DisposeManagedResources()
    {
        if (DotNetHelper is not null)
        {
            DotNetHelper.Dispose();
            DotNetHelper = null;
        }
    }

    protected override async ValueTask DisposeManagedResourcesAsync()
    {
        if (DotNetHelper is not null)
        {
            if (_jsInteropInitialized)
            {
                await JsInterop.DisposeSVGDragController(inputElement);
            }
            DotNetHelper.Dispose();
            DotNetHelper = null;
        }
    }

    [JSInvokable]
    public virtual async Task UpdatePosition(double x, double y)
    {
        X = x;
        // 采用笛卡尔坐标系，与SVG坐标Y轴相反
        Y = -y;

        if (OnPositionChanged.HasDelegate)
        {
            await OnPositionChanged.InvokeAsync(new Point(X, Y));
        }

        IsSelected = true;
        StateHasChanged();
    }
}
