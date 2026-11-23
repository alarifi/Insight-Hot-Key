using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InsightHotKey.Localization;
using InsightHotKey.Models;
using InsightHotKey.Services;
using InsightHotKey.Views;

namespace InsightHotKey
{
    public partial class App : Application
    {
        private const string MutexName = "InsightHotKey_SingleInstance_Mutex_2026";
        private Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains("--capture-ui"))
            {
                base.OnStartup(e);
                int idx = Array.IndexOf(e.Args, "--capture-ui");
                string outPath = idx >= 0 && idx + 1 < e.Args.Length ? e.Args[idx + 1] : "preview_test.png";

                try
                {
                    var mainWindow = new MainWindow();
                    mainWindow.PrepareVisualForCapture();

                    var root = (FrameworkElement)mainWindow.Content;
                    root.Width = 920;
                    root.Height = 540;
                    root.Measure(new Size(920, 540));
                    root.Arrange(new Rect(0, 0, 920, 540));
                    root.UpdateLayout();

                    var rtb = new RenderTargetBitmap(920, 540, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(root);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using var fs = File.Create(outPath);
                    encoder.Save(fs);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(outPath + ".err.txt", ex.ToString(), Encoding.UTF8);
                }

                Shutdown();
                return;
            }

            if (e.Args.Contains("--capture-edit-ui"))
            {
                base.OnStartup(e);
                int idx = Array.IndexOf(e.Args, "--capture-edit-ui");
                string outPath = idx >= 0 && idx + 1 < e.Args.Length ? e.Args[idx + 1] : "preview_edit.png";

                try
                {
                    var settings = StorageService.LoadSettings();
                    var sampleItem = settings.Items.FirstOrDefault(x => !x.IsEmpty) ?? new HotkeyItem
                    {
                        Title = "متصفح Edge",
                        ItemType = HotkeyType.Application,
                        TargetPath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                        KeyDisplay = "ALT + 1"
                    };

                    var editWindow = new EditItemWindow(sampleItem, settings);
                    var root = (FrameworkElement)editWindow.Content;
                    root.Width = 530;
                    root.Height = 660;
                    root.Measure(new Size(530, 660));
                    root.Arrange(new Rect(0, 0, 530, 660));
                    root.UpdateLayout();

                    var rtb = new RenderTargetBitmap(530, 660, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(root);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using var fs = File.Create(outPath);
                    encoder.Save(fs);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(outPath + ".err.txt", ex.ToString(), Encoding.UTF8);
                }

                Shutdown();
                return;
            }

            if (e.Args.Contains("--capture-settings-ui"))
            {
                base.OnStartup(e);
                int idx = Array.IndexOf(e.Args, "--capture-settings-ui");
                string outPath = idx >= 0 && idx + 1 < e.Args.Length ? e.Args[idx + 1] : "preview_settings.png";

                try
                {
                    var settings = StorageService.LoadSettings();
                    var settingsWindow = new SettingsWindow(settings);
                    var root = (FrameworkElement)settingsWindow.Content;
                    root.Width = 510;
                    root.Height = 680;
                    root.Measure(new Size(510, 680));
                    root.Arrange(new Rect(0, 0, 510, 680));
                    root.UpdateLayout();

                    var rtb = new RenderTargetBitmap(510, 680, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(root);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using var fs = File.Create(outPath);
                    encoder.Save(fs);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(outPath + ".err.txt", ex.ToString(), Encoding.UTF8);
                }

                Shutdown();
                return;
            }

            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                ActivateExistingInstance();
                Shutdown();
                return;
            }

            base.OnStartup(e);

            var main = new MainWindow();

            if (e.Args.Contains("--minimized"))
            {
                main.WindowState = WindowState.Minimized;
                main.Hide();
            }
            else
            {
                main.Show();
            }
        }

        private void ActivateExistingInstance()
        {
            try
            {
                var current = Process.GetCurrentProcess();
                var existing = Process.GetProcessesByName(current.ProcessName)
                    .FirstOrDefault(p => p.Id != current.Id);

                if (existing != null && existing.MainWindowHandle != IntPtr.Zero)
                {
                    WindowManager.ForceForegroundWindow(existing.MainWindowHandle);
                }
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
