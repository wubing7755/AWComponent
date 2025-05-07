using SharedLibrary.Components;

namespace SharedLibrary.Interfaces;

public interface IDiagramService
{
    IReadOnlyList<SvgElementBase> Elements { get; }

    int ElementCount { get; }

    void Add(SvgElementBase e);

    void Remove(SvgElementBase e);

    void RemoveAt(int index);

    bool Contains(SvgElementBase e);

    event Action? OnChange;
}
