# AWComponent - Blazor Component Library

![.NET Version](https://img.shields.io/badge/.NET-6.0-blueviolet)
[![Blazor Version](https://img.shields.io/badge/Blazor-WebAssembly%20.NET%206.0-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE.txt)

Enterprise-grade UI components for **Blazor (.NET 6.0)** with modern design and high performance

---

## 🚀 Key Features

- **Production-Ready**: 20+ enterprise UI components
- **Theming**: Dynamic skinning via CSS variables
- **Optimized Performance**: Enhanced virtual DOM rendering
- **i18n Support**: Built-in EN/CN localization
- **Type Safety**: Strongly-typed C# components

---

## 📦 Quick Start

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- IDE: Visual Studio 2022

## 🔧 Component Demo

```csharp
@using AWUI.Components

<Button OnClick="HandleClick">PlaceHolder</Button>

@code{
    private void HandleClick(MouseEventArgs args)
    {
        // add your code
    }
}
```

## 📚 NuGet Package Integration

1. Install the AWUI NuGet package

   ```bash
   dotnet add package AWUI --version 0.0.4
   ```

2. Service Configuration(Program.cs in Client)

    ```csharp
    builder.AddClientServices();
    ```

3. Global Configuration(appsettings.json in Client.wwwroot)

    ```json
    {
      "JsModules": {
        "Modules": [
          {
            "Name": "AWUI",
            "Path": "./_content/AWUI/js/AWUI.js",
            "Enable": true
          },
        ]
      }
    }
    ```

4. CSS Configuration(index.html in Client)
    ```html
    <link href="_content/AWUI/css/AWUI.css" rel="stylesheet" />
    ``` 

5. Add namespace in _Imports.razor：

    ```csharp
    @using AWUI.Components
    ```

## 🧩 Component List

|Category|Components|
|---|---|
|Basic|Alert、Button、Divider、Input、Label、Select、 SelectTree、Modal|
