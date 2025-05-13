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
        // 最外层容器
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, SafeAttributes);
        builder.AddAttribute(2, "class", SelectTreeClass);
        builder.AddAttribute(3, "style", SelectTreeStyle);

        int seq = 5;

        BuildLeafNode(builder, TreeNode, ref seq);
        
        builder.CloseElement();
    }

    protected virtual void BuildLeafNode(RenderTreeBuilder builder, TreeNode treeNode, ref int seq)
    {
        builder.OpenRegion(seq++);
        // 节点容器
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "style", $"margin-left: {treeNode.Level * 20}px"); // 每级缩进20px

        // 图标
        builder.OpenElement(2, "span");
        builder.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, 
            () => {
                treeNode.Toggle();
                ForceImmediateRender();
            }));

        builder.OpenElement(4, "i");
        builder.AddAttribute(5, "class", treeNode.CurrentIcon);
        builder.CloseElement();

        builder.CloseElement();

        // 文字
        builder.OpenElement(6, "span");
        builder.AddAttribute(7, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this,
            () => {
                OnNodeSelected?.Invoke(treeNode);
            }));
        builder.AddContent(8, treeNode.Text);
        builder.CloseElement();

        builder.CloseElement();
        builder.CloseRegion();

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
