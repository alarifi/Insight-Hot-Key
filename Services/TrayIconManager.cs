using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using InsightHotKey.Localization;

namespace InsightHotKey.Services
{
    public class TrayIconManager : IDisposable
    {
        private readonly Window _window;
        private IntPtr _hWnd = IntPtr.Zero;
        private HwndSource? _source;
        private bool _isAdded = false;
        private IntPtr _hIcon = IntPtr.Zero;

        public event Action? ToggleWindowRequested;
        public event Action? SettingsRequested;
        public event Action? ExitRequested;

        public TrayIconManager(Window window)
        {
            _window = window;
        }

        public void Initialize()
        {
            var helper = new WindowInteropHelper(_window);
            _hWnd = helper.EnsureHandle();
            _source = HwndSource.FromHwnd(_hWnd);
            _source.AddHook(HwndHook);

            // Load icon handle
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico");
            if (File.Exists(iconPath))
            {
                _hIcon = Win32Api.LoadImage(IntPtr.Zero, iconPath, Win32Api.IMAGE_ICON, 16, 16, Win32Api.LR_LOADFROMFILE);
            }

            if (_hIcon == IntPtr.Zero)
            {
                // Fallback to extraction from current executable
                string? exe = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(exe))
                {
                    IntPtr[] large = new IntPtr[1];
                    IntPtr[] small = new IntPtr[1];
                    Win32Api.ExtractIconEx(exe, 0, large, small, 1);
                    _hIcon = small[0] != IntPtr.Zero ? small[0] : large[0];
                    if (large[0] != IntPtr.Zero && large[0] != _hIcon) Win32Api.DestroyIcon(large[0]);
                }
            }

            var nid = new Win32Api.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf(typeof(Win32Api.NOTIFYICONDATA)),
                hWnd = _hWnd,
                uID = 1001,
                uFlags = Win32Api.NIF_MESSAGE | Win32Api.NIF_ICON | Win32Api.NIF_TIP,
                uCallbackMessage = Win32Api.WM_TRAYICON,
                hIcon = _hIcon,
                szTip = "Insight Hot Key"
            };

            Win32Api.Shell_NotifyIcon(Win32Api.NIM_ADD, ref nid);
            _isAdded = true;
        }

        public void UpdateTooltip(string text)
        {
            if (!_isAdded || _hWnd == IntPtr.Zero) return;

            var nid = new Win32Api.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf(typeof(Win32Api.NOTIFYICONDATA)),
                hWnd = _hWnd,
                uID = 1001,
                uFlags = Win32Api.NIF_TIP,
                szTip = text.Length > 120 ? text[..120] : text
            };
            Win32Api.Shell_NotifyIcon(Win32Api.NIM_MODIFY, ref nid);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32Api.WM_TRAYICON)
            {
                int mouseMsg = lParam.ToInt32();
                if (mouseMsg == Win32Api.WM_LBUTTONUP || mouseMsg == Win32Api.WM_LBUTTONDBLCLK)
                {
                    ToggleWindowRequested?.Invoke();
                    handled = true;
                }
                else if (mouseMsg == Win32Api.WM_RBUTTONUP)
                {
                    ShowContextMenu();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void ShowContextMenu()
        {
            var menu = new ContextMenu();
            menu.FlowDirection = AppLanguage.IsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            var itemShow = new MenuItem { Header = AppLanguage.Get("TrayShow") };
            itemShow.Click += (s, e) => ToggleWindowRequested?.Invoke();

            var itemSettings = new MenuItem { Header = AppLanguage.Get("TraySettings") };
            itemSettings.Click += (s, e) => SettingsRequested?.Invoke();

            var itemExit = new MenuItem { Header = AppLanguage.Get("TrayExit") };
            itemExit.Click += (s, e) => ExitRequested?.Invoke();

            menu.Items.Add(itemShow);
            menu.Items.Add(itemSettings);
            menu.Items.Add(new Separator());
            menu.Items.Add(itemExit);

            menu.IsOpen = true;
        }

        public void Dispose()
        {
            if (_isAdded && _hWnd != IntPtr.Zero)
            {
                var nid = new Win32Api.NOTIFYICONDATA
                {
                    cbSize = Marshal.SizeOf(typeof(Win32Api.NOTIFYICONDATA)),
                    hWnd = _hWnd,
                    uID = 1001
                };
                Win32Api.Shell_NotifyIcon(Win32Api.NIM_DELETE, ref nid);
                _isAdded = false;
            }

            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
            }

            if (_hIcon != IntPtr.Zero)
            {
                Win32Api.DestroyIcon(_hIcon);
                _hIcon = IntPtr.Zero;
            }
        }
    }
}
