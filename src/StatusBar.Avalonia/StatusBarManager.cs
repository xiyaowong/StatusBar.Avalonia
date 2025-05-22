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

    /// <summary>
    /// Initializes a new instance of the StatusBarManager class.
    /// </summary>
    public StatusBarManager() { }

    /// <summary>
    /// Binds a StatusBarContainer to this manager. Can only be called once.
    /// </summary>
    /// <param name="container">The StatusBarContainer to bind.</param>
    /// <exception cref="InvalidOperationException">Thrown when a container is already bound to this manager.</exception>
    public StatusBarManager(StatusBarContainer container)
    {
        _container = container;
    }

    /// <summary>
    /// Binds a StatusBarContainer to this manager. Can only be called once.
    /// </summary>
    /// <param name="statusBarContainer">The StatusBarContainer to bind.</param>
    /// <exception cref="InvalidOperationException">Thrown when a container is already bound to this manager.</exception>
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
    /// Creates a new status bar item with specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the status bar item.</param>
    /// <param name="alignment">The alignment of the item within the status bar.</param>
    /// <param name="priority">The priority of the item. Higher values position the item more to the left. Defaults to 0.</param>
    /// <returns>A new StatusBarItem instance.</returns>
    /// <remarks>This method must be called from the UI thread.</remarks>
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
    /// Sets a temporary message in the status bar that automatically hides after a specified timeout.
    /// </summary>
    /// <param name="text">The message text to display. Supports icon substitution.</param>
    /// <param name="hideAfterTimeout">The duration in milliseconds after which the message will be hidden.</param>
    /// <returns>An IDisposable object that can be used to manually hide the message before the timeout.</returns>
    /// <remarks>This method must be called from the UI thread.</remarks>
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
    /// Sets a temporary message in the status bar that hides when a specified task completes.
    /// </summary>
    /// <param name="text">The message text to display. Supports icon substitution.</param>
    /// <param name="hideWhenDone">A function that returns a Task. The message will be hidden when this task completes.</param>
    /// <returns>An IDisposable object that can be used to manually hide the message before the task completes.</returns>
    /// <remarks>This method must be called from the UI thread.</remarks>
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
