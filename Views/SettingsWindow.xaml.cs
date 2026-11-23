using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using InsightHotKey.Localization;
using InsightHotKey.Models;
using InsightHotKey.Services;

namespace InsightHotKey.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;
        private bool _isInitializing = true;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;

            AppLanguage.CurrentLanguage = _settings.Language;

            PopulateLanguages();
            LoadSettings();
            ApplyLocalization();
            _isInitializing = false;
        }

        private void PopulateLanguages()
        {
            CmbLanguage.Items.Clear();
            CmbLanguage.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Lang_Arabic"), Tag = "ar" });
            CmbLanguage.Items.Add(new ComboBoxItem { Content = AppLanguage.Get("Lang_English"), Tag = "en" });

            for (int i = 0; i < CmbLanguage.Items.Count; i++)
            {
                if (CmbLanguage.Items[i] is ComboBoxItem cbi && (string)cbi.Tag == _settings.Language)
                {
                    CmbLanguage.SelectedIndex = i;
                    break;
                }
            }
            if (CmbLanguage.SelectedIndex < 0) CmbLanguage.SelectedIndex = 0;
        }

        private void ApplyLocalization()
        {
            FlowDirection = AppLanguage.IsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            TxtTitle.Text = AppLanguage.Get("SettingsTitle");

            HeaderLanguage.Text = AppLanguage.Get("LanguageSection");
            LblSelectLang.Text = AppLanguage.Get("SelectLanguage");

            HeaderBehavior.Text = AppLanguage.Get("BehaviorSection");
            ChkStartup.Content = AppLanguage.Get("StartWithWindows");
            ChkMinimizeToTray.Content = AppLanguage.Get("MinimizeToTray");
            ChkSmartSwitch.Content = AppLanguage.Get("SmartSwitch");
            ChkAllowReservedHotkeys.Content = AppLanguage.Get("AllowReservedHotkeys");

            HeaderBackup.Text = AppLanguage.Get("BackupSection");
            TxtBackupDesc.Text = AppLanguage.Get("PortableNotice");
            BtnExport.Content = AppLanguage.Get("ExportSettings");
            BtnImport.Content = AppLanguage.Get("ImportSettings");

            TxtAboutTitle.Text = AppLanguage.Get("AboutTitle");
            TxtAboutDesc.Text = AppLanguage.Get("AboutDesc");
            TxtPortableBadge.Text = AppLanguage.Get("PortableBadge");

            BtnSaveClose.Content = AppLanguage.Get("SaveAndClose");
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;

            if (CmbLanguage.SelectedItem is ComboBoxItem cbi && cbi.Tag is string langCode)
            {
                _settings.Language = langCode;
                AppLanguage.CurrentLanguage = langCode;
                ApplyLocalization();
            }
        }

        private void LoadSettings()
        {
            ChkStartup.IsChecked = _settings.StartWithWindows || StartupManager.IsStartupEnabled();
            ChkMinimizeToTray.IsChecked = _settings.MinimizeToTrayOnClose;
            ChkSmartSwitch.IsChecked = _settings.SmartWindowSwitching;
            ChkAllowReservedHotkeys.IsChecked = _settings.AllowReservedWindowsHotkeys;
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = AppLanguage.Get("ExportSettings"),
                Filter = "JSON Files (*.json)|*.json",
                FileName = $"InsightHotKey_Backup_{DateTime.Now:yyyyMMdd}.json"
            };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    string json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(sfd.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(AppLanguage.Get("ExportSuccess"), AppLanguage.Get("AppTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, AppLanguage.Get("AppTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = AppLanguage.Get("ImportSettings"),
                Filter = "JSON Files (*.json)|*.json"
            };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(ofd.FileName, System.Text.Encoding.UTF8);
                    var imported = JsonSerializer.Deserialize<AppSettings>(json);
                    if (imported != null)
                    {
                        _settings.Items = imported.Items;
                        _settings.StartWithWindows = imported.StartWithWindows;
                        _settings.MinimizeToTrayOnClose = imported.MinimizeToTrayOnClose;
                        _settings.SmartWindowSwitching = imported.SmartWindowSwitching;
                        _settings.AllowReservedWindowsHotkeys = imported.AllowReservedWindowsHotkeys;
                        _settings.Language = imported.Language;

                        AppLanguage.CurrentLanguage = _settings.Language;
                        StorageService.SaveSettings(_settings);

                        MessageBox.Show(AppLanguage.Get("ImportSuccess"), AppLanguage.Get("AppTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, AppLanguage.Get("AppTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool startup = ChkStartup.IsChecked == true;
            _settings.StartWithWindows = startup;
            StartupManager.SetStartup(startup);

            _settings.MinimizeToTrayOnClose = ChkMinimizeToTray.IsChecked == true;
            _settings.SmartWindowSwitching = ChkSmartSwitch.IsChecked == true;
            _settings.AllowReservedWindowsHotkeys = ChkAllowReservedHotkeys.IsChecked == true;

            if (CmbLanguage.SelectedItem is ComboBoxItem cbi && cbi.Tag is string langCode)
            {
                _settings.Language = langCode;
                AppLanguage.CurrentLanguage = langCode;
            }

            StorageService.SaveSettings(_settings);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
