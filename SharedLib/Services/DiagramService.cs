using SharedLibrary.Components;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Services;

public class DiagramService : IDiagramService
{
    private readonly List<SvgElementBase> _elements = new();
    public IReadOnlyList<SvgElementBase> Elements => _elements;

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
}
