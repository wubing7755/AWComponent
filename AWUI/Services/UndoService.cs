using AWUI.Helper;
using AWUI.Interfaces;

namespace AWUI.Services;

public class UndoService : IUndoService
{
    private readonly object _syncRoot = new();
    private readonly Stack<IUndoItem> _undoStack = new();
    private readonly Stack<IUndoItem> _redoStack = new();

    public bool CanUndo
    {
        get { lock (_syncRoot) return _undoStack.Count > 0; }
    }

    public bool CanRedo
    {
        get { lock (_syncRoot) return _redoStack.Count > 0; }
    }

    public Result Do(IUndoItem item)
    {
        lock(_syncRoot)
        {
            var res = item.Redo();
            if (!res.IsSuccess)
                return res;

            _undoStack.Push(item);
            _redoStack.Clear();

            return res;
        }
    }

    public Result Redo()
    {
        lock (_syncRoot)
        {
            if (!CanRedo)
                return Result.Fail("无法进行恢复操作");

            if (!_redoStack.TryPop(out var item))
                return Result.NotOk;
            var res = item.Redo();
            if (res.IsSuccess)
            {
                _undoStack.Push(item);
            }
            else
            {
                _redoStack.Push(item);
            }
            return res;
        }
    }

    public Result Undo()
    {
        lock (_syncRoot)
        {
            if (!CanUndo)
                return Result.Fail("无法进行撤销操作");

            if (!_undoStack.TryPop(out var item))
                return Result.NotOk;

            var res = item.Undo();
            if (res.IsSuccess)
            {
                _redoStack.Push(item);
            }
            else
            {
                _undoStack.Push(item);
            }
            return res;
        }
    }
}
