using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace InsightHotKey.Services
{
    public static class WindowManager
    {
        public static bool ActivateOrSwitchToProcess(string targetExecutable, string arguments = "", string workingDir = "")
        {
            if (string.IsNullOrWhiteSpace(targetExecutable)) return false;

            string targetName = Path.GetFileNameWithoutExtension(targetExecutable).ToLowerInvariant();

            // Check if process is already running
            Process[] processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                try
                {
                    if (proc.ProcessName.ToLowerInvariant() == targetName)
                    {
                        IntPtr hWnd = proc.MainWindowHandle;
                        if (hWnd != IntPtr.Zero && Win32Api.IsWindowVisible(hWnd))
                        {
                            ForceForegroundWindow(hWnd);
                            return true;
                        }
                    }
                }
                catch
                {
                    // Ignore processes where access is restricted
                }
            }

            // Also check for open Explorer windows if targeting a folder
            if (Directory.Exists(targetExecutable))
            {
                if (TryActivateExplorerFolder(targetExecutable))
                {
                    return true;
                }
            }

            // If not found or not running, start it
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = targetExecutable,
                    Arguments = arguments,
                    WorkingDirectory = !string.IsNullOrWhiteSpace(workingDir) && Directory.Exists(workingDir)
                        ? workingDir
                        : (File.Exists(targetExecutable) ? Path.GetDirectoryName(targetExecutable) ?? "" : ""),
                    UseShellExecute = true
                };
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start process: {ex.Message}");
                return false;
            }
        }

        public static void ForceForegroundWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return;

            if (Win32Api.IsIconic(hWnd))
            {
                Win32Api.ShowWindow(hWnd, Win32Api.SW_RESTORE);
            }
            else
            {
                Win32Api.ShowWindow(hWnd, Win32Api.SW_SHOW);
            }

            IntPtr fgWindow = Win32Api.GetForegroundWindow();
            uint fgThread = Win32Api.GetWindowThreadProcessId(fgWindow, out _);
            uint curThread = Win32Api.GetCurrentThreadId();

            if (fgThread != curThread)
            {
                Win32Api.AttachThreadInput(curThread, fgThread, true);
                Win32Api.BringWindowToTop(hWnd);
                Win32Api.SetForegroundWindow(hWnd);
                Win32Api.AttachThreadInput(curThread, fgThread, false);
            }
            else
            {
                Win32Api.BringWindowToTop(hWnd);
                Win32Api.SetForegroundWindow(hWnd);
            }
        }

        private static bool TryActivateExplorerFolder(string folderPath)
        {
            string folderName = Path.GetFileName(folderPath.TrimEnd('\\', '/')).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(folderName)) return false;

            bool found = false;
            Win32Api.EnumWindows((hWnd, lParam) =>
            {
                if (!Win32Api.IsWindowVisible(hWnd)) return true;

                var sb = new StringBuilder(256);
                Win32Api.GetWindowText(hWnd, sb, 256);
                string title = sb.ToString().ToLowerInvariant();

                if (title.Contains(folderName))
                {
                    Win32Api.GetWindowThreadProcessId(hWnd, out uint pid);
                    try
                    {
                        var proc = Process.GetProcessById((int)pid);
                        if (proc.ProcessName.ToLowerInvariant() == "explorer")
                        {
                            ForceForegroundWindow(hWnd);
                            found = true;
                            return false; // Stop enumeration
                        }
                    }
                    catch { }
                }
                return true;
            }, IntPtr.Zero);

            return found;
        }
    }
}
