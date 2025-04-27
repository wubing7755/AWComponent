using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Options;

public class JsModuleOptions
{
    public const string SectionName = "JsModules";

    public string SharedLib { get; set; } = string.Empty;
}
