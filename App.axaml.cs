using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using avaloniaappwritetrae20260119.ViewModels;
using avaloniaappwritetrae20260119.Views;

namespace avaloniaappwritetrae20260119
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                
                desktop.MainWindow = mainWindow;

                if (desktop.Args != null && desktop.Args.Contains("--autostart"))
                {
                    // Start minimized to tray
                    mainWindow.WindowState = Avalonia.Controls.WindowState.Minimized;
                    mainWindow.ShowInTaskbar = false;
                    mainWindow.IsVisible = false;
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        private void TrayIcon_Clicked(object? sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void OpenFromTray_Click(object? sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void ExitFromTray_Click(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private void ShowMainWindow()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = desktop.MainWindow;
                if (window != null)
                {
                    window.Show();
                    window.WindowState = Avalonia.Controls.WindowState.Normal;
                    window.ShowInTaskbar = true;
                    window.Activate();
                    window.IsVisible = true;
                }
            }
        }
    }
}
