using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InsightHotKey.Models;

namespace InsightHotKey.Services
{
    public static class ShellIconService
    {
        public static BitmapSource? GetIconForTarget(string path, HotkeyType type)
        {
            try
            {
                if (type == HotkeyType.Folder || Directory.Exists(path))
                {
                    return GetFolderIcon(path);
                }

                if (type == HotkeyType.Website)
                {
                    return CreateVectorIcon("🌐", "#00E5FF");
                }

                if (type == HotkeyType.TextSnippet)
                {
                    return CreateVectorIcon("📝", "#FFD600");
                }

                if (type == HotkeyType.SystemCommand)
                {
                    return CreateVectorIcon("⚡", "#00D26A");
                }

                if (File.Exists(path))
                {
                    // For .lnk files, let SHGetFileInfo resolve the target icon
                    var shinfo = new Win32Api.SHFILEINFO();
                    uint flags = Win32Api.SHGFI_ICON | Win32Api.SHGFI_LARGEICON;

                    IntPtr hImg = Win32Api.SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                    if (shinfo.hIcon != IntPtr.Zero)
                    {
                        var bs = Imaging.CreateBitmapSourceFromHIcon(
                            shinfo.hIcon,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        Win32Api.DestroyIcon(shinfo.hIcon);
                        bs.Freeze();
                        return bs;
                    }
                }
            }
            catch
            {
                // Fallback to vector glyph
            }

            return CreateVectorIcon("⚙", "#00D26A");
        }

        public static BitmapSource? GetFolderIcon(string? path)
        {
            try
            {
                var shinfo = new Win32Api.SHFILEINFO();
                uint flags = Win32Api.SHGFI_ICON | Win32Api.SHGFI_LARGEICON;
                string folderPath = !string.IsNullOrWhiteSpace(path) && Directory.Exists(path) ? path : Environment.GetFolderPath(Environment.SpecialFolder.Windows);

                Win32Api.SHGetFileInfo(folderPath, Win32Api.FILE_ATTRIBUTE_DIRECTORY, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    var bs = Imaging.CreateBitmapSourceFromHIcon(
                        shinfo.hIcon,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    Win32Api.DestroyIcon(shinfo.hIcon);
                    bs.Freeze();
                    return bs;
                }
            }
            catch { }

            return CreateVectorIcon("📁", "#FFB300");
        }

        public static BitmapSource CreateVectorIcon(string symbol, string hexColor)
        {
            int size = 64;
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);
                var bgBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, color.R, color.G, color.B));
                var borderPen = new System.Windows.Media.Pen(new SolidColorBrush(color), 2);

                dc.DrawRoundedRectangle(bgBrush, borderPen, new Rect(4, 4, size - 8, size - 8), 12, 12);

                var formattedText = new FormattedText(
                    symbol,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new System.Windows.Media.FontFamily("Segoe UI Emoji, Segoe UI Symbol, Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    28,
                    System.Windows.Media.Brushes.White,
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip);

                double x = (size - formattedText.Width) / 2.0;
                double y = (size - formattedText.Height) / 2.0;
                dc.DrawText(formattedText, new System.Windows.Point(x, y));
            }

            var rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }

        public static string BitmapSourceToBase64(BitmapSource source)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static BitmapSource? Base64ToBitmapSource(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(bytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }
    }
}
