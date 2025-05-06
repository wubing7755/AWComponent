using System.Text;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using SharedLibrary.JsInterop;

namespace SharedLibrary.Components;

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
| SecureComponentBase     | - Theme management                                 |
|                         | - Localization services (Localizer)                |
|                         | - Attribute security filtering (SafeAttributes)    |
|-------------------------|----------------------------------------------------|
| ResponsiveComponentBase | - CSS class composition (CssClass)                 |
|                         | - Inline style handling (Style)                    |
|                         | - Disabled state management                        |
|                         | - Component building pipeline (BuildComponent)     |
|-------------------------|----------------------------------------------------|
| AWComponentBase         | - Form validation context (EditContext)            |
|                         | - Cross-component communication (EventBus)         |
|                         | - JavaScript interoperability (JsInterop)          |
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
public abstract class SecureComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
    public SecureComponentBase()
    {
        ObjectId = Guid.NewGuid();
    }

    private bool _disposed;
    private bool _disposedAsync;
    private IReadOnlyDictionary<string, object>? _safeAttributes;

    /// <summary>
    /// Unique identifier for component instance tracking
    /// 组件实例唯一标识符
    /// </summary>
    public Guid ObjectId { get; protected set; }

    /// <summary>
    /// Gets the localized string resources for this component.
    /// 国际化文本资源
    /// </summary>
    /// <remarks>
    /// usage: Localizer["bingbing"]
    /// </remarks>
    [Inject]
    protected IStringLocalizer<SecureComponentBase>? Localizer { get; set; }

    /// <summary>
    /// Cascading parameter providing theme settings to descendant components.
    /// </summary>
    [CascadingParameter]
    protected ThemeSettings? Theme { get; set; }

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
        => _safeAttributes ??= FilterAttributes(AdditionalAttributes);

    /// <summary>
    /// Filters raw attributes according to component security policy.
    /// </summary>
    /// <param name="attributes">Original attribute collection</param>
    /// <returns>Filtered attributes meeting safety criteria</returns>
    /// <seealso cref="IsAttributeAllowed"/>
    protected virtual IReadOnlyDictionary<string, object>? FilterAttributes(IReadOnlyDictionary<string, object>? attributes)
    {

        if (attributes is null) return null;
        var filtered = new Dictionary<string, object>();
        foreach (var attr in attributes)
        {
            if (IsAttributeAllowed(attr.Key))
            {
                filtered[attr.Key] = attr.Value;
            }
        }
        return new ReadOnlyDictionary<string, object>(filtered);
    }

    /// <summary>
    /// Determines if a given attribute is allowed to be rendered.
    /// 判断属性是否通过安全策略
    /// </summary>
    /// <param name="attributeName">Name of the HTML attribute</param>
    /// <returns>
    /// True if the attribute is permitted by security rules, false otherwise.
    /// </returns>
    /// <remarks>
    /// Base implementation blocks:
    /// - All event handlers (attributes starting with "on")
    /// </remarks>
    protected virtual bool IsAttributeAllowed(string attributeName)
    {
        // Security: Block all event handlers to prevent XSS
        // 安全措施：阻止所有事件处理器属性防止XSS
        return !attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Renders filtered attributes to the specified render tree builder.
    /// </summary>
    /// <param name="builder">Target render tree builder</param>
    /// <param name="sequence">Sequence number for rendering</param>
    protected void RenderFilteredAttributes(RenderTreeBuilder builder, int seq)
    {
        if (SafeAttributes is not null)
        {
            builder.AddMultipleAttributes(seq, SafeAttributes);
        }
    }

    #endregion

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
        if (_disposed) return;
        DisposeManagedResources();
        DisposeUnmanagedResources();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposedAsync) return;

        await DisposeManagedResourcesAsync().ConfigureAwait(false);
        await DisposeUnmanagedResourcesAsync().ConfigureAwait(false);

        _disposedAsync = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer ensuring unmanaged resources are released
    /// </summary>
    ~SecureComponentBase() => DisposeUnmanagedResources();
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
    private bool _shouldRenderAfterEvent = false;
    private bool _isEventHandling;

    protected virtual string BaseCssClass => string.Empty;
    protected virtual string BaseStyle => string.Empty;

    /// <summary>
    /// Indicates whether to enable render optimization for event handling.
    /// 开启自定义渲染
    /// </summary>
    protected virtual bool EnableRenderOptimization => true;

    /// <summary>
    /// Gets or sets the CSS class string applied to the root element.
    /// </summary>
    /// <remarks>
    /// Will be combined with base classes and additional attribute classes.
    /// </remarks>
    [Parameter]
    public string? CssClass { get; set; }

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
    public bool Disabled { get; set; }

    /// <summary>
    /// Controls the minimum viewport width at which this component becomes active.
    /// </summary>
    [Parameter]
    public Breakpoint Breakpoint { get; set; }

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
    protected string BuildCssClass()
    {
        var sb = new StringBuilder(BaseCssClass);

        if (!string.IsNullOrEmpty(CssClass))
        {
            sb.Append(' ').Append(CssClass);
        }

        if (AdditionalAttributes?.TryGetValue("class", out var extraClass) == true)
        {
            sb.Append(' ').Append(extraClass);
        }

        return sb.ToString().Trim();
    }

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
    protected string BuildStyle()
    {
        var sb = new StringBuilder(BaseStyle);

        if (!string.IsNullOrEmpty(Style))
        {
            sb.Append(';').Append(Style);
        }

        if (Breakpoint != Breakpoint.None)
        {
            sb.Append($";min-width:{(int)Breakpoint}px");
        }

        if (AdditionalAttributes?.TryGetValue("style", out var extraStyle) == true)
        {
            sb.Append(';').Append(extraStyle);
        }

        return sb.ToString().Trim(';');
    }

    /// <inheritdoc/>
    /// <remarks>
    /// UI components allow class and style attributes from AdditionalAttributes
    /// </remarks>
    protected override bool IsAttributeAllowed(string attributeName)
    {
        return base.IsAttributeAllowed(attributeName) ||
               attributeName.Equals("class", StringComparison.OrdinalIgnoreCase) ||
               attributeName.Equals("style", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Template method for component-specific rendering
    /// 组件特定渲染的模板方法
    /// </summary>
    /// <param name="builder">Blazor render tree builder</param>
    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildComponent(builder);
    }

    protected abstract void BuildComponent(RenderTreeBuilder builder);

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? arg)
    {
        // 1. 标记事件处理中状态
        _isEventHandling = true;
        try
        {
            // 2. 执行原始回调
            var task = item.InvokeAsync(arg);

            // 3. 根据条件决定是否触发渲染
            if (EnableRenderOptimization && !_shouldRenderAfterEvent)
            {
                // 跳过渲染
                return task;
            }

            // 4. 异步等待回调完成后触发渲染
            return HandleEventWithRendering(task);
        }
        finally
        {
            // 5. 重置标记
            _isEventHandling = false;
            _shouldRenderAfterEvent = false;
        }
    }

    private async Task HandleEventWithRendering(Task originalTask)
    {
        try
        {
            await originalTask;
        }
        finally
        {
            // 确保即使回调抛出异常也能恢复UI状态
            // 防止嵌套事件重复渲染
            if (!_isEventHandling)
            {
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// 标记下一次事件处理需要触发渲染
    /// </summary>
    protected void RequestRenderOnNextEvent()
    {
        _shouldRenderAfterEvent = true;
    }

    /// <summary>
    /// 强制立即渲染
    /// </summary>
    protected void ForceImmediateRender()
    {
        StateHasChanged();
    }
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
    /// Cascading parameter providing form validation context.
    /// </summary>
    [CascadingParameter]
    protected EditContext? EditContext { get; set; }

    /// <summary>
    /// Event bus for cross-component communication.
    /// </summary>
    [Inject]
    protected IEventBus EventBus { get; set; } = null!;

    /// <summary>
    /// Provides JavaScript interop functionality for the component.
    /// </summary>
    [Inject]
    protected AWJsInterop JsInterop { get; set; } = null!;

    /// <summary>
    /// Indicates whether the current form has validation errors.
    /// </summary>
    protected bool HasValidationErrors =>
        EditContext?.GetValidationMessages().Any() ?? false;

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
    protected Action Debounce(Action action, int milliseconds = 300)
    {
        return Debouncer.Execute(action, milliseconds);
    }

    /// <summary>
    /// Creates a debounced version of the specified generic action.
    /// </summary>
    /// <typeparam name="T">Type of action parameter</typeparam>
    protected Action<T> Debounce<T>(Action<T> action, int milliseconds = 300)
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

#region Helper Classes

/// <summary>
/// Provides debounce functionality for high-frequency events.
/// 使用CancellationTokenSource实现防抖模式
/// </summary>
/// /// <remarks>
/// Usage example (使用示例):
/// <code>
/// var debouncedSave = Debouncer.Execute(SaveData, 1000);
/// input.OnChange += debouncedSave;
/// </code>
/// </remarks>
public static class Debouncer
{
    /// <summary>
    /// Wraps an action to prevent frequent execution.
    /// </summary>
    /// <param name="action">Target action</param>
    /// <param name="milliseconds">Minimum interval between executions</param>
    public static Action Execute(Action action, int milliseconds)
    {
        CancellationTokenSource? cts = null;

        return () =>
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            Task.Delay(milliseconds, cts.Token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;

                action.Invoke();
            }, TaskScheduler.Default);
        };
    }

    /// <inheritdoc cref="Execute(Action, int)"/>
    public static Action<T> Execute<T>(Action<T> action, int milliseconds)
    {
        CancellationTokenSource? cts = null;
        T? lastArg = default;

        return arg =>
        {
            lastArg = arg;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            Task.Delay(milliseconds, cts.Token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;

                action.Invoke(lastArg);
            }, TaskScheduler.Default);
        };
    }
}

/// <summary>
/// Defines a contract for event-based communication between components.
/// </summary>
public interface IEventBus
{
    void Publish<TEvent>(TEvent @event) where TEvent : class;

    /// <summary>
    /// Registers a handler for events of type <typeparamref name="T"/>.
    /// </summary>
    void Subscribe<TEvent>(Action<TEvent> handler)where TEvent : class;

    /// <summary>
    /// Unregisters all event handlers from the specified subscriber.
    /// </summary>
    /// <param name="subscriber">Subscriber to remove</param>
    void UnsubscribeAll(object subscriber);

    /// <summary>
    /// Unsubscribes a previously registered event handler for the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <param name="handler">The event handler delegate to remove.</param>
    void Unsubscribe<TEvent>(Action<TEvent> handler);
}

#endregion

#region Utils

/// <summary>
/// Layout Breakpoint. Defines responsive breakpoints for UI components.
/// 布局断裂点。定义响应式布局的标准断点。
/// </summary>
/// <remarks>
/// Values represent minimum viewport widths in pixels.
/// </remarks>
public enum Breakpoint
{
    None = 0,
    ExtraSmall = 256,
    Small = 512,
    Medium = 640,
    Large = 1024,
    ExtraLarge = 2048,
    Custom
}

/// <summary>
/// Contains theme configuration settings for component styling.
/// 主题配置容器
/// </summary>
public class ThemeSettings
{
    /// <summary>
    /// Primary brand color
    /// </summary>
    public string PrimaryColor { get; set; } = "#2563eb";

    public string FontFamily { get; set; } = "Fira Code";
}

#endregion