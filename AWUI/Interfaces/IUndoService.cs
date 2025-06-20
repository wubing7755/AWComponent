﻿using AWUI.Helper;
using AWUI.Services;

namespace AWUI.Interfaces;

public interface IUndoService
{
    // 可执行撤销恢复的操作
    Result Do(IUndoItem item);

    bool CanUndo { get; }
    bool CanRedo { get; }

    Result Undo();
    Result Redo();
}
