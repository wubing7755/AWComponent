using SharedLibrary.Components;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Services;

public class DiagramService : IDiagramService
{
    public DiagramService(IEventBus eventBus)
    {
        eventBus.Subscribe<DiagramKeyEvent>(DiagramKeyHandle);

        _elements = new List<SvgElementBase>();
    }

    private readonly List<SvgElementBase> _elements;
    public IReadOnlyList<SvgElementBase> Elements => _elements.AsReadOnly();

    public int ElementCount => _elements.Count;

    public event Action? OnChange;

    public void Add(SvgElementBase e)
    {
        if(!_elements.Contains(e))
        {
            _elements.Add(e);
        }

        OnChange?.Invoke();
    }

    public bool Contains(SvgElementBase e)
    {
        return _elements.Contains(e);
    }

    public void Remove(SvgElementBase e)
    {
        _elements.Remove(e);

        OnChange?.Invoke();
    }

    public void RemoveAt(int index)
    {
        _elements.RemoveAt(index);

        OnChange?.Invoke();
    }

    private void DiagramKeyHandle(DiagramKeyEvent args)
    {
        switch (args.Code)
        {
            case "Delete":
                for (int i = ElementCount - 1; i >= 0; i--)
                {
                    if (Elements[i].IsSelected)
                    {
                        Elements[i].IsDeleted = true;
                    }
                }
                break;
            case "KeyC" when args.CtrlKey:
                for (int i = ElementCount - 1; i >= 0; i--)
                {
                    if (Elements[i].IsSelected)
                    {
                        Elements[i].IsCopyed = true;
                    }
                }
                break;
            case "KeyV" when args.CtrlKey:
                for (int i = ElementCount - 1; i >= 0; i--)
                {
                    var element = Elements[i];
                    if (element.IsCopyed)
                    {
                        if (element is Rect)
                        {
                            var rect = new Rect();
                            Add(rect);
                        }

                        element.IsCopyed = false;
                    }
                }
                break;
            case "KeyZ" when args.CtrlKey:
                break;
            case "KeyY" when args.CtrlKey:
                break;
        }
    }
}
