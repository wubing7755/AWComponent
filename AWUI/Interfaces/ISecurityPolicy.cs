namespace AWUI.Interfaces;

public interface ISecurityPolicy
{
    /// <summary>
    /// Filters raw attributes according to component security policy.
    /// </summary>
    /// <param name="attributes">Original attribute collection</param>
    /// <returns>Filtered attributes meeting safety criteria</returns>
    /// <seealso cref="IsAttributeAllowed"/>
    IReadOnlyDictionary<string, object>? Filter(IReadOnlyDictionary<string, object>? attributes);


    /// <summary>
    /// Determines if a given attribute is allowed to be rendered.
    /// 判断属性是否通过安全策略
    /// </summary>
    /// <param name="attributeName">Name of the HTML attribute</param>
    /// <returns>
    /// True if the attribute is permitted by security rules, false otherwise.
    /// </returns>
    /// <remarks>
    /// Base implementation blocks:
    /// - All event handlers (attributes starting with "on")
    /// </remarks>
    bool IsAttributeAllowed(string attributeName, object attributeValue);
}
