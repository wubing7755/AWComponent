using SharedLibrary.Models;

namespace SharedLibrary.Services;

public static class UndoFactory
{
    public static ActionUndoItem<DraggableSvgElementModel> SvgElementUndoItem(DraggableSvgElementModel model)
    {
        var undoItem = new ActionUndoItem<DraggableSvgElementModel>
        (
            model,
            (ctx) => {
                model.IsDeleted = false;
                return Result.Ok; 
            },
            (ctx) => {
                model.IsDeleted = true;
                return Result.Ok; 
            },
            "svg 元素 Ctrl + V 的 do 和 undo 操作"
        );

        return undoItem;
    }
}
