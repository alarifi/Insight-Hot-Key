using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using InsightHotKey.Models;

namespace InsightHotKey.Services
{
    public class HotkeyEngine : IDisposable
    {
        private HwndSource? _source;
        private IntPtr _windowHandle = IntPtr.Zero;
        private int _currentId = 1000;
        private readonly Dictionary<int, Action> _callbacks = new();
        private bool _disposed = false;

        public event Action<string>? HotkeyConflict;

        public void Initialize(Window window)
        {
            var helper = new WindowInteropHelper(window);
            _windowHandle = helper.EnsureHandle();
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
        }

        public int Register(KeyModifiers modifiers, uint virtualKey, Action callback, string displayName = "")
        {
            if (_windowHandle == IntPtr.Zero || virtualKey == 0) return 0;

            _currentId++;
            int id = _currentId;

            // Combine with NoRepeat flag (0x4000) to prevent repeating when held
            uint mod = (uint)(modifiers | KeyModifiers.NoRepeat);

            bool success = Win32Api.RegisterHotKey(_windowHandle, id, mod, virtualKey);
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                string message = $"تعذر تسجيل الاختصار ({displayName}). قد يكون مستخدماً بواسطة نظام ويندوز أو تطبيق آخر.";
                HotkeyConflict?.Invoke(message);
                return 0;
            }

            _callbacks[id] = callback;
            return id;
        }

        public bool Unregister(int id)
        {
            if (id <= 0 || _windowHandle == IntPtr.Zero) return false;

            _callbacks.Remove(id);
            return Win32Api.UnregisterHotKey(_windowHandle, id);
        }

        public void UnregisterAll()
        {
            if (_windowHandle == IntPtr.Zero) return;

            foreach (var id in _callbacks.Keys)
            {
                Win32Api.UnregisterHotKey(_windowHandle, id);
            }
            _callbacks.Clear();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32Api.WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (_callbacks.TryGetValue(id, out var callback))
                {
                    callback.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_disposed) return;
            UnregisterAll();
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source.Dispose();
            }
            _disposed = true;
        }
    }
}
