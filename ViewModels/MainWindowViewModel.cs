using avaloniaappwritetrae20260119.Models;
using avaloniaappwritetrae20260119.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Linq;
using Avalonia.Threading;

namespace avaloniaappwritetrae20260119.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly AppwriteService _appwriteService;
        private readonly DispatcherTimer _timer;
        private DateTime _lastNotificationDate = DateTime.MinValue;

        [ObservableProperty]
        private ObservableCollection<Subscription> _subscriptions = new();

        [ObservableProperty]
        private bool _isLoading;

        public MainWindowViewModel()
        {
            _appwriteService = new AppwriteService();
            LoadSubscriptionsCommand = new AsyncRelayCommand(LoadSubscriptionsAsync);
            
            // Auto load on start
            _ = LoadSubscriptionsAsync();

            // Initialize timer to check every minute
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            
            // Check if it's after 6:00 AM and we haven't notified today yet
            if (now.Hour >= 6 && _lastNotificationDate.Date != now.Date)
            {
                // Trigger notification check
                CheckAndNotifyExpiringSubscriptions();
                
                // Update last notification date to today
                _lastNotificationDate = now.Date;
            }
        }

        public IAsyncRelayCommand LoadSubscriptionsCommand { get; }

        private async Task LoadSubscriptionsAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                Subscriptions.Clear();
                var items = await _appwriteService.GetSubscriptionsAsync();
                foreach (var item in items)
                {
                    Subscriptions.Add(item);
                }

                // Initial check when app starts, but only if it's after 6 AM or user just launched it manually.
                // If user launches at 5 AM, we might not want to notify yet based on "after 6 AM" rule?
                // However, usually on startup we want to see status. 
                // Let's stick to the rule: "Every day after 6 AM check". 
                // If app starts at 7 AM, it should notify.
                // If app starts at 5 AM, it should wait until 6 AM.
                
                var now = DateTime.Now;
                if (now.Hour >= 6)
                {
                     CheckAndNotifyExpiringSubscriptions();
                     _lastNotificationDate = now.Date;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CheckAndNotifyExpiringSubscriptions()
        {
            // If subscriptions are empty (e.g. called from timer but list not loaded?), ensure we have data?
            // Since _subscriptions is ObservableCollection in memory, it should be fine if app is running.
            // If it's a long running app, maybe we should refresh data daily too?
            // Let's try to refresh data before checking if it's the daily check.
            
            if (!Subscriptions.Any())
            {
                // Try to load silently if empty
                 _ = _appwriteService.GetSubscriptionsAsync().ContinueWith(t => 
                 {
                     if (t.IsCompletedSuccessfully && t.Result != null)
                     {
                         Dispatcher.UIThread.Invoke(() => 
                         {
                             foreach(var item in t.Result) Subscriptions.Add(item);
                             ProcessNotifications();
                         });
                     }
                 });
            }
            else
            {
                ProcessNotifications();
            }
        }

        private void ProcessNotifications()
        {
            var today = DateTime.Now.Date;
            var threeDaysLater = today.AddDays(3);

            var expiringItems = Subscriptions.Where(s => s.NextDate.Date >= today && s.NextDate.Date <= threeDaysLater).ToList();

            if (expiringItems.Any())
            {
                foreach (var item in expiringItems)
                {
                    var daysLeft = (item.NextDate.Date - today).Days;
                    var dayText = daysLeft == 0 ? "today" : $"in {daysLeft} days";

                    try 
                    {
                        new ToastContentBuilder()
                            .AddText("Subscription Expiring")
                            .AddText($"{item.Name} is expiring {dayText} ({item.NextDate:yyyy-MM-dd})!")
                            .Show();
                    }
                    catch (Exception ex)
                    {
                        // Handle potential exception if not running on Windows or toast fails
                        System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
                    }
                }
            }
        }
    }
}
