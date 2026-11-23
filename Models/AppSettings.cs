using System.Collections.Generic;

namespace InsightHotKey.Models
{
    public class AppSettings
    {
        public string Language { get; set; } = "ar"; // "ar" or "en"
        public string Theme { get; set; } = "Dark";  // "Dark" or "Light"

        public KeyModifiers MasterModifiers { get; set; } = KeyModifiers.Alt;
        public uint MasterVirtualKey { get; set; } = 0x20; // Space key
        public string MasterKeyDisplay { get; set; } = "ALT + Space";

        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTrayOnClose { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;
        public bool SmartWindowSwitching { get; set; } = true;
        public bool AllowReservedWindowsHotkeys { get; set; } = false;

        public List<HotkeyItem> Items { get; set; } = new();
    }
}
