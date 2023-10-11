using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace MortalKombatOverlay;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const uint EVENT_OBJECT_CREATE = 0x8000;
    private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
    private const int MA_NOACTIVATE = 3;
    private const uint S_OK = 0;
    private const uint WINEVENT_OUTOFCONTEXT = 0;

    private const int WM_MOUSEACTIVATE = 0x0021;

    private static WinEventDelegate _winEventDelegate;

    private readonly string _windowTitle = "Mortal Kombat™ 1  ";

    private readonly IntPtr _winEventHook;

    private readonly Lazy<float> _dpiFactorX;
    private readonly Lazy<float> _dpiFactorY;
    private IntPtr _mkWindowHandle = IntPtr.Zero;
    private bool _needsUpdate;

    private readonly DispatcherTimer _updateTimer;

    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new GameWatcherViewModel();
        viewModel.StartWatching(_windowTitle);
        DataContext = viewModel;

        _dpiFactorX = new Lazy<float>(() => GetDpiX(_mkWindowHandle));
        _dpiFactorY = new Lazy<float>(() => GetDpiY(_mkWindowHandle));

        _winEventDelegate = WinEventCallback; // Assign the delegate
        _winEventHook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero,
            _winEventDelegate,
            0, 0, WINEVENT_OUTOFCONTEXT);

        _updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(0), DispatcherPriority.Input, UpdateTimer_Tick,
            Dispatcher.CurrentDispatcher);
    }

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    // todo: move this to a helper class
    public Rectangle GetGameRect(bool isDpiAware = true)
    {
        var mkWindowHandle = _mkWindowHandle;
        var rect = new Rect();
        var ptUL = new Point();
        var ptLR = new Point();

        GetClientRect(mkWindowHandle, out rect);

        ptUL.X = rect.Left;
        ptUL.Y = rect.Top;

        ptLR.X = rect.Right;
        ptLR.Y = rect.Bottom;

        ClientToScreen(mkWindowHandle, ref ptUL);
        ClientToScreen(mkWindowHandle, ref ptLR);

        var dpiScalingX = isDpiAware ? GetDpiFactorX() : 1;
        var dpiScalingY = isDpiAware ? GetDpiFactorY() : 1;


        ptUL.X = (int)(ptUL.X / dpiScalingX);
        ptUL.Y = (int)(ptUL.Y / dpiScalingY);
        ptLR.X = (int)(ptLR.X / dpiScalingX);
        ptLR.Y = (int)(ptLR.Y / dpiScalingY);


        return new Rectangle(ptUL.X, ptUL.Y, ptLR.X - ptUL.X, ptLR.Y - ptUL.Y);
    }


    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _updateTimer?.Stop();
        if (_winEventHook != IntPtr.Zero) UnhookWinEvent(_winEventHook);
    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        (DataContext as GameWatcherViewModel)?.SetGameForeground();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private float GetDpiFactorX()
    {
        return _dpiFactorX.Value;
    }

    private float GetDpiFactorY()
    {
        return _dpiFactorY.Value;
    }

    // todo: move this to a helper class
    private static float GetDpiX(IntPtr hwnd)
    {
        using (var graphics = Graphics.FromHwnd(hwnd))
        {
            return graphics.DpiX / 96.0f;
        }
    }

    private static float GetDpiY(IntPtr hwnd)
    {
        using (var graphics = Graphics.FromHwnd(hwnd))
        {
            return graphics.DpiY / 96.0f;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(IntPtr hWnd);


    private void SetWindowPositionAndSize()
    {
        try
        {
            // Define the Mortal Kombat window title or other means to locate it
            var mkWindowTitle = _windowTitle;

            // If the window handle is not cached or the cached window is no longer valid, find it again
            if (_mkWindowHandle == IntPtr.Zero || !IsWindow(_mkWindowHandle))
                _mkWindowHandle = FindWindow(null, mkWindowTitle);

            if (_mkWindowHandle != IntPtr.Zero)
            {
                // if window is minimized, don't update the overlay
                if (IsIconic(_mkWindowHandle)) return;

                var mkWindowRect = GetGameRect();

                // Calculate the width and height
                var width = mkWindowRect.Right - mkWindowRect.Left;
                var height = mkWindowRect.Bottom - mkWindowRect.Top;

                Left = mkWindowRect.Left;
                Top = mkWindowRect.Top;
                Width = width;
                Height = height;
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private void UpdateTimer_Tick(object sender, EventArgs e)
    {
        if (_needsUpdate)
        {
            SetWindowPositionAndSize();
            _needsUpdate = false;
        }

        _updateTimer.Stop();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            var viewModel = DataContext as GameWatcherViewModel;
            if (viewModel != null && !viewModel.IsMortalKombatRunning)
                DragMove();
        }
    }

    private void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        if (Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished) return;

        if (eventType != EVENT_OBJECT_CREATE && eventType != EVENT_OBJECT_LOCATIONCHANGE) return;

        _needsUpdate = true;
        _updateTimer.Start();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);
}