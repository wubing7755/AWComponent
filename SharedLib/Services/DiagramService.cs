using SharedLibrary.Components;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
using SharedLibrary.Models;

namespace SharedLibrary.Services;

public class DiagramService : IDiagramService
{
    private readonly IUndoService _undoService;

    public DiagramService(IEventBus eventBus, IUndoService undoService)
    {
        eventBus.Subscribe<DiagramKeyEvent>(DiagramKeyHandle);

        _elements = new List<DraggableSvgElementModel>();
        _undoService = undoService;
    }

    private readonly List<DraggableSvgElementModel> _elements;
    public IReadOnlyList<DraggableSvgElementModel> Elements => _elements.AsReadOnly();

    private ActionUndoItem<DraggableSvgElementModel> undoItem;

    public int ElementCount => _elements.Count;

    public event Action? OnChange;

    public void Add(DraggableSvgElementModel e)
    {
        if(!_elements.Contains(e))
        {
            _elements.Add(e);
        }

        OnChange?.Invoke();
    }

    public bool Contains(DraggableSvgElementModel e)
    {
        return _elements.Contains(e);
    }

    public void Remove(DraggableSvgElementModel e)
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
                    if (!element.IsDeleted && element.IsCopyed)
                    {
                        if (element is RectModel)
                        {
                            var rectM = new RectModel();
                            Add(rectM);

                            undoItem = UndoFactory.SvgElementUndoItem(rectM);
                            _undoService.Do(undoItem);
                        }

                        element.IsCopyed = false;
                    }
                }
                break;
            case "KeyZ" when args.CtrlKey:
                _undoService.Undo();
                break;
            case "KeyY" when args.CtrlKey:
                _undoService.Redo();
                break;
        }
    }
}
