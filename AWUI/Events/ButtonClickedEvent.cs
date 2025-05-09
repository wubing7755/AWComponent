namespace AWUI.Events;

public class ButtonClickedEvent
{
    public DateTime ClickTime { get; }
    public string? ButtonId { get; }

    public ButtonClickedEvent(string? buttonId)
    {
        ClickTime = DateTime.Now;
        ButtonId = buttonId;
    }
}

public class ButtonHoverEvent
{
    public bool IsHovering { get; }
    public string? ButtonId { get; }
    public ButtonHoverEvent(bool isHovering, string? buttonId = null)
    {
        IsHovering = isHovering;
        ButtonId = buttonId;
    }
}