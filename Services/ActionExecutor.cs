using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using InsightHotKey.Models;

namespace InsightHotKey.Services
{
    public static class ActionExecutor
    {
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        public static void Execute(HotkeyItem item, bool smartSwitch = true)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.TargetPath)) return;

            Task.Run(() =>
            {
                try
                {
                    switch (item.ItemType)
                    {
                        case HotkeyType.Application:
                            if (smartSwitch)
                            {
                                WindowManager.ActivateOrSwitchToProcess(item.TargetPath, item.Arguments, item.WorkingDirectory);
                            }
                            else
                            {
                                var psi = new ProcessStartInfo
                                {
                                    FileName = item.TargetPath,
                                    Arguments = item.Arguments,
                                    WorkingDirectory = item.WorkingDirectory,
                                    UseShellExecute = true
                                };
                                Process.Start(psi);
                            }
                            break;

                        case HotkeyType.Folder:
                            if (smartSwitch && WindowManager.ActivateOrSwitchToProcess(item.TargetPath))
                            {
                                return;
                            }
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Arguments = $"\"{item.TargetPath}\"",
                                UseShellExecute = true
                            });
                            break;

                        case HotkeyType.Website:
                            string url = item.TargetPath;
                            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                url = "https://" + url;
                            }
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = url,
                                UseShellExecute = true
                            });
                            break;

                        case HotkeyType.TextSnippet:
                            SendSnippetText(item.TargetPath);
                            break;

                        case HotkeyType.SystemCommand:
                            ExecuteSystemCommand(item.TargetPath);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error executing item {item.Title}: {ex.Message}");
                }
            });
        }

        private static void SendSnippetText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Small delay to allow target window to receive focus if needed
            Thread.Sleep(80);

            var inputs = new Win32Api.INPUT[text.Length * 2];
            int idx = 0;

            foreach (char c in text)
            {
                // Key down
                inputs[idx].type = Win32Api.INPUT_KEYBOARD;
                inputs[idx].u.ki.wVk = 0;
                inputs[idx].u.ki.wScan = c;
                inputs[idx].u.ki.dwFlags = Win32Api.KEYEVENTF_UNICODE;
                idx++;

                // Key up
                inputs[idx].type = Win32Api.INPUT_KEYBOARD;
                inputs[idx].u.ki.wVk = 0;
                inputs[idx].u.ki.wScan = c;
                inputs[idx].u.ki.dwFlags = Win32Api.KEYEVENTF_UNICODE | Win32Api.KEYEVENTF_KEYUP;
                idx++;
            }

            Win32Api.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Win32Api.INPUT)));
        }

        private static void ExecuteSystemCommand(string command)
        {
            string cmd = command.Trim().ToLowerInvariant();
            switch (cmd)
            {
                case "lock":
                case "قفل الجهاز":
                    LockWorkStation();
                    break;

                case "recyclebin":
                case "إفراغ سلة المحذوفات":
                    SHEmptyRecycleBin(IntPtr.Zero, null, 7); // SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND
                    break;

                case "calc":
                case "الحاسبة":
                    Process.Start(new ProcessStartInfo("calc.exe") { UseShellExecute = true });
                    break;

                case "notepad":
                case "المفكرة":
                    Process.Start(new ProcessStartInfo("notepad.exe") { UseShellExecute = true });
                    break;

                case "taskmgr":
                case "مدير المهام":
                    Process.Start(new ProcessStartInfo("taskmgr.exe") { UseShellExecute = true });
                    break;

                case "cmd":
                case "موجه الأوامر":
                    Process.Start(new ProcessStartInfo("cmd.exe") { UseShellExecute = true });
                    break;

                default:
                    Process.Start(new ProcessStartInfo("cmd.exe", $"/c {command}") { UseShellExecute = true, CreateNoWindow = true });
                    break;
            }
        }
    }
}
