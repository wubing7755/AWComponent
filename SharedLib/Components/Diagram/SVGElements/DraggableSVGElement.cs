using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedLibrary.Interfaces;
using SharedLibrary.Services;

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
    }

    protected override async ValueTask InitializeElementAsync()
    {
        await JsInterop.InitializeDraggableSVGElement(elementRef, DotNetRef, X, Y);
    }
}

