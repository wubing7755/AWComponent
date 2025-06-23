# AWUI - Blazor 自定义组件库

![.NET Version](https://img.shields.io/badge/.NET-9.0-blueviolet)
[![Blazor Version](https://img.shields.io/badge/Blazor-WebAssembly%20.NET%209.0-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE.txt)
[![EN](https://img.shields.io/badge/Language-English-blue)](README.en-US.md)
[![CN](https://img.shields.io/badge/语言-中文-red)](README.md)

🌐 **Choose Language**: 
[English](README.en-US.md) | [中文](README.md) 

基于 **Blazor (.NET 9.0)** 的高质量自定义组件库，提供可复用的 UI 组件和模块化解决方案。

---

## 🚀 功能特性

- **开箱即用**：预置多种企业级 UI 组件
- **主题定制**：通过 CSS 变量轻松自定义样式
- **高性能**：基于 .NET 9.0 优化渲染逻辑
- **完整文档**：内置示例页面和代码片段

---

## 📦 快速开始

### 前置条件
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- IDE: Visual Studio 2022

### 安装与运行

1. **克隆仓库**：

   ```bash
   git clone https://github.com/wubing7755/AWComponent.git
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
    ├── AWUI/                  # 组件库
    │   ├── Components/        # 可复用组件
    │   └── ...
    │
    ├── AW
    └── AWComponent.sln        # 解决方案文件
    ```

## 🔧 组件使用示例

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

## NuGet 包集成

1. 安装 AWUI NuGet 包：

   ```bash
   dotnet add package AWUI --version 9.0.1
   ```

2. 服务配置(Program.cs in Client)

    ```csharp
    builder.AddClientServices();
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

4. 样式配置(index.html in Client.wwwroot)

    ```html
    <link href="_content/AWUI/css/AWUI.css" rel="stylesheet" />
    ```

5. _Imports.razor 中添加命名空间：

    ```csharp
    @using AWUI.Components
    ```

## 🧩 支持组件

|组件类型|组件名|
|---|---|
|Basic|Alert、Button、Divider、Input、Label、Modal、Progress、Select、Tree|

## 📚 文档资源

- [Blazor 官方文档](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-9.0)
