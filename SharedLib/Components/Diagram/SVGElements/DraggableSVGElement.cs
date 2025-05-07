using Microsoft.JSInterop;

namespace SharedLibrary.Components;

public abstract class DraggableSvgElement : SvgElementBase
{
    public double X { get; set; }

    public double Y { get; set; }

    [JSInvokable]
    public virtual async Task UpdatePosition(double x, double y)
    {
        X = x;
        // 采用笛卡尔坐标系，与SVG坐标Y轴相反
        Y = -y;

        StateHasChanged();

        await Task.CompletedTask;
    }

    protected override async ValueTask InitializeElementAsync()
    {
        await JsInterop.InitializeDraggableSVGElement(elementRef, DotNetRef, X, Y);
    }
}

