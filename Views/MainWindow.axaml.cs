using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Win32;
using System;

namespace avaloniaappwritetrae20260119.Views
{
    public partial class MainWindow : Window
    {
        private const string AppName = "AvaloniaAppwriteSubscriptionManager";

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize checkbox state
            var checkBox = this.FindControl<CheckBox>("AutoStartCheckBox");
            if (checkBox != null)
            {
                checkBox.IsChecked = IsAutoStartEnabled();
            }
        }

        private void AutoStart_OnCheckChanged(object? sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                SetAutoStart(checkBox.IsChecked == true);
            }
        }

        private bool IsAutoStartEnabled()
        {
            try 
            {
                if (System.OperatingSystem.IsWindows())
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                    {
                        return key?.GetValue(AppName) != null;
                    }
                }
                return false;
            }
            catch 
            {
                return false; 
            }
        }

        private void SetAutoStart(bool enable)
        {
            try
            {
                if (System.OperatingSystem.IsWindows())
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        if (key == null) return;

                        if (enable)
                        {
                            string? path = System.Environment.ProcessPath;
                            if (!string.IsNullOrEmpty(path))
                            {
                                // Add --autostart argument
                                key.SetValue(AppName, $"\"{path}\" --autostart");
                            }
                        }
                        else
                        {
                            key.DeleteValue(AppName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting auto start: {ex.Message}");
            }
        }
    }
}
