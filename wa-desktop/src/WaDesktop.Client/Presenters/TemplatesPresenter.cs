using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;

namespace WaDesktop.Client.Presenters
{
    public class TemplatesPresenter : IDisposable
    {
        private readonly IManagementView<Template> _view;
        private readonly IApiClient _api;
        private List<Template> _data;
        private readonly IEventAggregator _bus;
        private bool _disposed;

        public TemplatesPresenter(IManagementView<Template> view, IApiClient api, IEventAggregator bus)
        {
            _view = view;
            _api = api;
            _bus = bus;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.AddClicked += OnAdd;
            _view.EditClicked += OnEdit;
            _view.DeleteClicked += OnDelete;
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            _view.IsLoading = true;
            try
            {
                var data = await Task.Run(() => _api.GetTemplatesAsync(search));
                _data = data;
                _view.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load templates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private void OnAdd(object sender, EventArgs e){
            //MessageBox.Show("Add Template — implement form dialog.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var key = $"template_create";
            _bus.Publish(new RequestOpenTabMessage(key, "Template Baru"));
        }

        private void OnEdit(object sender, EventArgs e)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            //MessageBox.Show("Edit Template — implement form dialog.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var item = _data[_view.SelectedIndex];
            var key = $"templatedetail_{item.Id}";
            _bus.Publish(new RequestOpenTabMessage(key, item.Name ?? item.Id));
        }

        private void OnDelete(object sender, EventArgs e)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            var confirm = MessageBox.Show("Hapus template?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
                MessageBox.Show("Delete Template — implement API call.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                _disposed = true;
            }
        }
    }
}
