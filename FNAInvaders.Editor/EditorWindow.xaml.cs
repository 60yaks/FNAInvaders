using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;

namespace FNAInvaders.Editor;

public partial class EditorWindow : Window
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll")]
    private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    private const uint MF_BYCOMMAND = 0x00000000;
    private const uint MF_GRAYED = 0x00000001;

    private const uint SC_CLOSE = 0xF060;

    private const int WM_SHOWWINDOW = 0x00000018;
    private const int WM_CLOSE = 0x10;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            hwndSource.AddHook(WndProc);
        }
    }
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_SHOWWINDOW)
        {
            var hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
        }
        else if (msg == WM_CLOSE)
        {
            handled = true;
        }
        return IntPtr.Zero;
    }

    public static CancellationTokenSource StartEditor(World world, ISystem<GameTime> mainSystem, int left, int top)
    {
        var cts = new CancellationTokenSource();
        var thread = new Thread(() =>
        {
            var app = new Application();
            cts.Token.Register(() => app.Dispatcher.InvokeShutdown());
            var window = new EditorWindow();
            window.InitializeComponent();
            window.Top = top;
            window.Left = left;
            window.DataContext = new EditorViewModel(world, mainSystem);
            app.Run(window);
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return cts;
    }
}
