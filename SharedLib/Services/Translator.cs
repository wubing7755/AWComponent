using System.Text;
using Newtonsoft.Json.Linq;

public class Translator
{
    // Google 翻译页面 URL
    private const string GoogleTranslateUrl = "https://translate.googleapis.com/translate_a/single";
    // 浏览器 User-Agent
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

    public static async Task<string> TranslateAsync(string text)
    {
        try
        {
            // 自动检测语言：中文还是英文
            bool isChinese = IsChinese(text);

            string fromLanguage = isChinese ? "zh-CN" : "en";
            string toLanguage = isChinese ? "en" : "zh-CN";

            // 构建带参数的请求URL
            var queryParams = new Dictionary<string, string>
            {
                { "client", "gtx" },    // 客户端类型
                { "sl", fromLanguage }, // 源语言代码
                { "tl", toLanguage },   // 目标语言代码
                { "dt", "t" },          // 指定返回翻译文本
                { "q", text }           // 需要翻译的文本
            };

            var uriBuilder = new UriBuilder(GoogleTranslateUrl)
            {
                Query = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync()
            };

            using (var client = new HttpClient())
            {
                // 设置 User-Agent
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

                // 发送GET请求
                HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // 解析JSON响应
                JArray jsonResponse = JArray.Parse(responseBody);
                StringBuilder result = new StringBuilder();

                // 提取翻译结果（兼容多段落文本）
                if (jsonResponse.Count > 0 && jsonResponse[0] is JArray translations)
                {
                    foreach (JToken segment in translations)
                    {
                        if (segment.Count() > 0 && segment[0] is JToken translatedSegment)
                        {
                            result.Append(translatedSegment.ToString());
                        }
                    }
                }

                return result.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"translate error: {ex.Message}");

            return string.Empty;
        }
    }

    private static bool IsChinese(string text)
    {
        /**
         * 中文Unicode范围：
         * 常用汉字：     0x4E00 - 0x9FFF
         * 扩展A区汉字：  0x3400 - 0x4DBF
         * 扩展B区汉字：  0x20000 - 0x2A6DF
         * 
         */
        foreach (char c in text)
        {
            if (c >= 0x4E00 && c <= 0x9FFF)
            {
                return true;
            }
        }

        return false;
    }
}
