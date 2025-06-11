using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using AWUI.JsInterop;
using AWUI.Interfaces;
using AWUI.Helper;

namespace AWUI.Components;

/// <summary>
/// Defines the component hierarchy with core functionality distribution
/// </summary>
/// <remarks>
/// Component inheritance chain:
/// <para>
/// 1. <see cref="SecureComponentBase"/> (Security and localization foundation)
/// 2. <see cref="ResponsiveComponentBase"/> (UI and behavior foundation)
/// 3. <see cref="AWComponentBase"/> (Business logic and form integration)
/// </para>
/// </remarks>
/*

Component Hierarchy Feature Matrix:
================================================================================
| Base Class              | Key Features                                       |
|-------------------------|----------------------------------------------------|
| SecureComponentBase     | - Localization services (Localizer)                |
|                         | - Attribute security filtering (SafeAttributes)    |
|                         | - Dispose management                               |   
|-------------------------|----------------------------------------------------|
| ResponsiveComponentBase | - CSS class composition (CssClass)                 |
|                         | - Inline style handling (Style)                    |
|                         | - JavaScript interoperability (JsInterop)          |
|                         | - Theme management                                 |
|                         | - Disabled state management                        |
|                         | - Component building pipeline (BuildComponent)     |
|-------------------------|----------------------------------------------------|
| AWComponentBase         | - Form validation context (EditContext)            |
|                         | - Cross-component communication (EventBus)         |
|                         | - Debounce                                         |
================================================================================

*/


#region SecureComponentBase

/// <summary>
/// Abstract base class for all components in the system.
/// 提供核心安全功能的基底层
/// </summary>
/// <remarks>
/// Implements core functionality including:
/// - Localization support
/// - Theme propagation
/// - Attribute safety filtering
/// - Resource disposal management
/// </remarks>
public abstract class SecureComponentBase : ComponentBase, IDisposable, IAsyncDisposable, ISecurityPolicy
{
    protected SecureComponentBase()
    {
        Id = Guid.NewGuid();
    }

    private bool _disposed;
    private bool _disposedAsync;

    /// <summary>
    /// Unique identifier for component instance tracking
    /// 组件实例唯一标识符
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the localized string resources for this component.
    /// 国际化文本资源
    /// </summary>
    /// <remarks>
    /// usage: Localizer["bingbing"]
    /// </remarks>
    [Inject, NotNull]
    protected IStringLocalizer<SecureComponentBase>? Localizer { get; set; }

    #region Attributes

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied 
    /// to the created element.
    /// 经过安全过滤的HTML属性
    /// </summary>
    /// <remarks>
    /// This parameter captures all unmatched attribute values passed to the component. 
    /// Use <see cref="SafeAttributes"/> to access filtered attributes.
    /// </remarks>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the safely filtered attributes after applying security rules.
    /// 经过安全过滤的HTML属性
    /// </summary>
    /// <value>
    /// Read-only dictionary containing only allowed attributes based on 
    /// <see cref="IsAttributeAllowed"/> policy.
    /// </value>
    protected virtual IReadOnlyDictionary<string, object>? SafeAttributes
        => ((ISecurityPolicy)this).Filter(AdditionalAttributes);

    /// <inheritdoc />
    IReadOnlyDictionary<string, object>? ISecurityPolicy.Filter(IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes is null) return null;

        var filtered = new Dictionary<string, object>();

        foreach (var (attrK, attrV) in attributes)
        {
            if (((ISecurityPolicy)this).IsAttributeAllowed(attrK, attrV))
            {
                filtered[attrK] = attrV;
            }
        }
        return new ReadOnlyDictionary<string, object>(filtered);
    }

    /// <inheritdoc />
    bool ISecurityPolicy.IsAttributeAllowed(string attributeName, object attributeValue)
    {
        // Security: Block all event handlers to prevent XSS
        // 安全措施：阻止所有事件处理器属性防止XSS

        return !attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase) 
            || attributeName.Equals("class", StringComparison.OrdinalIgnoreCase) 
            || attributeName.Equals("style", StringComparison.OrdinalIgnoreCase) 
            || attributeName.Equals("type", StringComparison.OrdinalIgnoreCase) 
            || attributeName.Equals("autofocus", StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Releases managed resources used by this component.
    /// </summary>
    /// <remarks>
    /// Override this method to dispose any IDisposable resources 
    /// created by your component.
    /// </remarks>
    protected virtual void DisposeManagedResources() { }

    /// <summary>
    /// Releases managed resources used by this component asynchronously.
    /// </summary>
    /// <remarks>
    /// Override this method to dispose any IDisposable/IAsyncDisposable resources 
    /// created by your component that require async disposal.
    /// </remarks>
    protected virtual ValueTask DisposeManagedResourcesAsync() => ValueTask.CompletedTask;

    /// <summary>
    /// Releases unmanaged resources used by this component.
    /// </summary>
    /// <remarks>
    /// Always call base.DisposeUnmanagedResources() when overriding.
    /// </remarks>
    protected virtual void DisposeUnmanagedResources() { }

    /// <summary>
    /// Releases unmanaged resources used by this component asynchronously.
    /// </summary>
    /// <remarks>
    /// Always call base.DisposeUnmanagedResourcesAsync() when overriding.
    /// </remarks>
    protected virtual ValueTask DisposeUnmanagedResourcesAsync() => ValueTask.CompletedTask;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, 
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    /// <remarks>
    /// Execution order:
    /// 1. DisposeManagedResources()
    /// 2. DisposeUnmanagedResources()
    /// 3. GC.SuppressFinalize()
    /// 
    /// 执行顺序：
    /// 1. 释放托管资源
    /// 2. 释放非托管资源
    /// 3. 抑制终结器
    /// </remarks>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            DisposeManagedResources();
        }

        DisposeUnmanagedResources();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposedAsync) return;

#if DEBUG
        Console.WriteLine($"DisposeAsync called for {Id}. Was disposed properly: {_disposedAsync}.");
#endif

        await DisposeManagedResourcesAsync().ConfigureAwait(false);
        await DisposeUnmanagedResourcesAsync().ConfigureAwait(false);

        GC.SuppressFinalize(this);
        _disposedAsync = true;
    }

    /// <summary>
    /// Finalizer ensuring unmanaged resources are released
    /// </summary>
    ~SecureComponentBase()
    {
        Dispose(false);
    }

    #endregion
}

#endregion

#region ResponsiveComponentBase

/// <summary>
/// Base class for presentational UI components with styling support.
/// 具备响应式设计能力的表现层
/// </summary>
/// <remarks>
/// Extends <see cref="SecureComponentBase"/> with:
/// - CSS class composition
/// - Inline style merging
/// - Responsive breakpoint handling
/// </remarks>
public abstract class ResponsiveComponentBase : SecureComponentBase, IHandleEvent
{
    private bool _renderScheduled = false;

    protected virtual string BaseClass => string.Empty;
    protected virtual string BaseStyle => string.Empty;

    /// <summary>
    /// Constructs a combined CSS class string from multiple sources.
    /// </summary>
    /// <param name="baseClass">Base class defined by the component</param>
    /// <returns>
    /// Merged class string containing (in order):
    /// 1. Component base classes
    /// 2. Explicit CssClass parameter
    /// 3. AdditionalAttributes class values
    /// </returns>
    protected string ComputedClass => CssBuilder.Default
            .AddClass(BaseClass)
            .AddClass(Class)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

    /// <summary>
    /// Constructs a combined inline style string from multiple sources.
    /// </summary>
    /// <param name="baseStyle">Base style defined by the component</param>
    /// <returns>
    /// Merged style string containing (in order):
    /// 1. Component base styles
    /// 2. Explicit Style parameter
    /// 3. Breakpoint constraints
    /// 4. AdditionalAttributes style values
    /// </returns>
    protected string ComputedStyle => StyleBuilder.Default
            .AddStyle(BaseStyle)
            .AddStyle(Style)
            .AddStyleFromAttributes(AdditionalAttributes)
            .AddStyle("display:none", IsDisabled)
            .Build();

    /// <summary>
    /// Indicates whether to enable render optimization for event handling.
    /// 开启自定义优化渲染
    /// </summary>
    protected bool EnableRenderOptimization { get; set; } = true;

    /// <summary>
    /// Provides JavaScript interop functionality for the component.
    /// </summary>
    [Inject]
    protected AWJsInterop JsInterop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CSS class string applied to the root element.
    /// </summary>
    /// <remarks>
    /// Will be combined with base classes and additional attribute classes.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style string applied to the root element.
    /// </summary>
    /// <remarks>
    /// Will be merged with base styles and additional attribute styles.
    /// </remarks>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Disabled Component
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; } = false;

    [Parameter]
    public virtual bool IsVisible { get; set; } = true;

    #region Render

    /// <summary>
    /// Template method for component-specific rendering
    /// 组件特定渲染的模板方法
    /// </summary>
    /// <param name="builder">Blazor render tree builder</param>
    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!IsVisible) return;

        BuildComponent(builder);
    }

    protected abstract void BuildComponent(RenderTreeBuilder builder);

    /// <summary>
    /// 优化渲染
    /// </summary>
    /// <param name="item"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? arg)
    {
        // 同步处理，无渲染
        if (TryHandleSynchronous(item, arg, out var task))
        {
            return task;
        }

        // 异步处理，根据条件决定渲染
        return TryHandleAsync(item, arg);
    }

    private bool TryHandleSynchronous(EventCallbackWorkItem item, object? arg, out Task task)
    {
        task = null!;

        // 开启延迟渲染 || 未开启优化渲染
        if (_renderScheduled || !EnableRenderOptimization)
        {
            return false;
        }

        try
        {
            var syncTask = item.InvokeAsync(arg);

            if (syncTask.IsCompletedSuccessfully)
            {
                task = Task.CompletedTask;

                return true;
            }
        }
        catch
        {
            _renderScheduled = true;
            throw;
        }

        return false;
    }

    private async Task TryHandleAsync(EventCallbackWorkItem item, object? arg)
    {
        try
        {
            await item.InvokeAsync(arg).ConfigureAwait(false);
        }
        finally
        {
            if (_renderScheduled || !EnableRenderOptimization)
            {
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }

            _renderScheduled = false;
        }
    }

    /// <summary>
    /// 延迟渲染直到下次事件触发
    /// </summary>
    /// <remarks>
    /// 减少不必要的渲染
    /// </remarks>
    protected void MarkForRenderOnNextEvent()
    {
        _renderScheduled = true;
    }

    #endregion
}

#endregion

#region AWComponentBase

/// <summary>
/// Base class for business logic components with form integration.
/// 包含领域特定功能的业务逻辑层
/// </summary>
/// <remarks>
/// Extends <see cref="ResponsiveComponentBase"/> with:
/// - EditContext integration for form validation
/// - Event bus
/// - JS interop
/// - Debounced action handling
/// </remarks>
public abstract class AWComponentBase : ResponsiveComponentBase
{
    /// <summary>
    /// Event bus for cross-component communication.
    /// </summary>
    [Inject]
    protected IEventBus EventBus { get; set; } = null!;

    #region Event

    /// <summary>
    /// Publishes the event.
    /// 发布事件到事件总线
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event.</param>
    protected void PublishEvent<TEvent>(TEvent @event) where TEvent : class
    {
        EventBus.Publish(@event);
    }

    /// <summary>
    /// Subscribes to events of specified type through the event bus.
    /// 订阅事件
    /// </summary>
    /// <typeparam name="TEvent">Type of event to handle</typeparam>
    /// <param name="handler">Event handler delegate</param>
    protected void SubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        EventBus.Subscribe(handler);
    }

    /// <summary>
    /// Unsubscribes all events.
    /// 取消该组件订阅的所有事件
    /// </summary>
    protected void UnsubscribeAllEvents()
    {
        EventBus.UnsubscribeAll(this);
    }

    /// <summary>
    /// Unsubscribes the event.
    /// 取消订阅特定事件处理器
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handler">The handler.</param>
    protected void UnsubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        EventBus.Unsubscribe(handler);
    }

    #endregion

    #region Debounce

    /// <summary>
    /// Creates a debounced version of the specified action.
    /// </summary>
    /// <param name="action">Action to debounce</param>
    /// <param name="milliseconds">Debounce interval in milliseconds</param>
    /// <returns>Debounced action wrapper</returns>
    /// <remarks>
    /// Implement using proper async debounce pattern in actual implementation
    /// </remarks>
    protected static Action Debounce(Action action, int milliseconds = 300)
    {
        return Debouncer.Execute(action, milliseconds);
    }

    /// <summary>
    /// Creates a debounced version of the specified generic action.
    /// </summary>
    /// <typeparam name="T">Type of action parameter</typeparam>
    protected static Action<T> Debounce<T>(Action<T> action, int milliseconds = 300)
    {
        return Debouncer.Execute(action, milliseconds);
    }

    #endregion

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        EventBus.UnsubscribeAll(this);
        base.DisposeManagedResources();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeManagedResourcesAsync()
    {
        EventBus.UnsubscribeAll(this);
        await base.DisposeManagedResourcesAsync().ConfigureAwait(false);
    }
}

#endregion
