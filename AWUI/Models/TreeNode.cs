using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AWUI.Models;

public class TreeNode : IEnumerable<TreeNode>
{
    /// <summary>
    /// 节点唯一标识符 (不可为空)
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 节点显示文本 (不可为空)
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 节点图标类名
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 节点展开时的图标类名
    /// </summary>
    public string? ExpandedIcon { get; set; }

    /// <summary>
    /// 附加的自定义数据
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// 节点层级 (0 表示根节点)
    /// </summary>
    public int Level { get; internal set; } = 0;

    /// <summary>
    /// 是否展开
    /// </summary>
    public bool IsExpanded { get; set; } = false;

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelected { get; set; } = false;

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool IsDisabled { get; set; } = false;

    private List<TreeNode>? _children;

    /// <summary>
    /// 子节点列表 (延迟初始化)
    /// </summary>
    [NotNull]
    public List<TreeNode> Children
    {
        get => _children ??= new List<TreeNode>();
        set
        {
            _children = value ?? throw new ArgumentNullException(nameof(value));

            foreach (var child in _children)
            {
                child.Level = Level + 1;
                child.UpdateChildLevels(); // 递归更新子树的层级
            }
        }
    }

    /// <summary>
    /// 是否有子节点
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// 是否是叶子节点
    /// </summary>
    public bool IsLeaf => !HasChildren;

    /// <summary>
    /// 当前应显示的图标
    /// </summary>
    public string CurrentIcon
    {
        get
        {
            if (IsLeaf) return Icon ?? string.Empty;

            return IsExpanded ? (ExpandedIcon ?? "iconfont aw-Unfold") : (Icon ?? "iconfont aw-fold");
        }
    }

    /// <summary>
    /// 创建树节点
    /// </summary>
    /// <param name="text">节点显示文本 (必须)</param>
    /// <param name="icon">节点图标 (可选)</param>
    /// <param name="expandedIcon">展开时图标 (可选)</param>
    /// <exception cref="ArgumentNullException">当 text 为 null 时抛出</exception>
    public TreeNode(string text, string? icon = null, string? expandedIcon = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Icon = icon;
        ExpandedIcon = expandedIcon;
    }

    /// <summary>
    /// 切换展开/折叠状态
    /// </summary>
    public void Toggle() => IsExpanded = !IsExpanded;

    /// <summary>
    /// 添加子节点（支持集合初始化器语法）
    /// </summary>
    public void Add(TreeNode node)
    {
        AddChild(node);
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="node">要添加的节点</param>
    /// <exception cref="ArgumentNullException">当 node 为 null 时抛出</exception>
    public void AddChild(TreeNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        Children.Add(node);
    }

    /// <summary>
    /// 批量添加子节点
    /// </summary>
    /// <param name="nodes">要添加的节点集合</param>
    public void AddChildren(IEnumerable<TreeNode> nodes)
    {
        foreach (var node in nodes ?? Enumerable.Empty<TreeNode>())
        {
            AddChild(node);
        }
    }

    /// <summary>
    /// 扁平化当前节点及其所有后代节点
    /// </summary>
    public IEnumerable<TreeNode> Flatten()
    {
        yield return this;

        foreach (var child in Children)
            foreach (var descendant in child.Flatten())
            {
                yield return descendant;
            }
    }

    /// <summary>
    /// 根据 ID 查找节点
    /// </summary>
    /// <param name="id">要查找的节点 ID</param>
    /// <returns>找到的节点或 null</returns>
    public TreeNode? FindNode(string id) => Flatten().FirstOrDefault(n => n.Id == id);

    /// <summary>
    /// 递归设置所有子节点的层级
    /// </summary>
    internal void UpdateChildLevels()
    {
        foreach (var child in Children)
        {
            child.Level = Level + 1;
            child.UpdateChildLevels();
        }
    }

    public override string ToString() => $"{Text} (Level: {Level}, Children: {Children.Count})";

    public IEnumerator<TreeNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
