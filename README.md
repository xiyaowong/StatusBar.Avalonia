# StatusBar.Avalonia

[![NuGet Version](https://img.shields.io/nuget/v/StatusBar.Avalonia.svg)](https://www.nuget.org/packages/StatusBar.Avalonia)
[![NuGet Downloads](https://img.shields.io/nuget/dt/StatusBar.Avalonia.svg)](https://www.nuget.org/packages/StatusBar.Avalonia)

A status bar control for [AvaloniaUI](https://avaloniaui.net), inspired by the Visual Studio Code status bar.
It features a simple and flexible API.

![overview](https://github.com/user-attachments/assets/fe224d00-cd77-4764-955e-24ad3cfbb0b3)

---

## Features

- Embed icons directly in the status text. Refer to
  the [VS Code Icon Reference](https://code.visualstudio.com/api/references/icons-in-labels#icons-in-labels).

- Support for custom content
  ![custom content](https://github.com/user-attachments/assets/530cea1c-ee4f-4970-9341-586711bf83d2)

- Built-in context menu for enabling/disabling status bar items
  ![contextmenu](https://github.com/user-attachments/assets/0f30161b-1a65-494d-9010-f9d970544259)

- Built-in color themes
  ![color theme](https://github.com/user-attachments/assets/c0b361bc-1cb9-42a2-a4bf-dace58a79cee)

- Simple click event handling

  ![click-action](https://github.com/user-attachments/assets/86b045f6-479b-477b-90a2-5e251d3919c1)

- Temporarily display a status bar item
  ![temporary message](https://github.com/user-attachments/assets/ce8ff890-3447-4d84-b50e-27b3de0ae337)

## Getting Started

### 1. Install via NuGet

```powershell
Install-Package StatusBar.Avalonia
```

### 2. Reference the style in `App.axaml`

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

```xaml
<status:StatusBarContainer Name="_StatusBarContainer"/>
```

### 4. Create and bind the `StatusBarManager`

```csharp
public StatusBarManager StatusBarManager { get; } = new();

...

StatusBarManager.BindContainer(_StatusBarContainer);
```

### 5. Create and display a status bar item

```csharp
var status = StatusBarManager.CreateStatusBarItem("status");
status.Text = "Hello World";
status.Show();
```

---

## Usage

### Embedding icons in text

```csharp
status.Text = "$(git-branch) master"; // a git branch icon
status.Text = "$(sync~spin) Syncing..."; // a spinning sync icon
```

### Custom content

```csharp
status.Content = new TextBlock
{
    Text = "Hello World",
    FontStyle = FontStyle.Italic,
};
```

### Context menu

Enabled by default. To disable it, set the `DisableDefaultContextMenu` property of `StatusBarContainer` to `true`.

The `DisabledItems` property on `StatusBarContainer` contains the list of disabled items.

### Click action

```csharp
status.Click = () =>
{
    // Do something when the status bar item is clicked
};
```

### Temporary messages

```csharp
// show 'Hello world!' for 3 seconds
status.SetStatusBarMessage("Hello world!", 3000);
// show 'Hello world!' until the action is completed
status.SetStatusBarMessage("Hello world!", async () =>
{
    // Do something
});
```

### Theming

### Built-in themes

- `DarkPlus`
- `OneDark`
- `GithubDefault`

Set the `ColorTheme` property of `StatusBarContainer` to apply a theme.

### Customizable colors

- `StatusBarBackground` - the background color of the status bar
- `StatusBarForeground` - the foreground color of the status bar
- `StatusBarItemHoverBackground` - the background color of the status bar item when hovered
- `StatusBarItemHoverForeground` - the foreground color of the status bar item when hovered
- `StatusBarItemActiveBackground` - the background color of the status bar item when active (pressed)

## Demo

* See
  the [demo project](https://github.com/xiyaowong/StatusBar.Avalonia/blob/main/demo/StatusBarDemo/ViewModels/MainViewModel.cs)
  for more examples.

---

## Resources

* [Source Code on GitHub](https://github.com/xiyaowong/StatusBar.Avalonia)

---

## License

MIT
