using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Messages;

namespace WaDesktop.Client.Presenters
{
    public class SidebarPresenter : IDisposable
    {
        private readonly ISidebarView _view;
        private readonly IApiClient _api;
        private readonly IEventAggregator _bus;
        private Company _currentCompany;
        private bool _disposed;
        private readonly System.Timers.Timer _webhookStatusTimer;
        private readonly IDisposable _sessionExpiredSub;
        private readonly IDisposable _sessionRestoredSub;

        public SidebarPresenter(ISidebarView view, IApiClient api, IEventAggregator bus)
        {
            _view = view;
            _api = api;
            _bus = bus;

            _view.PhoneNumberSelected += OnPhoneNumberSelected;
            _view.RefreshRequested += OnRefreshRequested;
            _view.SettingLimitClicked += OnSettingLimitClicked;

            _webhookStatusTimer = new System.Timers.Timer(15000); // 15 detik
            _webhookStatusTimer.Elapsed += async (s, e) => await CheckWebhookStatusAsync();
            _webhookStatusTimer.Start();

            _sessionExpiredSub = _bus.Subscribe<SessionExpiredMessage>(msg => 
            {
                _webhookStatusTimer.Stop();
            });

            _sessionRestoredSub = _bus.Subscribe<LoginCompletedMessage>(async msg => 
            {
                await LoadDataAsync();
                _webhookStatusTimer.Start();
            });
        }

        private async Task CheckWebhookStatusAsync()
        {
            try
            {
                var status = await Task.Run(() => _api.GetWebhookStatusAsync());
                _view.UpdateWebhookStatus(status.IsRunning, status.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load webhook status: {ex.Message}");
            }
        }

        public async Task LoadDataAsync()
        {
            _view.IsLoading = true;
            try
            {
                //var phones = await Task.Run(() => _api.GetPhoneNumbersAsync());
                //_view.LoadPhoneNumbers(BuildTree(phones));

                _currentCompany = await Task.Run(() => _api.GetBillingAnalyticsAsync());
                _view.UpdateUsageSummary(_currentCompany);

                // Initial webhook status check
                await CheckWebhookStatusAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load sidebar data: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnSettingLimitClicked(object sender, EventArgs e)
        {
            if (_currentCompany == null) return;

            var dialogView = _view.CreateLimitBillingView();
            using (var presenter = new LimitBillingPresenter(dialogView, _api, _currentCompany))
            {
                if (_view.ShowDialog(dialogView))
                {
                    await LoadDataAsync();
                }
            }
        }

        private async void OnRefreshRequested(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private static IList<PhoneNumberNode> BuildTree(List<PhoneNumberNode> phones)
        {
            var root = new PhoneNumberNode { DisplayName = "Phone Numbers" };
            foreach (var p in phones.OrderBy(p => p.DisplayName))
                root.Children.Add(p);
            return new[] { root };
        }

        private void OnPhoneNumberSelected(object sender, PhoneNumberSelectedEventArgs e)
        {
            // Only leaf phone numbers (with PhoneNumberId) trigger tab open
            if (string.IsNullOrEmpty(e.PhoneNumberId)) return;
            var key = $"phonedetail_{e.PhoneNumberId}";
            _bus.Publish(new RequestOpenTabMessage(key, e.DisplayName ?? e.WaId));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _sessionExpiredSub?.Dispose();
                _sessionRestoredSub?.Dispose();

                _webhookStatusTimer?.Stop();
                _webhookStatusTimer?.Dispose();

                _view.PhoneNumberSelected -= OnPhoneNumberSelected;
                _view.RefreshRequested -= OnRefreshRequested;
                _view.SettingLimitClicked -= OnSettingLimitClicked;
                _disposed = true;
            }
        }
    }
}
