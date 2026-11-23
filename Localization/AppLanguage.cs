using System;
using System.Collections.Generic;

namespace InsightHotKey.Localization
{
    public static class AppLanguage
    {
        public static string CurrentLanguage { get; set; } = "ar";

        public static bool IsRtl => CurrentLanguage == "ar";

        private static readonly Dictionary<string, string> Ar = new()
        {
            // Main Window
            ["AppTitle"] = "إنسايت للاختصارات السريعة",
            ["DashboardTitle"] = "لوحة الاختصارات السريعة",
            ["ProBadge"] = "احترافي",
            ["Settings"] = "الإعدادات",
            ["Minimize"] = "تصغير",
            ["Close"] = "إغلاق",
            ["EmptySlot"] = "إضافة جديد",
            ["AddNew"] = "إضافة اختصار جديد",
            ["AddRow"] = "إضافة صف جديد",
            ["Edit"] = "تعديل",
            ["Delete"] = "حذف",
            ["Run"] = "تشغيل",
            ["Options"] = "خيارات الإجراء",
            ["MasterHotkey"] = "مفتاح استدعاء اللوحة السريعة:",
            ["DragDropHint"] = "اسحب وأفلت أي برنامج أو مجلد هنا لإضافته فوراً",
            ["DeleteConfirm"] = "هل تريد بالتأكيد حذف هذا الاختصار؟",
            ["Confirm"] = "تأكيد",

            // Type Badges
            ["Badge_Application"] = "تطبيق",
            ["Badge_Folder"] = "مجلد",
            ["Badge_Website"] = "موقع",
            ["Badge_TextSnippet"] = "نص",
            ["Badge_SystemCommand"] = "نظام",

            // Edit Window
            ["DialogTitleEdit"] = "تعديل الاختصار",
            ["DialogTitleAdd"] = "إضافة اختصار جديد",
            ["ItemName"] = "اسم العنصر:",
            ["ItemType"] = "نوع الإجراء (حدد نوع الهدف):",
            ["TargetPath"] = "المسار أو الرابط أو النص المطلوب:",
            ["Arguments"] = "المعاملات الإضافية (اختياري):",
            ["BrowseFile"] = "استعراض ملف...",
            ["BrowseFolder"] = "استعراض مجلد...",
            ["ShortcutKey"] = "مفتاح الاختصار المطلوب:",
            ["RecordKeyHint"] = "انقر هنا واضغط أي مفتاح لتسجيله",
            ["ClearKey"] = "مسح الاختصار",
            ["IconPreview"] = "معاينة الأيقونة:",
            ["RefreshIcon"] = "تحديث الأيقونة تلقائياً",
            ["Save"] = "حفظ التغييرات",
            ["Cancel"] = "إلغاء",
            ["Test"] = "تجربة الإجراء الآن",
            ["FillRequired"] = "يرجى كتابة الاسم وتحديد المسار أو الرابط أولاً.",

            // Types List
            ["Type_Application"] = "برنامج تنفيذي (.exe)",
            ["Type_Folder"] = "مجلد ملفات على جهازك",
            ["Type_Website"] = "رابط موقع ويب (URL)",
            ["Type_TextSnippet"] = "نص سريع / كود (كتابة تلقائية)",
            ["Type_SystemCommand"] = "أمر نظام سريع (قفل، حاسبة، مفكرة)",

            // Settings Window
            ["SettingsTitle"] = "إعدادات البرنامج",
            ["LanguageSection"] = "لغة واجهة البرنامج",
            ["SelectLanguage"] = "اختر اللغة المفضلة للواجهة:",
            ["Lang_Arabic"] = "اللغة العربية",
            ["Lang_English"] = "English",
            ["BehaviorSection"] = "خيارات التشغيل وسلوك النوافذ",
            ["StartWithWindows"] = "التشغيل التلقائي مع بدء تشغيل ويندوز",
            ["MinimizeToTray"] = "التصغير إلى صينية النظام عند الإغلاق",
            ["SmartSwitch"] = "التبديل الذكي للبرامج المفتوحة (تجنب التكرار)",
            ["AllowReservedHotkeys"] = "السماح باختصارات ويندوز الأساسية (مع التنبيه)",
            ["BackupSection"] = "النسخ الاحتياطي ومسار البيانات",
            ["PortableNotice"] = "برنامج محمول بالكامل (Portable) يحفظ إعداداته في مجلد البرنامج.",
            ["ExportSettings"] = "تصدير الإعدادات (ملف JSON)",
            ["ImportSettings"] = "استيراد الإعدادات",
            ["AboutTitle"] = "إنسايت للاختصارات السريعة - الإصدار 1.0.0",
            ["AboutDesc"] = "برنامج محمول متطور لإدارة وتشغيل اختصارات ويندوز بسرعة البرق.",
            ["PortableBadge"] = "● المحمول النشط",
            ["SaveAndClose"] = "حفظ وإغلاق",

            // Tray Menu
            ["TrayShow"] = "إظهار لوحة الاختصارات",
            ["TraySettings"] = "الإعدادات",
            ["TrayExit"] = "خروج نهائي",

            // Warnings and Dialogs
            ["ReservedHotkeyWarningTitle"] = "⚠️ تحذير: اختصار ويندوز أساسي",
            ["ReservedHotkeyWarningMsg"] = "الاختصار الذي اخترته ({0}) هو اختصار أساسي في نظام ويندوز ({1}).\n\nتخصيصه هنا قد يعطل استخدامه داخل البرامج الأخرى (مثل النسخ، اللصق، الحفظ).\n\nهل أنت متأكد تماماً من رغبتك في استخدام هذا الاختصار وتجاوز حماية ويندوز؟",
            ["HotkeyConflict"] = "تعذر تسجيل الاختصار ({0})، قد يكون مستخدماً في برنامج آخر.",
            ["ExportSuccess"] = "تم تصدير الإعدادات بنجاح!",
            ["ImportSuccess"] = "تم استيراد الإعدادات بنجاح وسيتم تطبيقها الآن.",
            ["SlotsFull"] = "تم ملء الخانات الحالية، انقر على 'إضافة صف جديد' لزيادة المساحة.",
            ["RowAdded"] = "تمت إضافة صف جديد بنجاح!"
        };

        private static readonly Dictionary<string, string> En = new()
        {
            // Main Window
            ["AppTitle"] = "Insight Hot Key - Quick Launcher",
            ["DashboardTitle"] = "Quick Hotkey Dashboard",
            ["ProBadge"] = "PRO",
            ["Settings"] = "Settings",
            ["Minimize"] = "Minimize",
            ["Close"] = "Close",
            ["EmptySlot"] = "Add New",
            ["AddNew"] = "Add New Hotkey",
            ["AddRow"] = "Add New Row",
            ["Edit"] = "Edit",
            ["Delete"] = "Delete",
            ["Run"] = "Run",
            ["Options"] = "Action Options",
            ["MasterHotkey"] = "Master Dashboard Hotkey:",
            ["DragDropHint"] = "Drag and drop any app or folder here to add instantly",
            ["DeleteConfirm"] = "Are you sure you want to delete this hotkey?",
            ["Confirm"] = "Confirm",

            // Type Badges
            ["Badge_Application"] = "App",
            ["Badge_Folder"] = "Folder",
            ["Badge_Website"] = "Web",
            ["Badge_TextSnippet"] = "Snippet",
            ["Badge_SystemCommand"] = "System",

            // Edit Window
            ["DialogTitleEdit"] = "Edit Shortcut",
            ["DialogTitleAdd"] = "Add New Shortcut",
            ["ItemName"] = "Item Name:",
            ["ItemType"] = "Action Type (Select target kind):",
            ["TargetPath"] = "Target Path, URL, or Text:",
            ["Arguments"] = "Arguments (Optional):",
            ["BrowseFile"] = "Browse File...",
            ["BrowseFolder"] = "Browse Folder...",
            ["ShortcutKey"] = "Assigned Shortcut Key:",
            ["RecordKeyHint"] = "Click here and press any key to record",
            ["ClearKey"] = "Clear Key",
            ["IconPreview"] = "Icon Preview:",
            ["RefreshIcon"] = "Refresh Icon Automatically",
            ["Save"] = "Save Changes",
            ["Cancel"] = "Cancel",
            ["Test"] = "Test Action Now",
            ["FillRequired"] = "Please enter an item name and target path or URL.",

            // Types List
            ["Type_Application"] = "Executable Program (.exe)",
            ["Type_Folder"] = "Local Folder / Directory",
            ["Type_Website"] = "Website URL",
            ["Type_TextSnippet"] = "Text Snippet / Code (Auto Typing)",
            ["Type_SystemCommand"] = "System Command (Lock, Calc, Notepad)",

            // Settings Window
            ["SettingsTitle"] = "Application Settings",
            ["LanguageSection"] = "Interface Language",
            ["SelectLanguage"] = "Select Preferred Interface Language:",
            ["Lang_Arabic"] = "Arabic",
            ["Lang_English"] = "English",
            ["BehaviorSection"] = "Startup and Window Behavior",
            ["StartWithWindows"] = "Launch automatically on Windows startup",
            ["MinimizeToTray"] = "Minimize to system tray on close",
            ["SmartSwitch"] = "Smart window switching (Avoid duplicate instances)",
            ["AllowReservedHotkeys"] = "Allow reserved Windows shortcuts (with warning)",
            ["BackupSection"] = "Data Persistence & Backup",
            ["PortableNotice"] = "100% Portable application saving configuration to its local folder.",
            ["ExportSettings"] = "Export Settings (JSON File)",
            ["ImportSettings"] = "Import Settings",
            ["AboutTitle"] = "Insight Hot Key - Version 1.0.0",
            ["AboutDesc"] = "Advanced lightweight portable hotkey launcher for Windows.",
            ["PortableBadge"] = "● 100% PORTABLE",
            ["SaveAndClose"] = "Save and Close",

            // Tray Menu
            ["TrayShow"] = "Show Dashboard",
            ["TraySettings"] = "Settings",
            ["TrayExit"] = "Exit",

            // Warnings and Dialogs
            ["ReservedHotkeyWarningTitle"] = "⚠️ Warning: Core Windows Shortcut",
            ["ReservedHotkeyWarningMsg"] = "The shortcut you selected ({0}) is a core Windows system shortcut ({1}).\n\nAssigning it here may disable its normal function in other applications (such as Copy, Paste, Save).\n\nAre you sure you want to override and use this shortcut?",
            ["HotkeyConflict"] = "Unable to register hotkey ({0}). It may be used by another application.",
            ["ExportSuccess"] = "Settings exported successfully!",
            ["ImportSuccess"] = "Settings imported successfully and applied.",
            ["SlotsFull"] = "Current slots are full. Click 'Add New Row' to create more slots.",
            ["RowAdded"] = "New row added successfully!"
        };

        public static string Get(string key)
        {
            var dict = CurrentLanguage == "ar" ? Ar : En;
            if (dict.TryGetValue(key, out var val)) return val;
            if (En.TryGetValue(key, out var enVal)) return enVal;
            return key;
        }

        public static string GetTypeBadge(InsightHotKey.Models.HotkeyType type)
        {
            return type switch
            {
                InsightHotKey.Models.HotkeyType.Application => Get("Badge_Application"),
                InsightHotKey.Models.HotkeyType.Folder => Get("Badge_Folder"),
                InsightHotKey.Models.HotkeyType.Website => Get("Badge_Website"),
                InsightHotKey.Models.HotkeyType.TextSnippet => Get("Badge_TextSnippet"),
                InsightHotKey.Models.HotkeyType.SystemCommand => Get("Badge_SystemCommand"),
                _ => ""
            };
        }
    }
}
