using AWUI.Enums;

namespace AWUI.Helper;

/// <summary>
/// 提供将AutocompleteType枚举转换为HTML autocomplete属性值的方法
/// </summary>
public static class AutocompleteFactory
{
    private static readonly Dictionary<AutocompleteType, string> _autocompleteMappings = new Dictionary<AutocompleteType, string>
    {
        { AutocompleteType.Off, "off" },
        { AutocompleteType.On, "on" },
        { AutocompleteType.Name, "name" },
        { AutocompleteType.GivenName, "given-name" },
        { AutocompleteType.FamilyName, "family-name" },
        { AutocompleteType.AdditionalName, "additional-name" },
        { AutocompleteType.Nickname, "nickname" },
        { AutocompleteType.Email, "email" },
        { AutocompleteType.Username, "username" },
        { AutocompleteType.NewPassword, "new-password" },
        { AutocompleteType.CurrentPassword, "current-password" },
        { AutocompleteType.OneTimeCode, "one-time-code" },
        { AutocompleteType.Organization, "organization" },
        { AutocompleteType.OrganizationTitle, "organization-title" },
        { AutocompleteType.StreetAddress, "street-address" },
        { AutocompleteType.AddressLine1, "address-line1" },
        { AutocompleteType.AddressLine2, "address-line2" },
        { AutocompleteType.AddressLine3, "address-line3" },
        { AutocompleteType.Country, "country" },
        { AutocompleteType.CountryName, "country-name" },
        { AutocompleteType.PostalCode, "postal-code" },
        { AutocompleteType.Tel, "tel" },
        { AutocompleteType.TelCountryCode, "tel-country-code" },
        { AutocompleteType.TelNational, "tel-national" },
        { AutocompleteType.TelMobile, "tel-mobile" },
        { AutocompleteType.CcName, "cc-name" },
        { AutocompleteType.CcNumber, "cc-number" },
        { AutocompleteType.CcExpMonth, "cc-exp-month" },
        { AutocompleteType.CcExpYear, "cc-exp-year" },
        { AutocompleteType.CcCsc, "cc-csc" },
        { AutocompleteType.Bday, "bday" },
        { AutocompleteType.BdayDay, "bday-day" },
        { AutocompleteType.BdayMonth, "bday-month" },
        { AutocompleteType.BdayYear, "bday-year" },
        { AutocompleteType.Sex, "sex" },
        { AutocompleteType.Photo, "photo" },
        { AutocompleteType.Impp, "impp" },
        { AutocompleteType.Url, "url" },
        { AutocompleteType.TransactionAmount, "transaction-amount" },
        { AutocompleteType.TransactionCurrency, "transaction-currency" },
        { AutocompleteType.Language, "language" }
    };

    /// <summary>
    /// 获取AutocompleteType对应的HTML autocomplete属性值
    /// </summary>
    /// <param name="type">AutocompleteType枚举值</param>
    /// <returns>HTML autocomplete属性值字符串</returns>
    public static string GetAutocomplete(AutocompleteType type)
    {
        if (_autocompleteMappings.TryGetValue(type, out var autocompleteValue))
        {
            return autocompleteValue;
        }

        throw new ArgumentException($"未找到AutocompleteType.{type}对应的HTML autocomplete属性值", nameof(type));
    }

    /// <summary>
    /// 尝试获取AutocompleteType对应的HTML autocomplete属性值
    /// </summary>
    /// <param name="type">AutocompleteType枚举值</param>
    /// <param name="autocompleteValue">输出参数，HTML autocomplete属性值</param>
    /// <returns>是否成功找到对应的值</returns>
    public static bool TryGetAutocomplete(AutocompleteType type, out string? autocompleteValue)
    {
        return _autocompleteMappings.TryGetValue(type, value: out autocompleteValue);
    }
}
