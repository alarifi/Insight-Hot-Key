using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using InsightHotKey.Models;

namespace InsightHotKey.Services
{
    public static class StorageService
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static AppSettings LoadSettings()
        {
            AppSettings settings;
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath, System.Text.Encoding.UTF8);
                    settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? CreateDefaultSettings();
                }
                else
                {
                    settings = CreateDefaultSettings();
                    SaveSettings(settings);
                }
            }
            catch
            {
                settings = CreateDefaultSettings();
            }

            // Ensure we have at least 16 slots, rounded to multiples of 8 (8 slots per row)
            EnsureSlotCount(settings, 16);

            // Populate icons
            foreach (var item in settings.Items)
            {
                if (!item.IsEmpty)
                {
                    if (!string.IsNullOrEmpty(item.IconBase64))
                    {
                        item.LoadedIcon = ShellIconService.Base64ToBitmapSource(item.IconBase64);
                    }
                    if (item.LoadedIcon == null)
                    {
                        item.LoadedIcon = ShellIconService.GetIconForTarget(item.TargetPath, item.ItemType);
                        if (item.LoadedIcon is System.Windows.Media.Imaging.BitmapSource bs)
                        {
                            item.IconBase64 = ShellIconService.BitmapSourceToBase64(bs);
                        }
                    }
                }
            }

            return settings;
        }

        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(ConfigPath, json, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        public static AppSettings CreateDefaultSettings()
        {
            var settings = new AppSettings
            {
                Language = "ar",
                Theme = "Dark",
                MasterModifiers = KeyModifiers.Alt,
                MasterVirtualKey = 0x20, // Space
                MasterKeyDisplay = "ALT + Space",
                StartWithWindows = false,
                MinimizeToTrayOnClose = true,
                SmartWindowSwitching = true,
                Items = new List<HotkeyItem>()
            };

            // Preset items inspired by Wise HotKey
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Slot 0: Browser (msedge) -> ALT + 1
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (!File.Exists(edgePath)) edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 0,
                Title = "متصفح الإنترنت",
                ItemType = HotkeyType.Application,
                TargetPath = File.Exists(edgePath) ? edgePath : "https://www.google.com",
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x31, // '1'
                KeyDisplay = "ALT + 1"
            });

            // Slot 1: Explorer / This PC -> ALT + 2
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 1,
                Title = "جهاز الكمبيوتر",
                ItemType = HotkeyType.Folder,
                TargetPath = userProfile,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x32, // '2'
                KeyDisplay = "ALT + 2"
            });

            // Slot 2: Downloads -> ALT + 3
            string downloads = Path.Combine(userProfile, "Downloads");
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 2,
                Title = "التنزيلات",
                ItemType = HotkeyType.Folder,
                TargetPath = Directory.Exists(downloads) ? downloads : userProfile,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x33, // '3'
                KeyDisplay = "ALT + 3"
            });

            // Slot 3: Documents -> ALT + 4
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 3,
                Title = "المستندات",
                ItemType = HotkeyType.Folder,
                TargetPath = documents,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x34, // '4'
                KeyDisplay = "ALT + 4"
            });

            // Slot 4: Notepad -> ALT + 5
            string notepad = Path.Combine(system32, "notepad.exe");
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 4,
                Title = "المفكرة",
                ItemType = HotkeyType.Application,
                TargetPath = notepad,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x35, // '5'
                KeyDisplay = "ALT + 5"
            });

            // Slot 5: Calculator -> ALT + 6
            string calc = Path.Combine(system32, "calc.exe");
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 5,
                Title = "الحاسبة",
                ItemType = HotkeyType.Application,
                TargetPath = calc,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x36, // '6'
                KeyDisplay = "ALT + 6"
            });

            // Slot 6: Task Manager -> ALT + T
            string taskmgr = Path.Combine(system32, "taskmgr.exe");
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 6,
                Title = "مدير المهام",
                ItemType = HotkeyType.Application,
                TargetPath = taskmgr,
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x54, // 'T'
                KeyDisplay = "ALT + T"
            });

            // Slot 7: Lock Workstation -> ALT + L
            settings.Items.Add(new HotkeyItem
            {
                SlotIndex = 7,
                Title = "قفل الشاشة",
                ItemType = HotkeyType.SystemCommand,
                TargetPath = "lock",
                Modifiers = KeyModifiers.Alt,
                VirtualKey = 0x4C, // 'L'
                KeyDisplay = "ALT + L"
            });

            EnsureSlotCount(settings, 16);
            return settings;
        }

        public static void AddRow(AppSettings settings)
        {
            int startIndex = settings.Items.Count;
            for (int i = 0; i < 8; i++)
            {
                settings.Items.Add(new HotkeyItem
                {
                    SlotIndex = startIndex + i,
                    Title = string.Empty,
                    TargetPath = string.Empty,
                    KeyDisplay = "None"
                });
            }
            SaveSettings(settings);
        }

        private static void EnsureSlotCount(AppSettings settings, int minCount = 16)
        {
            int target = Math.Max(minCount, ((settings.Items.Count + 7) / 8) * 8);
            var dict = new Dictionary<int, HotkeyItem>();
            foreach (var itm in settings.Items)
            {
                dict[itm.SlotIndex] = itm;
            }

            settings.Items.Clear();
            for (int i = 0; i < target; i++)
            {
                if (dict.TryGetValue(i, out var existing))
                {
                    existing.SlotIndex = i;
                    settings.Items.Add(existing);
                }
                else
                {
                    settings.Items.Add(new HotkeyItem
                    {
                        SlotIndex = i,
                        Title = string.Empty,
                        TargetPath = string.Empty,
                        KeyDisplay = "None"
                    });
                }
            }
        }
    }
}
