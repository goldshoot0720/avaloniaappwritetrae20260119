using avaloniaappwritetrae20260119.Models;
using avaloniaappwritetrae20260119.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Linq;

namespace avaloniaappwritetrae20260119.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly AppwriteService _appwriteService;

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

                CheckAndNotifyExpiringSubscriptions();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CheckAndNotifyExpiringSubscriptions()
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
