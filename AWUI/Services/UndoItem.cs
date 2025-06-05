using AWUI.Helper;

namespace AWUI.Services;

/// <summary>
/// Represents an undoable operation in the system with automatic rollback on error
/// 表示系统中可撤销的操作（执行错误时自动回滚）
/// </summary>
public interface IUndoItem
{
    /// <summary>
    /// Unique identifier for the undo operation
    /// 撤销操作唯一标识符
    /// </summary>
    Guid OperationId { get; }

    /// <summary>
    /// Timestamp when the operation was performed
    /// 操作执行时间戳
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// Description of the operation (for UI display)
    /// 操作描述（用于UI显示）
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the operation
    /// 执行恢复操作
    /// </summary>
    /// <returns>Operation result</returns>
    Result Redo();

    /// <summary>
    /// Executes the undo operation
    /// 执行撤销操作
    /// </summary>
    /// <returns>Operation result</returns>
    Result Undo();

    /// <summary>
    /// Callback after successful Redo operation
    /// Redo操作成功后的回调
    /// </summary>
    Action<Result, object?>? AfterRedo { get; set; }

    /// <summary>
    /// Callback after successful Undo operation
    /// Undo操作成功后的回调
    /// </summary>
    Action<Result, object?>? AfterUndo { get; set; }

    /// <summary>
    /// Merges with a subsequent operation if possible (for command compression)
    /// 如果可能，与后续操作合并（用于命令压缩）
    /// </summary>
    bool TryMerge(IUndoItem subsequentOperation);
}

/// <summary>
/// Base implementation of an undoable operation with common functionality
/// 可撤销操作的基础实现，包含通用功能
/// </summary>
public abstract class UndoItem : IUndoItem
{
    private readonly Lazy<Guid> _operationId;
    private readonly Lazy<DateTime> _timestamp;

    protected UndoItem(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _operationId = new Lazy<Guid>(Guid.NewGuid);
        _timestamp = new Lazy<DateTime>(() => DateTime.UtcNow);
    }

    public Guid OperationId => _operationId.Value;
    public DateTime Timestamp => _timestamp.Value;
    public string Description { get; protected set; }
    public Action<Result, object?>? AfterRedo { get; set; }
    public Action<Result, object?>? AfterUndo { get; set; }

    public abstract Result Redo();
    public abstract Result Undo();

    public virtual bool TryMerge(IUndoItem subsequentOperation) => false;

    public override string ToString() => Description;
}

/// <summary>
/// Generic implementation for undoable operations with value
/// 带值的可撤销操作通用实现
/// </summary>
/// <typeparam name="TValue">Type of value associated with the operation</typeparam>
public abstract class UndoItem<TValue> : UndoItem
{
    protected readonly TValue Value;

    protected UndoItem(TValue value, string description) : base(description)
    {
        Value = value;
    }

    protected virtual Result RedoFunction(TValue value) => Result.Ok;
    protected virtual Result UndoFunction(TValue value) => Result.Ok;

    public override Result Redo()
    {
        Result res;
        try
        {
            res = RedoFunction(Value);
        }
        catch (Exception e)
        {
            res = Result.Fail(e.ToString());
        }
        AfterRedo?.Invoke(res, Value);
        return res;
    }

    public override Result Undo()
    {
        Result res;
        try
        {
            res = UndoFunction(Value);
        }
        catch (Exception e)
        {
            res = Result.Fail(e.ToString());
        }
        AfterUndo?.Invoke(res, Value);
        return res;
    }
}

/// <summary>
/// 基于委托实现的可撤销操作项（命令模式的具体实现）
/// <para>核心功能</para>
/// <list type="bullet">
/// <item>通过Lambda表达式定义操作/撤销行为</item>
/// <item>自动异常处理与结果回调</item>
/// <item>支持操作合并(Command Compression)</item>
/// </list>
/// </summary>
/// <typeparam name="TValue">操作关联值类型（通常包含操作上下文）</typeparam>
/// <example>
/// 【文档内容修改】
/// <code>
/// // 1. 创建可撤销操作
/// var operation = new ActionUndoItem<(Document Doc, string NewText)>(
///     (currentDoc, "修改后的文本"),
///     
///     // 执行操作（Do）
///     x => {
///         x.Doc.History.Push(x.Doc.Content); // 保存旧值
///         x.Doc.Content = x.NewText;         // 应用新值
///         return Result.Success();
///     },
///     
///     // 撤销操作（Undo） 
///     x => {
///         x.Doc.Content = x.Doc.History.Pop(); // 恢复旧值
///         return Result.Success();
///     },
///     
///     description: "修改文档正文"
/// );
/// 
/// // 2. 配置执行后回调（AfterDo）
/// operation.AfterDo = (result, ctx) => {
///     if (result.IsSuccess) {
///         StatusBar.Show($"文档更新成功 | 版本: {result.Data.OperationId}");
///         Editor.MarkModified();
///         
///         // 自动保存到临时副本
///         AutoSaveManager.CreateSnapshot(ctx.Doc);
///     } else {
///         ErrorDialog.Show("文档更新失败", result.Error);
///     }
/// };
/// 
/// // 3. 配置撤销后回调（AfterUndo）
/// operation.AfterUndo = (result, ctx) => {
///     if (result.IsSuccess) {
///         StatusBar.Show($"已恢复版本: {result.Data.OperationId}");
///         Editor.ReloadSyntaxHighlighting();
///         
///         // 撤销时清除临时副本
///         AutoSaveManager.ClearSnapshot(ctx.Doc.Id);
///     } else {
///         ErrorDialog.Show("撤销操作失败", result.Error);
///     }
/// };
/// 
/// // 4. 执行操作（演示完整生命周期）
/// var executeResult = operation.Do(); 
/// /* 将依次触发：
///    - Do() 方法执行
///    - AfterDo 回调
///    - 用户界面更新
/// */
/// 
/// // 5. 撤销操作（当用户点击撤销按钮时）
/// if(executeResult.IsSuccess) 
/// {
///     var undoResult = operation.Undo();
///     /* 将依次触发：
///        - Undo() 方法执行
///        - AfterUndo 回调
///        - 界面状态回滚
///     */
/// }
/// </code>
/// </example>
/// <remarks>
/// ■ 回调设计规范：
/// 1. 执行顺序保证：Do/Undo → AfterDo/AfterUndo
/// 2. 上下文传递：通过 <typeparamref name="TValue"/> 共享操作上下文
/// 3. 错误处理：建议在回调中处理所有预期异常
/// </remarks>
public class ActionUndoItem<TValue> : UndoItem<TValue>
{
    private readonly Func<TValue, Result> _redoFunc;
    private readonly Func<TValue, Result> _undoFunc;

    public ActionUndoItem(
        TValue value,
        Func<TValue, Result> redoFunc,
        Func<TValue, Result> undoFunc,
        string description) : base(value, description)
    {
        _redoFunc = redoFunc ?? throw new ArgumentNullException(nameof(redoFunc));
        _undoFunc = undoFunc ?? throw new ArgumentNullException(nameof(undoFunc));
    }

    protected override Result RedoFunction(TValue value) => _redoFunc(value);
    protected override Result UndoFunction(TValue value) => _undoFunc(value);
}
