using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StatusBar.Avalonia;
using StatusBar.Avalonia.Themes;

namespace StatusBarDemo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public StatusBarManager StatusBarManager { get; } = new();

    private readonly StatusBarItem _modeItem;
    private readonly StatusBarItem _encodingItem;
    private readonly StatusBarItem _cursorItem;
    private readonly StatusBarItem _filetypeItem;
    private readonly StatusBarItem _lineBreakItem;
    private readonly StatusBarItem _gitBranchItem;
    private readonly StatusBarItem _gitStatusItem;
    private readonly StatusBarItem _loginItem;
    private readonly StatusBarItem _logoItem;
    private readonly StatusBarItem _counterItem;

    [ObservableProperty]
    private AvaloniaList<string> _disabledItems;

    [ObservableProperty]
    private bool _disableContextMenu;

    public MainViewModel()
    {
        DisabledItems = [];

        _modeItem = StatusBarManager.CreateStatusBarItem("mode");
        _gitBranchItem = StatusBarManager.CreateStatusBarItem("git-branch");
        _gitStatusItem = StatusBarManager.CreateStatusBarItem("git-status");

        _counterItem = StatusBarManager.CreateStatusBarItem("counter", StatusBarAlignment.Center);

        _loginItem = StatusBarManager.CreateStatusBarItem("login", StatusBarAlignment.Right);
        _logoItem = StatusBarManager.CreateStatusBarItem("logo", StatusBarAlignment.Right);
        _cursorItem = StatusBarManager.CreateStatusBarItem("cursor", StatusBarAlignment.Right);
        _lineBreakItem = StatusBarManager.CreateStatusBarItem("line-break", StatusBarAlignment.Right);
        _encodingItem = StatusBarManager.CreateStatusBarItem("encoding", StatusBarAlignment.Right);
        _filetypeItem = StatusBarManager.CreateStatusBarItem("filetype", StatusBarAlignment.Right);

        InitModeItem();
        InitGitItem();
        InitCursorItem();
        InitLineBreakItem();
        InitEncodingItem();
        InitFiletypeItem();
        InitLoginItem();
        InitLogoItem();
        InitCounterItem();
    }

    private void InitCounterItem()
    {
        _counterItem.Text = "0";
        _counterItem.ToolTip = "Click above buttons to change the counter value";
        _counterItem.Show();
    }

    private void InitLogoItem()
    {
        var image = new Image
        {
            Source = new Bitmap(AssetLoader.Open(new Uri("avares://StatusBarDemo/Assets/avalonia-logo.ico"))),
            Width = 18,
            Height = 18,
        };

        var showAboutPopupCommand = new RelayCommand(() =>
        {
            var flyout = new Flyout()
            {
                Content = new TextBlock()
                {
                    Text = "Avalonia StatusBar Demo",
                    FontSize = 18,
                    FontWeight = FontWeight.Bold,
                },
                ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway,
            };
            flyout.ShowAt(image, true);
        });

        image.ContextMenu = new ContextMenu
        {
            Items =
            {
                new MenuItem { Header = "_About", Command = showAboutPopupCommand },
            },
        };

        _logoItem.Content = image;
        _logoItem.Show();
    }

    private void InitLoginItem()
    {
        _loginItem.Text = "$(account) Login";
        _loginItem.ToolTip = "Login to your account";
        _loginItem.Click = async void () =>
        {
            try
            {
                StatusBarManager.SetStatusBarMessage("$(loading~spin) Logging in...", 3000);
                _loginItem.Hide();
                await Task.Delay(3100);
                _loginItem.Show();
            }
            catch
            {
                // ignored
            }
        };
        _loginItem.Show();
    }

    private void InitModeItem()
    {
        _modeItem.Color = Brushes.Black;
        _modeItem.Click = () =>
            setMode(
                _modeItem.Text switch
                {
                    "Normal" => "Insert",
                    "Insert" => "Visual",
                    _ => "Normal",
                }
            );
        setMode("Normal");
        _modeItem.Show();

        return;

        void setMode(string mode)
        {
            _modeItem.Text = mode;
            _modeItem.BackgroundColor = mode switch
            {
                "Normal" => Brushes.MediumSeaGreen,
                "Insert" => Brushes.DodgerBlue,
                "Visual" => Brushes.OrangeRed,
                _ => _modeItem.BackgroundColor,
            };
        }
    }

    private void InitGitItem()
    {
        _gitBranchItem.Text = "$(git-branch) master";
        _gitBranchItem.ToolTip = "Current branch";
        _gitBranchItem.Click = () =>
        {
            _gitBranchItem.Text =
                _gitBranchItem.Text == "$(git-branch) master" ? "$(git-branch) develop" : "$(git-branch) master";
        };
        _gitBranchItem.Show();

        _gitStatusItem.Text = "↑ 1 ↓ 0 ! 0";
        _gitStatusItem.ToolTip = "Sync with remote";
        _gitStatusItem.Click = () =>
        {
            if (_gitStatusItem.Text == "$(sync~spin) ↑ 1 ↓ 0 ! 0")
                return;

            _gitStatusItem.Text = "$(sync~spin) ↑ 1 ↓ 0 ! 0";
            Task.Delay(2000).ContinueWith(_ => _gitStatusItem.Text = "↑ 1 ↓ 0 ! 0");
        };
        _gitStatusItem.Show();
    }

    private void InitCursorItem()
    {
        _cursorItem.Text = "1,1";
        _cursorItem.Show();
    }

    private void InitLineBreakItem()
    {
        _lineBreakItem.Text = "LF";
        _lineBreakItem.Show();
    }

    private void InitEncodingItem()
    {
        _encodingItem.Text = "UTF-8";
        _encodingItem.Show();
    }

    private void InitFiletypeItem()
    {
        _filetypeItem.Text = "Markdown";
        _filetypeItem.Show();
    }

    private IDisposable? _currentDateTimeDisposable;

    [RelayCommand]
    private void ShowCurrentDateTime()
    {
        _currentDateTimeDisposable?.Dispose();
        _currentDateTimeDisposable = StatusBarManager.SetStatusBarMessage(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            3000
        );
    }

    [RelayCommand]
    private void IncreaseCounter()
    {
        _counterItem.Text = (int.Parse(_counterItem.Text) + 1).ToString();
    }

    [RelayCommand]
    private void DecreaseCounter()
    {
        _counterItem.Text = (int.Parse(_counterItem.Text) - 1).ToString();
    }

    [RelayCommand]
    private void ResetCounter()
    {
        _counterItem.Text = "0";
    }

    [RelayCommand]
    private async Task SimulateSettingPythonEnvironment()
    {
        var statusBarItem = StatusBarManager.CreateStatusBarItem("python-env", StatusBarAlignment.Left, -100);
        statusBarItem.Show();
        try
        {
            statusBarItem.Text = "$(gear) Initializing Python environment...";
            await Task.Delay(1000);

            statusBarItem.Text = "$(loading~spin) Checking Python interpreter...";
            await Task.Delay(1500);

            statusBarItem.Text = "$(debug-alt) Loading virtual environment...";
            await Task.Delay(1200);

            statusBarItem.Text = "$(terminal) Configuring terminal environment variables...";
            await Task.Delay(1000);

            statusBarItem.Text = "$(settings) Applying development configuration...";
            await Task.Delay(1000);

            statusBarItem.Text = "$(check) Python development environment is ready";
            await Task.Delay(2000);
        }
        finally
        {
            statusBarItem.Dispose();
        }

        await Task.Delay(100);
    }

    [RelayCommand]
    private void EnableItem(string? itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        DisabledItems.Remove(itemId);
    }
}
