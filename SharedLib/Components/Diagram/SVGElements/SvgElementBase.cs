using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace SharedLibrary.Components;

public abstract class SvgElementBase : AWComponentBase
{
    /// <summary>
    /// 元素引用
    /// </summary>
    protected ElementReference elementRef;

    /// <summary>
    /// dotnet引用
    /// </summary>
    protected DotNetObjectReference<SvgElementBase> DotNetRef;

    /// <summary>
    /// 删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 选中
    /// </summary>
    public bool IsSelected { get; set; } = false;

    /// <summary>
    /// 复制
    /// </summary>
    public bool IsCopyed { get; set; } = false;

    public abstract void Render(RenderTreeBuilder builder);
    protected virtual ValueTask InitializeElementAsync() => ValueTask.CompletedTask;

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        if (IsDeleted) return;

        Render(builder);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            DotNetRef = DotNetObjectReference.Create(this);
            await InitializeElementAsync();
        }
    }

    protected override void DisposeManagedResources()
    {
        DotNetRef?.Dispose();
    }

    [JSInvokable]
    public virtual async Task SelectedElement()
    {
        IsSelected = true;

        await Task.CompletedTask;
    }

    [JSInvokable]
    public virtual async Task UnSelectedElement()
    {
        IsSelected = false;

        await Task.CompletedTask;
    }
}
