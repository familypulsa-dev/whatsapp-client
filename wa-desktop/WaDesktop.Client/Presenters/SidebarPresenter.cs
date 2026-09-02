using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Domain.State;

namespace WaDesktop.Client.Presenters
{
    public class SidebarPresenter : IDisposable
    {
        private readonly ISidebarView _view;
        private readonly ICompanyRepository _companies;
        private readonly IBillingRepository _billing;
        private readonly IAppSettingsRepository _settings;
        private readonly IEventAggregator _bus;
        private Company _currentCompany;
        private bool _disposed;
        private readonly System.Timers.Timer _webhookStatusTimer;
        private readonly IDisposable _sessionExpiredSub;
        private readonly IDisposable _sessionRestoredSub;
        private readonly AppState _state;

        public SidebarPresenter(ISidebarView view, ICompanyRepository companies,
            IBillingRepository billing, IAppSettingsRepository settings, IEventAggregator bus, AppState state)
        {
            _view = view;
            _companies = companies;
            _billing = billing;
            _settings = settings;
            _bus = bus;
            _state = state;

            _view.PhoneNumberSelected += OnPhoneNumberSelected;
            _view.RefreshRequested += OnRefreshRequested;

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
                var result = await Task.Run(() => _settings.GetWebhookStatusAsync());
                if (result.IsFailure)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load webhook status: {result.Error.Message}");
                    return;
                }
                _view.UpdateWebhookStatus(result.Value.IsRunning, result.Value.Message);
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
                //var phones = await Task.Run(() => _companies.GetPhoneNumbersAsync());
                //_view.LoadPhoneNumbers(BuildTree(phones));

                if (!_state.IsSuperAdmin)
                {
                    var result = await Task.Run(() => _billing.GetAnalyticsAsync());
                    if (result.IsSuccess)
                    {
                        _view.UpdateUsageSummary(result.Value);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load sidebar data: {result.Error.Message}");
                    }
                }

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
                _disposed = true;
            }
        }
    }
}
