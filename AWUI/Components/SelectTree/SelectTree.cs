using AWUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace AWUI.Components;

public class SelectTree : AWComponentBase
{
    [Parameter]
    public TreeNode TreeNode { get; set; } = default!;

    [Parameter]
    public Action<TreeNode>? OnNodeSelected { get; set; }

    protected sealed override string BaseCssClass => "aw-selectTree";

    protected virtual string SelectTreeClass => BuildCssClass();

    protected virtual string SelectTreeStyle => BuildStyle();

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        // 最外层容器
        builder.OpenElement(seq++, "div");
        builder.AddMultipleAttributes(seq++, SafeAttributes);
        builder.AddAttribute(seq++, "class", SelectTreeClass);
        builder.AddAttribute(seq++, "style", SelectTreeStyle);

        BuildLeafNode(builder, TreeNode, ref seq);
        
        builder.CloseElement();
    }

    protected virtual void BuildLeafNode(RenderTreeBuilder builder, TreeNode treeNode, ref int seq)
    {
        // 节点容器
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "style", $"margin-left: {treeNode.Level * 20}px"); // 每级缩进20px

        // 图标
        builder.OpenElement(seq++, "span");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, 
            () => {
                treeNode.Toggle();
                ForceImmediateRender();
            }));

        builder.OpenElement(seq++, "i");
        builder.AddAttribute(seq++, "class", treeNode.CurrentIcon);
        builder.CloseElement();

        builder.CloseElement();

        // 文字
        builder.OpenElement(seq++, "span");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this,
            () => {
                OnNodeSelected?.Invoke(treeNode);
            }));
        builder.AddContent(seq, treeNode.Text);
        builder.CloseElement();

        builder.CloseElement();

        // 子节点
        if (treeNode.IsExpanded && treeNode.HasChildren)
        {
            foreach(var child in treeNode.Children)
            {
                BuildLeafNode(builder, child, ref seq);
            }
        }
    }
}
