using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Messages;
using System.Collections.Generic;

using WaDesktop.Client.Views;
using WaDesktop.Client.Views.ManagementViews;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace WaDesktop.Client.Presenters
{
    public class PhoneNumbersPresenter : IDisposable, IPresenterBase
    {
        private readonly IManagementView<PhoneNumberDetail> _view;
        private readonly IPhoneNumberRepository _phones;
        private readonly IWabaRepository _wabas;
        private readonly IEventAggregator _bus;
        private readonly IServiceProvider _serviceProvider;
        private List<PhoneNumberDetail> _data;
        private bool _disposed;
        private PhoneNumberView _realView;

        public PhoneNumbersPresenter(IManagementView<PhoneNumberDetail> view, IPhoneNumberRepository phones, IWabaRepository wabas, IEventAggregator bus, IServiceProvider serviceProvider)
        {
            _view = view;
            _phones = phones;
            _wabas = wabas;
            _bus = bus;
            _serviceProvider = serviceProvider;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.AddClicked += OnAdd;
            _view.EditClicked += OnEdit;
            _view.DeleteClicked += OnDelete;

            _realView = _view as PhoneNumberView;
            if (_realView != null)
            {
                _realView.SyncClicked += OnSync;
                _realView.WabaFilterChanged += OnWabaFilterChanged;
                _realView.RegisterClicked += OnRegister;
                _realView.WebhookClicked += OnWebhook;
            }
        }

        private string _currentWabaFilter = null;

        private bool _isLoadingData = false;

        private async void OnWabaFilterChanged(object sender, string wabaId)
        {
            _currentWabaFilter = wabaId;
            
            // Jangan load ulang jika kita sedang dalam proses initial load
            if (!_isLoadingData && _wabasLoaded)
            {
                await LoadDataAsync();
            }
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private bool _wabasLoaded = false;

        private async Task LoadDataAsync(string search = null)
        {
            if (_isLoadingData) return;
            
            _isLoadingData = true;
            _view.IsLoading = true;
            try
            {
                // 1. Load WABA terlebih dahulu jika belum pernah diload
                if (!_wabasLoaded && _realView != null)
                {
                    var wabasResult = await Task.Run(() => _wabas.GetAllAsync());
                    if (wabasResult.IsFailure)
                        throw new Exception(wabasResult.Error.Message);
                    var wabas = wabasResult.Value;
                    _realView.SetWabaSyncDataSource(wabas);
                    _wabasLoaded = true;

                    // Fallback: pastikan _currentWabaFilter terisi WabaId pertama
                    if (string.IsNullOrEmpty(_currentWabaFilter) && wabas != null && wabas.Count > 0)
                    {
                        _currentWabaFilter = wabas[0].WabaId;
                    }
                }

                // 2. Jika tidak ada WABA sama sekali, kosongkan grid
                if (string.IsNullOrEmpty(_currentWabaFilter))
                {
                    _data = new List<PhoneNumberDetail>();
                    _view.DataSource = _data;
                    return;
                }

                // 3. Setelah WABA terjamin ada, baru panggil API list
                var dataResult = await Task.Run(() => _phones.GetAllAsync(_currentWabaFilter));
                if (dataResult.IsFailure)
                    throw new Exception(dataResult.Error.Message);
                var data = dataResult.Value;

                if (!string.IsNullOrEmpty(search))
                    data = data.FindAll(p =>
                        (p.DisplayName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.DisplayPhone ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
                _data = data;
                _view.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load phone numbers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
                _isLoadingData = false;
            }
        }

        private void OnAdd(object sender, EventArgs e)
        {
            var wabaId = _realView?.SelectedWabaForSyncId;
            if (string.IsNullOrEmpty(wabaId))
            {
                MessageBox.Show("Silakan pilih WABA terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = new PhoneRegistrationDialog();
            dialog.WabaId = wabaId;
            dialog.PhoneNumberId = "";

            var useCase = _serviceProvider.GetRequiredService<IPhoneRegistrationUseCase>();
            using (var presenter = new PhoneRegistrationPresenter(dialog, useCase))
            {
                presenter.Initialize();
                var form = _view as System.Windows.Forms.Control;
                var parent = form?.FindForm();
                if (dialog.ShowDialog(parent) == DialogResult.OK)
                {
                    _ = LoadDataAsync();
                }
            }
        }

        private void OnSync(object sender, EventArgs e)
        {
            var wabaId = _realView?.SelectedWabaForSyncId;
            if (string.IsNullOrEmpty(wabaId))
            {
                MessageBox.Show("Silakan pilih WABA terlebih dahulu untuk disinkronisasi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Yakin ingin sinkronisasi nomor telepon dari Meta?", "Sinkronisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _view.IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        var sync = await _phones.SyncFromMetaAsync(wabaId);
                        if (sync.IsFailure)
                            throw new Exception(sync.Error.Message);
                        await LoadDataAsync();
                        MessageBox.Show("Sinkronisasi selesai.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Sinkronisasi gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _view.IsLoading = false;
                    }
                });
            }
        }

        private void OnEdit(object sender, string phoneID)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            var item = _data.Where(p => p.PhoneNumberId == phoneID).FirstOrDefault();
            if(item != null) {
                var key = $"phonedetail_{item.PhoneNumberId}";
                _bus.Publish(new RequestOpenTabMessage(key, item.DisplayName ?? item.PhoneNumberId));
            }
        }

        private void OnDelete(object sender, EventArgs e)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            MessageBox.Show("Delete phone number — implement API call if needed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnRegister(object sender, string phoneId)
        {
            var wabaId = _realView?.SelectedWabaForSyncId;
            if (string.IsNullOrEmpty(wabaId))
            {
                MessageBox.Show("Silakan pilih WABA terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = new PhoneRegistrationDialog();
            dialog.WabaId = wabaId;
            dialog.PhoneNumberId = phoneId ?? "";

            var useCase = _serviceProvider.GetRequiredService<IPhoneRegistrationUseCase>();
            using (var presenter = new PhoneRegistrationPresenter(dialog, useCase))
            {
                presenter.Initialize();
                var form = _view as System.Windows.Forms.Control;
                var parent = form?.FindForm();
                if (dialog.ShowDialog(parent) == DialogResult.OK)
                {
                    _ = LoadDataAsync();
                }
            }
        }

        private async void OnWebhook(object sender, string phoneNumberId)
        {
            var form = _view as System.Windows.Forms.Control;
            var parent = form?.FindForm();

            try
            {
                // GET current webhook
                _view.IsLoading = true;
                var getResult = await Task.Run(() => _phones.GetWebhookAsync(phoneNumberId));
                if (getResult.IsFailure)
                {
                    MessageBox.Show($"Gagal mengambil data webhook: {getResult.Error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var wh = getResult.Value;
                var currentUrl = wh?.Application ?? "(belum diset)";

                var input = new WebhookInputDialog();
                input.DialogTitle = "Webhook Configuration";
                input.PhoneNumberId = phoneNumberId;
                input.CurrentWebhookUrl = currentUrl;
                input.WebhookUrl = wh?.Application ?? "";

                if (input.ShowDialog(parent) == DialogResult.OK)
                {
                    var setResult = await Task.Run(() => _phones.SetWebhookAsync(phoneNumberId, input.WebhookUrl));
                    if (setResult.IsSuccess)
                        MessageBox.Show("Webhook berhasil disimpan.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show($"Gagal menyimpan webhook: {setResult.Error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.RefreshClicked -= null;
                _view.SearchClicked -= null;
                _view.AddClicked -= null;
                _view.EditClicked -= null;
                _view.DeleteClicked -= null;

                if (_realView != null)
                {
                    _realView.SyncClicked -= OnSync;
                    _realView.WabaFilterChanged -= OnWabaFilterChanged;
                    _realView.RegisterClicked -= OnRegister;
                    _realView.WebhookClicked -= OnWebhook;
                }

                _disposed = true;
            }
        }
    }
}
