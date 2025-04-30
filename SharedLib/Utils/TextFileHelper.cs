using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace SharedLibrary.Utils;

public class TextFileHelper
{
    public async Task DecodingText(IBrowserFile browserFile)
    {
        //var stream = browserFile.OpenReadStream();
        //using var reader = new StreamReader(stream, Encoding.UTF8); // 假设文件是 UTF-8 编码
        //var text = await reader.ReadToEndAsync();
        //Console.WriteLine(text);
    }
}
