namespace AWUI.Events;

public class InputKeyEvent
{
    public string Id { get; }

    public string Key { get; }

    public InputKeyEvent(string id, string key)
    {
        Id = id;
        Key = key;
    }
}
