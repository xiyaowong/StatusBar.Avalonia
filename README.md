# StatusBar.Avalonia

[![NuGet Version](https://img.shields.io/nuget/v/StatusBar.Avalonia.svg)](https://www.nuget.org/packages/StatusBar.Avalonia)
[![NuGet Downloads](https://img.shields.io/nuget/dt/StatusBar.Avalonia.svg)](https://www.nuget.org/packages/StatusBar.Avalonia)

A status bar control for [AvaloniaUI](https://avaloniaui.net), inspired by the Visual Studio Code status bar, with a
simple and flexible API.

https://github.com/user-attachments/assets/7e88c2bb-a6df-4496-b03e-e1b6c7ee99fc

---

## Getting Started

### 1. Install the NuGet package

```powershell
Install-Package StatusBar.Avalonia
```

### 2. Add the style reference to `App.xaml`

```xaml
<Application ...
        xmlns:status="https://github.com/xiyaowong/StatusBar.Avalonia"
        ...>
    <Application.Styles>
        <status:StatusBarTheme/>
    </Application.Styles>
</Application>
```

### 3. Add the `StatusBarContainer` to your layout

```xml

<status:StatusBarContainer Name="_StatusBarContainer"/>
```

### 4. Create and bind the `StatusBarManager`

```csharp
public StatusBarManager StatusBarManager { get; } = new();

...

StatusBarManager.BindContainer(_StatusBarContainer);
```

### 5. Create and show a status bar item

```csharp
var status = StatusBarManager.CreateStatusBarItem("status");
status.Text = "Hello World";
status.Show();
```

---

## Examples and Usage

* See
  the [demo project](https://github.com/xiyaowong/StatusBar.Avalonia/blob/main/demo/StatusBarDemo/ViewModels/MainViewModel.cs)
  for more examples.
* You can embed icons inside the status text, just like in VS Code.
  See [VS Code Icon Reference](https://code.visualstudio.com/api/references/icons-in-labels#icons-in-labels) for
  reference.

---

## Resources

* [Source Code on GitHub](https://github.com/xiyaowong/StatusBar.Avalonia)

---

## License

MIT