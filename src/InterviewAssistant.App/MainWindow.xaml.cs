using InterviewAssistant.Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;
using WinRT.Interop;

namespace InterviewAssistant.App;

public sealed partial class MainWindow : Window
{
    private readonly IOverlayService _overlayService;
    private readonly IHotkeyService _hotkeyService;
    private IntPtr _hwnd;

    public MainWindow()
    {
        InitializeComponent();

        _overlayService = App.Services.GetRequiredService<IOverlayService>();
        _hotkeyService = App.Services.GetRequiredService<IHotkeyService>();

        _hotkeyService.AltPressed += OnAltPressed;
        _hotkeyService.AltReleased += OnAltReleased;
        _hotkeyService.LeftMouseDown += OnLeftMouseDown;

        _hotkeyService.Start();

        Activated += OnActivated;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        Activated -= OnActivated;

        _hwnd = WindowNative.GetWindowHandle(this);
        _overlayService.Initialize(_hwnd);
    }

    private void OnAltPressed()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _overlayService.EnableInteraction();
        });
        Debug.WriteLine("ALT DOWN");
    }

    private void OnAltReleased()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _overlayService.DisableInteraction();
        });
        Debug.WriteLine("ALT UP");
    }

    private void OnLeftMouseDown()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _overlayService.TryBeginDrag();
        });
    }
}

