# AWComponent - Blazor 自定义组件库

![.NET Version](https://img.shields.io/badge/.NET-6.0-blueviolet)
[![Blazor Version](https://img.shields.io/badge/Blazor-WebAssembly%20.NET%206.0-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE.txt)
[![EN](https://img.shields.io/badge/Language-English-blue)](README.en-US.md)
[![CN](https://img.shields.io/badge/语言-中文-red)](README.md)

🌐 **Choose Language**: 
[English](README.en-US.md) | [中文](README.md) 

基于 **Blazor (.NET 6.0)** 的高质量自定义组件库，提供可复用的 UI 组件和模块化解决方案。

---

## 🚀 功能特性

- **开箱即用**：预置多种企业级 UI 组件
- **主题定制**：通过 CSS 变量轻松自定义样式
- **高性能**：基于 .NET 6.0 优化渲染逻辑
- **完整文档**：内置示例页面和代码片段

---

## 📦 快速开始

### 前置条件
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- IDE: Visual Studio 2022

### 安装与运行

1. **克隆仓库**：

   ```bash
   git clone https://github.com/your-username/AWComponent.git
   ```

2. **运行程序**:

    ```bash
    cd AWComponent/AW/Server
    dotnet run
    ```

    访问`https://localhost:7208`查看运行效果

## 📂 项目架构

```text
AWComponent/
├── SharedLibrary/         # 共享组件库
│   ├── Components/        # 可复用组件
│   └── ...
│
├── AW.Client/...
├── AW.Server/...
├── AW.Shared/...
└── AWComponent.sln        # 解决方案文件
```

## 🔧 组件使用示例

```csharp
@using SharedLibrary.Components

<Button OnClick="HandleClick">PlaceHolder</Button>

@code{
    private void HandleClick(MouseEventArgs args)
    {
        // add your code
    }
}
```

## NuGet 包集成

1. 安装 AWComponent NuGet 包：

   ```bash
   dotnet add package AWComponent --version 0.0.1-beta
   ```

2. 服务配置(Program.cs in Client)

    ```csharp
    builder.AddAWComponentServices();
    ```

3. 全局配置(appsettings.json in Client.wwwroot)

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

4. _Imports.razor 中添加命名空间：

    ```csharp
    @using AWComponent.Components
    ```

## 🧩 支持组件

|组件类型|组件名|
|---|---|
|基础组件|Alert、Button、Divider、Input、Label、Select、Modal|

## 📚 文档资源

- [Blazor 官方文档](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-6.0)
