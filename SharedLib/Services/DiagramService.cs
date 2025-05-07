using SharedLibrary.Components;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Services;

public class DiagramService : IDiagramService
{
    private readonly List<DraggableSVGElement> _elements = new();
    public IReadOnlyList<DraggableSVGElement> Elements => _elements;

    public int ElementCount => _elements.Count;

    public event Action? OnChange;

    public void Add(DraggableSVGElement e)
    {
        if(!_elements.Contains(e))
        {
            _elements.Add(e);
        }

        OnChange?.Invoke();
    }

    public void Remove(DraggableSVGElement e)
    {
        _elements.Remove(e);

        OnChange?.Invoke();
    }

    public void RemoveAt(int index)
    {
        _elements.RemoveAt(index);

        OnChange?.Invoke();
    }
}
