using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using InsightHotKey.Localization;
using InsightHotKey.Models;
using InsightHotKey.Services;

namespace InsightHotKey.Views
{
    public partial class EditItemWindow : Window
    {
        public HotkeyItem Item { get; private set; }
        private readonly AppSettings? _settings;
        private BitmapSource? _currentIcon;
        private KeyModifiers _recordedModifiers = KeyModifiers.None;
        private uint _recordedVirtualKey = 0;
        private string _recordedKeyDisplay = "None";

        public EditItemWindow(HotkeyItem item, AppSettings? settings = null)
        {
            InitializeComponent();
            Item = item;
            _settings = settings;

            if (_settings != null && !string.IsNullOrEmpty(_settings.Language))
            {
                AppLanguage.CurrentLanguage = _settings.Language;
            }

            ApplyLocalization();
            PopulateTypes();
            LoadItemData();
        }

        private void ApplyLocalization()
        {
            FlowDirection = AppLanguage.IsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            TxtDialogTitle.Text = Item.IsEmpty ? AppLanguage.Get("DialogTitleAdd") : AppLanguage.Get("DialogTitleEdit");
            LblType.Text = AppLanguage.Get("ItemType");
            LblTitle.Text = AppLanguage.Get("ItemName");
            LblTarget.Text = AppLanguage.Get("TargetPath");
            LblArgs.Text = AppLanguage.Get("Arguments");
            LblShortcut.Text = AppLanguage.Get("ShortcutKey");
            TxtKeyHint.Text = AppLanguage.Get("RecordKeyHint");
            BtnClearKey.Content = AppLanguage.Get("ClearKey");
            LblIconPreview.Text = AppLanguage.Get("IconPreview");
            BtnRefreshIcon.Content = AppLanguage.Get("RefreshIcon");
            BtnBrowse.Content = AppLanguage.Get("BrowseFile");
            BtnSave.Content = AppLanguage.Get("Save");
            BtnCancel.Content = AppLanguage.Get("Cancel");
            BtnTest.Content = AppLanguage.Get("Test");
        }

        private void PopulateTypes()
        {
            CmbType.Items.Clear();
            CmbType.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Type_Application"), Tag = HotkeyType.Application });
            CmbType.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Type_Folder"), Tag = HotkeyType.Folder });
            CmbType.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Type_Website"), Tag = HotkeyType.Website });
            CmbType.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Type_TextSnippet"), Tag = HotkeyType.TextSnippet });
            CmbType.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Type_SystemCommand"), Tag = HotkeyType.SystemCommand });
        }

        private void LoadItemData()
        {
            for (int i = 0; i < CmbType.Items.Count; i++)
            {
                if (CmbType.Items[i] is ComboBoxItem cbi && (HotkeyType)cbi.Tag == Item.ItemType)
                {
                    CmbType.SelectedIndex = i;
                    break;
                }
            }
            if (CmbType.SelectedIndex < 0) CmbType.SelectedIndex = 0;

            TxtTitle.Text = Item.Title;
            TxtTarget.Text = Item.TargetPath;
            TxtArguments.Text = Item.Arguments;

            _recordedModifiers = Item.Modifiers;
            _recordedVirtualKey = Item.VirtualKey;
            _recordedKeyDisplay = string.IsNullOrEmpty(Item.KeyDisplay) ? "None" : Item.KeyDisplay;
            TxtKeyDisplay.Text = _recordedKeyDisplay;

            if (Item.LoadedIcon is BitmapSource bs)
            {
                _currentIcon = bs;
                ImgIconPreview.Source = bs;
            }
            else
            {
                UpdateIconPreview();
            }
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbType.SelectedItem is ComboBoxItem cbi && cbi.Tag is HotkeyType type)
            {
                PnlArguments.Visibility = type == HotkeyType.Application ? Visibility.Visible : Visibility.Collapsed;
                BtnBrowse.Visibility = (type == HotkeyType.Application || type == HotkeyType.Folder) ? Visibility.Visible : Visibility.Collapsed;
                BtnBrowse.Content = type == HotkeyType.Folder ? AppLanguage.Get("BrowseFolder") : AppLanguage.Get("BrowseFile");

                if (type == HotkeyType.Website && string.IsNullOrWhiteSpace(TxtTarget.Text))
                {
                    TxtTarget.Text = "https://";
                }
                else if (type == HotkeyType.SystemCommand && string.IsNullOrWhiteSpace(TxtTarget.Text))
                {
                    TxtTarget.Text = "lock";
                }

                UpdateIconPreview();
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = GetCurrentType();
            if (selectedType == HotkeyType.Folder)
            {
                var dialog = new OpenFolderDialog
                {
                    Title = AppLanguage.Get("BrowseFolder")
                };
                if (dialog.ShowDialog() == true)
                {
                    TxtTarget.Text = dialog.FolderName;
                    if (string.IsNullOrWhiteSpace(TxtTitle.Text))
                    {
                        TxtTitle.Text = Path.GetFileName(dialog.FolderName.TrimEnd('\\', '/'));
                    }
                    UpdateIconPreview();
                }
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    Title = AppLanguage.Get("BrowseFile"),
                    Filter = "All Supported Files (*.exe;*.lnk;*.bat;*.cmd;*.*)|*.exe;*.lnk;*.bat;*.cmd;*.*|Applications (*.exe)|*.exe|Shortcuts (*.lnk)|*.lnk|All Files (*.*)|*.*"
                };
                if (dialog.ShowDialog() == true)
                {
                    TxtTarget.Text = dialog.FileName;
                    if (string.IsNullOrWhiteSpace(TxtTitle.Text))
                    {
                        TxtTitle.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                    }
                    UpdateIconPreview();
                }
            }
        }

        private HotkeyType GetCurrentType()
        {
            if (CmbType.SelectedItem is ComboBoxItem cbi && cbi.Tag is HotkeyType t) return t;
            return HotkeyType.Application;
        }

        private void TxtTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) && !string.IsNullOrWhiteSpace(TxtTarget.Text))
            {
                var type = GetCurrentType();
                if (type == HotkeyType.Application || type == HotkeyType.Folder)
                {
                    try
                    {
                        string name = Path.GetFileNameWithoutExtension(TxtTarget.Text);
                        if (!string.IsNullOrEmpty(name)) TxtTitle.Text = name;
                    }
                    catch { }
                }
            }
        }

        private void RefreshIcon_Click(object sender, RoutedEventArgs e)
        {
            UpdateIconPreview();
        }

        private void UpdateIconPreview()
        {
            var icon = ShellIconService.GetIconForTarget(TxtTarget.Text, GetCurrentType());
            if (icon != null)
            {
                _currentIcon = icon;
                ImgIconPreview.Source = icon;
            }
        }

        private void BrdKeyRecorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BrdKeyRecorder.Focus();
            BrdKeyRecorder.BorderBrush = System.Windows.Media.Brushes.DeepSkyBlue;
            TxtKeyHint.Text = AppLanguage.IsRtl ? "اضغط المفاتيح الآن..." : "Press keys now...";
        }

        private void BrdKeyRecorder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            // Ignore pure modifier presses
            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            KeyModifiers mods = KeyModifiers.None;
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) mods |= KeyModifiers.Alt;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) mods |= KeyModifiers.Control;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) mods |= KeyModifiers.Shift;
            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin)) mods |= KeyModifiers.Win;

            int vk = KeyInterop.VirtualKeyFromKey(key);
            if (vk <= 0) return;

            _recordedModifiers = mods;
            _recordedVirtualKey = (uint)vk;

            string display = "";
            if (mods.HasFlag(KeyModifiers.Control)) display += "CTRL + ";
            if (mods.HasFlag(KeyModifiers.Alt)) display += "ALT + ";
            if (mods.HasFlag(KeyModifiers.Shift)) display += "SHIFT + ";
            if (mods.HasFlag(KeyModifiers.Win)) display += "WIN + ";

            display += key.ToString().ToUpperInvariant();
            _recordedKeyDisplay = display;

            TxtKeyDisplay.Text = _recordedKeyDisplay;
            TxtKeyHint.Text = AppLanguage.IsRtl ? "تم تسجيل الاختصار" : "Shortcut recorded";
            BrdKeyRecorder.BorderBrush = System.Windows.Media.Brushes.SeaGreen;
        }

        private void BrdKeyRecorder_KeyDown(object sender, KeyEventArgs e)
        {
            BrdKeyRecorder_PreviewKeyDown(sender, e);
        }

        private void ClearKey_Click(object sender, RoutedEventArgs e)
        {
            _recordedModifiers = KeyModifiers.None;
            _recordedVirtualKey = 0;
            _recordedKeyDisplay = "None";
            TxtKeyDisplay.Text = "None";
            TxtKeyHint.Text = AppLanguage.IsRtl ? "تم إلغاء الاختصار" : "Shortcut cleared";
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            var tempItem = new HotkeyItem
            {
                Title = TxtTitle.Text,
                ItemType = GetCurrentType(),
                TargetPath = TxtTarget.Text,
                Arguments = TxtArguments.Text
            };
            ActionExecutor.Execute(tempItem, false);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) && string.IsNullOrWhiteSpace(TxtTarget.Text))
            {
                MessageBox.Show(AppLanguage.Get("FillRequired"), AppLanguage.Get("AppTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check for reserved Windows hotkeys protection
            if (_recordedVirtualKey != 0)
            {
                if (HotkeyValidator.IsReserved(_recordedModifiers, _recordedVirtualKey, out string reservedDesc))
                {
                    bool allowWithoutWarning = _settings?.AllowReservedWindowsHotkeys ?? false;
                    if (!allowWithoutWarning)
                    {
                        string msg = string.Format(AppLanguage.Get("ReservedHotkeyWarningMsg"), _recordedKeyDisplay, reservedDesc);
                        var confirm = MessageBox.Show(msg, AppLanguage.Get("ReservedHotkeyWarningTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (confirm != MessageBoxResult.Yes)
                        {
                            return; // Do not save, let user choose a different key
                        }
                    }
                }
            }

            Item.Title = string.IsNullOrWhiteSpace(TxtTitle.Text) ? "Shortcut" : TxtTitle.Text.Trim();
            Item.ItemType = GetCurrentType();
            Item.TargetPath = TxtTarget.Text.Trim();
            Item.Arguments = TxtArguments.Text.Trim();
            Item.Modifiers = _recordedModifiers;
            Item.VirtualKey = _recordedVirtualKey;
            Item.KeyDisplay = _recordedKeyDisplay;

            if (_currentIcon != null)
            {
                Item.LoadedIcon = _currentIcon;
                Item.IconBase64 = ShellIconService.BitmapSourceToBase64(_currentIcon);
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
