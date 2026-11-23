using System;
using System.Collections.Generic;
using InsightHotKey.Localization;
using InsightHotKey.Models;

namespace InsightHotKey.Services
{
    public static class HotkeyValidator
    {
        private record ReservedKeyInfo(KeyModifiers Modifiers, uint VirtualKey, string DescAr, string DescEn);

        private static readonly List<ReservedKeyInfo> ReservedKeys = new()
        {
            new(KeyModifiers.Control, 0x43, "النسخ (Copy)", "Copy (Ctrl+C)"),
            new(KeyModifiers.Control, 0x56, "اللصق (Paste)", "Paste (Ctrl+V)"),
            new(KeyModifiers.Control, 0x58, "القص (Cut)", "Cut (Ctrl+X)"),
            new(KeyModifiers.Control, 0x5A, "التراجع (Undo)", "Undo (Ctrl+Z)"),
            new(KeyModifiers.Control, 0x59, "الإعادة (Redo)", "Redo (Ctrl+Y)"),
            new(KeyModifiers.Control, 0x41, "تحديد الكل (Select All)", "Select All (Ctrl+A)"),
            new(KeyModifiers.Control, 0x53, "الحفظ (Save)", "Save (Ctrl+S)"),
            new(KeyModifiers.Control, 0x46, "البحث (Find)", "Find (Ctrl+F)"),
            new(KeyModifiers.Control, 0x50, "الطباعة (Print)", "Print (Ctrl+P)"),
            new(KeyModifiers.Control, 0x4F, "فتح ملف (Open)", "Open File (Ctrl+O)"),
            new(KeyModifiers.Control, 0x4E, "إنشاء جديد (New)", "New (Ctrl+N)"),
            new(KeyModifiers.Control, 0x57, "إغلاق التبويب (Close Tab)", "Close Tab (Ctrl+W)"),
            new(KeyModifiers.Alt, 0x73, "إغلاق التطبيق (Alt+F4)", "Close Application (Alt+F4)"), // VK_F4 = 0x73
            new(KeyModifiers.Alt, 0x09, "التبديل بين النوافذ (Alt+Tab)", "Switch Windows (Alt+Tab)"), // VK_TAB = 0x09
            new(KeyModifiers.Win, 0x44, "إظهار سطح المكتب (Show Desktop)", "Show Desktop (Win+D)"),
            new(KeyModifiers.Win, 0x45, "مستكشف الملفات (Explorer)", "File Explorer (Win+E)"),
            new(KeyModifiers.Win, 0x4C, "قفل الشاشة (Lock Windows)", "Lock Windows (Win+L)"),
            new(KeyModifiers.Win, 0x52, "نافذة تشغيل (Run)", "Run Dialog (Win+R)"),
            new(KeyModifiers.Win, 0x58, "قائمة النظام السريعة (Win+X)", "Quick Link Menu (Win+X)"),
        };

        public static bool IsReserved(KeyModifiers modifiers, uint virtualKey, out string description)
        {
            description = string.Empty;
            // Normalize modifiers (strip NoRepeat if present)
            var cleanMods = modifiers & ~KeyModifiers.NoRepeat;

            foreach (var rk in ReservedKeys)
            {
                if (rk.Modifiers == cleanMods && rk.VirtualKey == virtualKey)
                {
                    description = AppLanguage.CurrentLanguage == "ar" ? rk.DescAr : rk.DescEn;
                    return true;
                }
            }
            return false;
        }
    }
}
