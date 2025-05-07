using SharedLibrary.Components;

namespace SharedLibrary.Interfaces;

public interface IDiagramService
{
    IReadOnlyList<DraggableSVGElement> Elements { get; }

    int ElementCount { get; }

    void Add(DraggableSVGElement e);

    void Remove(DraggableSVGElement e);

    void RemoveAt(int index);

    event Action? OnChange;
}
