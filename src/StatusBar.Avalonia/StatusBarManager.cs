using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using StatusBar.Avalonia.Controls;

namespace StatusBar.Avalonia;

public class StatusBarManager
{
    private readonly ConcurrentQueue<StatusBarEntry> _pendingItems = new();

    private StatusBarContainer? _container;

    public StatusBarManager() { }

    public StatusBarManager(StatusBarContainer container)
    {
        _container = container;
    }

    public void BindContainer(StatusBarContainer statusBarContainer)
    {
        if (_container != null)
        {
            throw new InvalidOperationException($"{nameof(StatusBarManager)} already has a container.");
        }

        _container = statusBarContainer;

        while (_pendingItems.TryDequeue(out var item))
        {
            _container.AddStatusBarEntry(item);
        }
    }

    /// <summary>
    /// Creates a status bar item.
    /// </summary>
    /// <param name="id">The identifier of the item. Must be unique.</param>
    /// <param name="alignment">The alignment of the item.</param>
    /// <param name="priority">The priority of the item. Higher values mean the item should be shown more to the left.</param>
    /// <returns></returns>
    public StatusBarItem CreateStatusBarItem(string id, StatusBarAlignment alignment = default, int priority = 0)
    {
        Dispatcher.UIThread.VerifyAccess();

        var entry = new StatusBarEntry
        {
            Id = id,
            Alignment = alignment,
            Priority = priority,
        };
        AddStatusBarEntry(entry);
        return new StatusBarItem(entry);
    }

    /// <summary>
    /// Set a message to the status bar.
    /// </summary>
    /// <param name="text">The message to show, supports icon substitution.</param>
    /// <param name="hideAfterTimeout">Timeout in milliseconds after which the message will be disposed.</param>
    /// <returns>A disposable which hides the status bar message.</returns>
    public IDisposable SetStatusBarMessage(string text, int hideAfterTimeout)
    {
        Dispatcher.UIThread.VerifyAccess();

        var entry = new StatusBarEntry
        {
            Id = Guid.NewGuid().ToString("B"),
            IsTemporary = true,
            Text = text,
            IsShow = true,
        };
        AddStatusBarEntry(entry);

        var cts = new CancellationTokenSource();

        Task.Run(
            async () =>
            {
                try
                {
                    await Task.Delay(hideAfterTimeout, cts.Token);
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    Dispatcher.UIThread.Post(() => entry.Dispose());
                }
            },
            cts.Token
        );

        return new DisposableAction(() =>
        {
            cts.Cancel();
            Dispatcher.UIThread.Post(() => entry.Dispose());
        });
    }

    /// <summary>
    /// Set a message to the status bar.
    /// </summary>
    /// <param name="text">The message to show, supports icon substitution.</param>
    /// <param name="hideWhenDone">A task which when completed will hide the message.</param>
    /// <returns>A disposable which hides the status bar message.</returns>
    public IDisposable SetStatusBarMessage(string text, Func<Task> hideWhenDone)
    {
        Dispatcher.UIThread.VerifyAccess();

        var entry = new StatusBarEntry
        {
            Id = Guid.NewGuid().ToString("B"),
            IsTemporary = true,
            Text = text,
            IsShow = true,
        };
        AddStatusBarEntry(entry);

        var cts = new CancellationTokenSource();

        Task.Run(
            async () =>
            {
                try
                {
                    await hideWhenDone();
                }
                finally
                {
                    Dispatcher.UIThread.Post(() => entry.Dispose());
                }
            },
            cts.Token
        );

        return new DisposableAction(() =>
        {
            cts.Cancel();
            Dispatcher.UIThread.Post(() => entry.Dispose());
        });
    }

    private void AddStatusBarEntry(StatusBarEntry entry)
    {
        if (_container == null)
        {
            _pendingItems.Enqueue(entry);
            return;
        }

        while (_pendingItems.TryDequeue(out var item))
        {
            _container.AddStatusBarEntry(item);
        }

        _container.AddStatusBarEntry(entry);
    }
}

internal class DisposableAction(Action action) : IDisposable
{
    public void Dispose()
    {
        action();
    }
}
