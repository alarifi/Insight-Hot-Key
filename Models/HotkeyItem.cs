using System;
using System.Text.Json.Serialization;
using InsightHotKey.Localization;

namespace InsightHotKey.Models
{
    public enum HotkeyType
    {
        Application = 0,
        Folder = 1,
        Website = 2,
        TextSnippet = 3,
        SystemCommand = 4
    }

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        NoRepeat = 0x4000
    }

    public class HotkeyItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public int SlotIndex { get; set; }
        public string Title { get; set; } = string.Empty;
        public HotkeyType ItemType { get; set; } = HotkeyType.Application;
        public string TargetPath { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public string WorkingDirectory { get; set; } = string.Empty;

        public KeyModifiers Modifiers { get; set; } = KeyModifiers.None;
        public uint VirtualKey { get; set; } = 0;
        public string KeyDisplay { get; set; } = "None";

        public bool IsEnabled { get; set; } = true;
        public string? IconBase64 { get; set; }

        [JsonIgnore]
        public object? LoadedIcon { get; set; }

        [JsonIgnore]
        public bool IsEmpty => string.IsNullOrWhiteSpace(TargetPath) && string.IsNullOrWhiteSpace(Title);

        [JsonIgnore]
        public int RegisteredId { get; set; } = 0;

        [JsonIgnore]
        public string TypeBadgeText => IsEmpty ? "" : AppLanguage.GetTypeBadge(ItemType);

        [JsonIgnore]
        public string TypeBadgeColor => ItemType switch
        {
            HotkeyType.Application => "#00E5FF",
            HotkeyType.Folder => "#FBBF24",
            HotkeyType.Website => "#C084FC",
            HotkeyType.TextSnippet => "#34D399",
            HotkeyType.SystemCommand => "#F472B6",
            _ => "#00D26A"
        };

        [JsonIgnore]
        public string TypeBadgeBg => ItemType switch
        {
            HotkeyType.Application => "#102534",
            HotkeyType.Folder => "#2A1E10",
            HotkeyType.Website => "#241434",
            HotkeyType.TextSnippet => "#10261C",
            HotkeyType.SystemCommand => "#2A1222",
            _ => "#12261A"
        };
    }
}
