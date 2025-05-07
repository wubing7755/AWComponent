using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Components;

/**
 * 两种方案：① 在ChildContent中使用SVG元素，② 在Elements中添加SVG元素并渲染
 */
public abstract class DraggableSVGElement : AWComponentBase
{
    protected bool _jsInteropInitialized;
    protected ElementReference inputElement;
    protected DotNetObjectReference<DraggableSVGElement>? DotNetHelper;

    [CascadingParameter]
    public Diagram Diagram { get; set; }

    [Parameter]
    public double X { get; set; }

    [Parameter] 
    public double Y { get; set; }

    [Parameter]
    public double Width { get; set; }

    [Parameter]
    public double Height { get; set; }

    [Parameter]
    public string Fill { get; set; } = "red";

    public object? Data { get; set; }

    [Inject]
    public IDiagramService DiagramService { get; set; } = null!;

    /// <summary>
    /// 选中
    /// </summary>
    public bool IsSelected { get; set; } = false;

    /// <summary>
    /// 删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 复制
    /// </summary>
    public bool IsCopyed { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_jsInteropInitialized && DotNetHelper is null)
        {
            Console.WriteLine($"id: {inputElement.Id} -- ctx is null: {inputElement.Context is null}");
            DotNetHelper = DotNetObjectReference.Create(this);
            try
            {

                await JsInterop.InitializeDraggableSVGElement(inputElement, DotNetHelper, X, Y);
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
                await JsInterop.CleanUpDraggableSVGElement(inputElement);
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

        StateHasChanged();

        await Task.CompletedTask;
    }

    [JSInvokable]
    public virtual async Task SelectedElement()
    {
        this.IsSelected = true;

        await Task.CompletedTask;
    }

    [JSInvokable]
    public virtual async Task UnSelectedElement()
    {
        this.IsSelected = false;

        await Task.CompletedTask;
    }

}
