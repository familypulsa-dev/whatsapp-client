using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Client.Views.ManagementViews;

namespace WaDesktop.Client.Presenters
{
    public class TemplatesPresenter : IDisposable, IPresenterBase
    {
        private readonly IManagementView<Template> _view;
        private readonly ITemplateRepository _templates;
        private readonly IWabaRepository _wabas;
        private List<Template> _data;
        private readonly IEventAggregator _bus;
        private readonly IAuthService _auth;
        private readonly string _messagesUrl;
        private readonly string _apiBaseUrl;
        private TemplatesView _realView;
        private MessagesPresenter _embeddedMsgPresenter;
        private bool _disposed;
        private string _currentWabaFilter;
        private bool _wabasLoaded;
        private bool _isLoadingData;
        private readonly List<string> _pendingDeletions = new List<string>();

        public TemplatesPresenter(IManagementView<Template> view, ITemplateRepository templates, IWabaRepository wabas,
            IEventAggregator bus, IAuthService auth, string messagesUrl, string apiBaseUrl)
        {
            _view = view;
            _templates = templates;
            _wabas = wabas;
            _bus = bus;
            _auth = auth;
            _messagesUrl = messagesUrl;
            _apiBaseUrl = apiBaseUrl;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.AddClicked += OnAdd;
            _view.EditClicked += OnEdit;
            _view.DeleteClicked += OnDelete;

            _realView = _view as TemplatesView;
            if (_realView != null)
            {
                _realView.SyncClicked += OnSync;
                _realView.WabaFilterChanged += OnWabaFilterChanged;
                _realView.PreviewClicked += OnPreview;
                _realView.UserDeletedRowItem += OnUserDeletedRow;
            }
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            if (_isLoadingData) return;

            _pendingDeletions.Clear();
            _isLoadingData = true;
            _view.IsLoading = true;
            try
            {
                if (!_wabasLoaded && _realView != null)
                {
                    var wabasResult = await Task.Run(() => _wabas.GetAllAsync());
                    if (wabasResult.IsFailure)
                        throw new Exception(wabasResult.Error.Message);
                    var wabas = wabasResult.Value;
                    _realView.SetWabaSyncDataSource(wabas);
                    _wabasLoaded = true;

                    if (string.IsNullOrEmpty(_currentWabaFilter) && wabas != null && wabas.Count > 0)
                    {
                        _currentWabaFilter = wabas[0].WabaId;
                    }
                }

                if (string.IsNullOrEmpty(_currentWabaFilter))
                {
                    _data = new List<Template>();
                    _view.DataSource = _data;
                    return;
                }

                var dataResult = await Task.Run(() => _templates.GetAllAsync(search, _currentWabaFilter));
                if (dataResult.IsFailure)
                    throw new Exception(dataResult.Error.Message);
                _data = dataResult.Value;
                _view.DataSource = dataResult.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load templates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
                _isLoadingData = false;
            }
        }

        private async void OnWabaFilterChanged(object sender, string wabaId)
        {
            _currentWabaFilter = wabaId;
            if (!_isLoadingData && _wabasLoaded)
            {
                await LoadDataAsync();
            }
        }

        private async void OnSync(object sender, EventArgs e)
        {
            var wabaId = _realView?.SelectedWabaForSyncId;
            if (string.IsNullOrEmpty(wabaId))
            {
                MessageBox.Show("Silakan pilih WABA terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Yakin ingin sinkronisasi template dari Meta?", "Sinkronisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _view.IsLoading = true;
                try
                {
                    var sync = await Task.Run(() => _templates.SyncAsync(wabaId));
                    if (sync.IsFailure)
                        throw new Exception(sync.Error.Message);
                    await LoadDataAsync();
                    MessageBox.Show("Sinkronisasi selesai.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Sinkronisasi gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _view.IsLoading = false;
                }
            }
        }

        private void OnPreview(object sender, string templateId)
        {
            if (string.IsNullOrEmpty(templateId)) return;

            var url = _messagesUrl + $"templates/preview/{templateId}";
            var previewView = _realView?.GetOrCreatePreviewView();
            if (previewView == null) return;

            if (_embeddedMsgPresenter == null)
            {
                _embeddedMsgPresenter = new MessagesPresenter(previewView, _bus, _auth, url, _apiBaseUrl);
            }
            else
            {
                previewView.Url = url;
            }
        }

        private void OnAdd(object sender, EventArgs e)
        {
            var key = $"template_create_{_currentWabaFilter}";
            _bus.Publish(new RequestOpenTabMessage(key, "Template Baru"));
        }

        private void OnEdit(object sender, string templateId)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            var item = _data.Where(p => p.Id == templateId).FirstOrDefault();
            if(item == null) return;
            var key = $"template_detail_{item.Id}";
            _bus.Publish(new RequestOpenTabMessage(key, item.Name ?? item.Id));
        }

        private void OnUserDeletedRow(object sender, string templateId)
        {
            if (!string.IsNullOrEmpty(templateId) && !_pendingDeletions.Contains(templateId))
                _pendingDeletions.Add(templateId);
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            if (_pendingDeletions.Count == 0)
            {
                MessageBox.Show("Tidak ada perubahan untuk disimpan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Simpan perubahan? {_pendingDeletions.Count} template akan dihapus permanen dari Meta.",
                "Konfirmasi Simpan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _view.IsLoading = true;
            var total = _pendingDeletions.Count;
            var failed = 0;
            try
            {
                foreach (var id in _pendingDeletions.ToList())
                {
                    try
                    {
                        var del = await _templates.DeleteAsync(id);
                        if (del.IsFailure)
                            failed++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                _pendingDeletions.Clear();
                await LoadDataAsync();

                if (failed == 0)
                    MessageBox.Show($"Berhasil menghapus {total} template.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"{total - failed} berhasil, {failed} gagal.", "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _data = null;
                _view.RefreshClicked -= null;
                _view.SearchClicked -= null;
                _view.AddClicked -= null;
                _view.EditClicked -= null;
                _view.DeleteClicked -= null;

                if (_realView != null)
                {
                    _realView.SyncClicked -= OnSync;
                    _realView.WabaFilterChanged -= OnWabaFilterChanged;
                    _realView.PreviewClicked -= OnPreview;
                    _realView.UserDeletedRowItem -= OnUserDeletedRow;
                }

                _embeddedMsgPresenter?.Dispose();

                _disposed = true;
            }
        }
    }
}
