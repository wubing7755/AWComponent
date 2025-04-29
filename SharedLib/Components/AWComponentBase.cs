using System.Text;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using SharedLibrary.JsInterop;

namespace SharedLibrary.Components;

#region Utils

/// <summary>
/// Defines responsive breakpoints for UI components.
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
    Custom          // 自定义
}

/// <summary>
/// Contains theme configuration settings for component styling.
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

#region SecureComponentBase

/// <summary>
/// Abstract base class for all components in the system.
/// </summary>
/// <remarks>
/// Implements core functionality including:
/// - Attribute safety filtering
/// - Resource disposal management
/// - Theme propagation
/// - Localization support
/// </remarks>
public abstract class SecureComponentBase : ComponentBase, IDisposable
{
    public SecureComponentBase()
    {
        ObjectId = Guid.NewGuid();
    }

    private bool _disposed;
    private IReadOnlyDictionary<string, object>? _safeAttributes;

    public Guid ObjectId { get; protected set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied 
    /// to the created element.
    /// </summary>
    /// <remarks>
    /// This parameter captures all unmatched attribute values passed to the component. 
    /// Use <see cref="SafeAttributes"/> to access filtered attributes.
    /// </remarks>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the localized string resources for this component.
    /// </summary>
    [Inject]
    protected IStringLocalizer<SecureComponentBase>? L { get; set; }

    /// <summary>
    /// Cascading parameter providing theme settings to descendant components.
    /// </summary>
    [CascadingParameter]
    protected ThemeSettings? Theme { get; set; }

    /// <summary>
    /// Gets the safely filtered attributes after applying security rules.
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

        if (attributes == null) return null;
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
        return !attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Renders filtered attributes to the specified render tree builder.
    /// </summary>
    /// <param name="builder">Target render tree builder</param>
    /// <param name="sequence">Sequence number for rendering</param>
    protected void MergeAttributes(RenderTreeBuilder builder, int seq)
    {
        if (SafeAttributes is not null)
        {
            builder.AddMultipleAttributes(seq, SafeAttributes);
        }
    }

    /// <summary>
    /// Releases managed resources used by this component.
    /// </summary>
    /// <remarks>
    /// Override this method to dispose any IDisposable resources 
    /// created by your component.
    /// </remarks>
    protected virtual void DisposeManagedResources() { }

    /// <summary>
    /// Releases unmanaged resources used by this component.
    /// </summary>
    /// <remarks>
    /// Always call base.DisposeUnmanagedResources() when overriding.
    /// </remarks>
    protected virtual void DisposeUnmanagedResources() { }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, 
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        DisposeManagedResources();
        DisposeUnmanagedResources();
        _disposed = true;
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
/// </summary>
/// <remarks>
/// Extends <see cref="SecureComponentBase"/> with:
/// - CSS class composition
/// - Inline style merging
/// - Responsive breakpoint handling
/// </remarks>
public abstract class ResponsiveComponentBase : SecureComponentBase
{
    protected virtual string BaseCssClass => string.Empty;
    protected virtual string BaseStyle => string.Empty;

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

    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildComponent(builder);
    }

    protected abstract void BuildComponent(RenderTreeBuilder builder);
}

#endregion

#region AWComponentBase

/// <summary>
/// Base class for business logic components with form integration.
/// </summary>
/// <remarks>
/// Extends <see cref="ResponsiveComponentBase"/> with:
/// - EditContext integration for form validation
/// - Event bus communication
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

    /// <summary>
    /// Subscribes to events of specified type through the event bus.
    /// </summary>
    /// <typeparam name="TEvent">Type of event to handle</typeparam>
    /// <param name="handler">Event handler delegate</param>
    protected void SubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        EventBus.Subscribe(handler);
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        EventBus.UnsubscribeAll(this);
        base.DisposeManagedResources();
    }

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
}

#endregion

#region Helper Classes

/// <summary>
/// Provides debounce functionality for high-frequency events.
/// </summary>
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
