using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace SharedLibrary.Components;

public class Input<TValue> : AWComponentBase where TValue : class
{
    private ElementReference inputElement;

    private TValue? currentValue;

    [Parameter]
    public InputType Type { get; set; } = InputType.Text;

    [Parameter]
    public AutocompleteType Autocomplete { get; set; } = AutocompleteType.Off;

    [Parameter]
    public bool AutoFocus { get; set;} = false;

    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnBlur { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnEnter { get; set; }

    [Parameter]
    public EventCallback<ElementReference> InputRefChanged { get; set; }

    protected override Task OnParametersSetAsync()
    {
        currentValue = Value;
        return base.OnParametersSetAsync();
    } 

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (AutoFocus)
        {
            await inputElement.FocusAsync();
        }
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;
  
        builder.OpenElement(seq++, "input");

        builder.AddAttribute(seq++, "type", Type);
        builder.AddAttribute(seq++, "value", BindConverter.FormatValue(currentValue));
        builder.AddAttribute(seq++, "oninput", EventCallback.Factory.CreateBinder<TValue>(
                    this, __value => currentValue = __value, currentValue));
        builder.AddAttribute(seq++, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, async _ =>
        {
            if (OnBlur != null)
            {
                await ValueChanged.InvokeAsync(currentValue);
                await OnBlur.Invoke(currentValue);
            }
        }));
        builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, async args =>
        {
            if (args.Key == "Enter")
            {
                if (OnEnter != null)
                {
                    await ValueChanged.InvokeAsync(currentValue);
                    await OnEnter.Invoke(currentValue);
                }
            }
        }));


        builder.AddAttribute(seq++, "autocomplete", AutocompleteFactory.GetAutocomplete(Autocomplete));

        // add element reference
        builder.AddElementReferenceCapture(seq++, async capturedRef =>
        {
            inputElement = capturedRef;

            if(InputRefChanged.HasDelegate)
            {
                await InputRefChanged.InvokeAsync(capturedRef);
            }
        });


        builder.CloseElement();
    }
}

public enum InputType
{
    Button,
    Checkbox,
    Color,
    Date,
    DatetimeLocal,
    Email,
    File,
    Hidden,
    Image,
    Month,
    Number,
    Password,
    Radio,
    Range,
    Reset,
    Search,
    Submit,
    Tel,
    Text,
    Time,
    Url,
    Week
}

public enum AutocompleteType
{
    /// <summary>
    /// 浏览器不自动填充输入
    /// </summary>
    Off,

    /// <summary>
    /// 浏览器可以自动填充输入
    /// </summary>
    On,

    /// <summary>
    /// 全名
    /// </summary>
    Name,

    /// <summary>
    /// 名
    /// </summary>
    GivenName,

    /// <summary>
    /// 姓
    /// </summary>
    FamilyName,

    /// <summary>
    /// 中间名
    /// </summary>
    AdditionalName,

    /// <summary>
    /// 昵称
    /// </summary>
    Nickname,

    /// <summary>
    /// 电子邮件地址
    /// </summary>
    Email,

    /// <summary>
    /// 用户名
    /// </summary>
    Username,

    /// <summary>
    /// 新密码
    /// </summary>
    NewPassword,

    /// <summary>
    /// 当前密码
    /// </summary>
    CurrentPassword,

    /// <summary>
    /// 组织/公司名称
    /// </summary>
    Organization,

    /// <summary>
    /// 组织/公司部门
    /// </summary>
    OrganizationTitle,

    /// <summary>
    /// 街道地址
    /// </summary>
    StreetAddress,

    /// <summary>
    /// 地址行1
    /// </summary>
    AddressLine1,

    /// <summary>
    /// 地址行2
    /// </summary>
    AddressLine2,

    /// <summary>
    /// 地址行3
    /// </summary>
    AddressLine3,

    /// <summary>
    /// 国家/地区代码
    /// </summary>
    Country,

    /// <summary>
    /// 国家/地区名称
    /// </summary>
    CountryName,

    /// <summary>
    /// 邮政编码
    /// </summary>
    PostalCode,

    /// <summary>
    /// 完整电话号码
    /// </summary>
    Tel,

    /// <summary>
    /// 区号
    /// </summary>
    TelCountryCode,

    /// <summary>
    /// 电话号码(不含区号)
    /// </summary>
    TelNational,

    /// <summary>
    /// 手机号码
    /// </summary>
    TelMobile,

    /// <summary>
    /// 信用卡持卡人姓名
    /// </summary>
    CcName,

    /// <summary>
    /// 信用卡号码
    /// </summary>
    CcNumber,

    /// <summary>
    /// 信用卡过期月份
    /// </summary>
    CcExpMonth,

    /// <summary>
    /// 信用卡过期年份
    /// </summary>
    CcExpYear,

    /// <summary>
    /// 信用卡安全码
    /// </summary>
    CcCsc,

    /// <summary>
    /// 生日(完整日期)
    /// </summary>
    Bday,

    /// <summary>
    /// 生日-日
    /// </summary>
    BdayDay,

    /// <summary>
    /// 生日-月
    /// </summary>
    BdayMonth,

    /// <summary>
    /// 生日-年
    /// </summary>
    BdayYear,

    /// <summary>
    /// 性别
    /// </summary>
    Sex,

    /// <summary>
    /// 个人照片URL
    /// </summary>
    Photo,

    /// <summary>
    /// 即时通讯账号
    /// </summary>
    Impp,

    /// <summary>
    /// URL/网站地址
    /// </summary>
    Url,

    /// <summary>
    /// 交易金额
    /// </summary>
    TransactionAmount,

    /// <summary>
    /// 交易货币
    /// </summary>
    TransactionCurrency,

    /// <summary>
    /// 语言偏好
    /// </summary>
    Language,

    /// <summary>
    /// 单行文本输入
    /// </summary>
    OneTimeCode
}

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
