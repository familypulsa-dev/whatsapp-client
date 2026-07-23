using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Extensions;

namespace WaDesktop.Client.Views.ManagementViews
{
    public partial class TemplatesView : UserControl, IManagementView<Template>
    {
        private MessagesView _previewView;
        private readonly Label _lblEmptyPreview;

        public TemplatesView()
        {
            InitializeComponent();

            _lblEmptyPreview = new Label
            {
                Text = "Pilih baris dan klik Preview untuk melihat detail.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.Gray
            };
            splitContainer1.Panel2.Controls.Add(_lblEmptyPreview);
        }

        public IList<Template> DataSource
        {
            set
            {
                this.InvokeIfRequired(() =>
                {
                    dataGridView.Rows.Clear();
                    foreach (var t in value)
                        dataGridView.Rows.Add(t.Id, t.Name, t.Language, t.Status, t.Category);
                });
            }
        }

        public int SelectedIndex => dataGridView.SelectedRows.Count > 0 ? dataGridView.SelectedRows[0].Index : -1;
        public IList<int> SelectedIndices
        {
            get
            {
                var indices = new List<int>();
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    indices.Add(row.Index);
                return indices;
            }
        }
        public bool IsLoading { set => this.InvokeIfRequired(() => Cursor = value ? Cursors.WaitCursor : Cursors.Default); }

        public event EventHandler<string> SearchClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler AddClicked;
        public event EventHandler<string> EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler SyncClicked;
        public event EventHandler<string> WabaFilterChanged;
        public event EventHandler<string> PreviewClicked;
        public event EventHandler<string> UserDeletedRowItem;

        public void SetWabaSyncDataSource(IList<Waba> wabas)
        {
            this.InvokeIfRequired(() =>
            {
                cmbWabaSync.SelectedIndexChanged -= CmbWabaSync_SelectedIndexChanged;

                cmbWabaSync.DataSource = wabas;
                cmbWabaSync.DisplayMember = "Name";
                cmbWabaSync.ValueMember = "WabaId";
                if (wabas != null && wabas.Count > 0) cmbWabaSync.SelectedIndex = 0;

                cmbWabaSync.SelectedIndexChanged += CmbWabaSync_SelectedIndexChanged;
            });
        }

        public string SelectedWabaForSyncId
        {
            get
            {
                return cmbWabaSync?.SelectedValue?.ToString();
            }
        }

        public IMessagesView GetOrCreatePreviewView()
        {
            if (_previewView == null)
            {
                _lblEmptyPreview.Visible = false;
                _previewView = new MessagesView
                {
                    Dock = DockStyle.Fill
                };
                splitContainer1.Panel2.Controls.Add(_previewView);
            }
            return _previewView;
        }

        private void CmbWabaSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            WabaFilterChanged?.Invoke(this, cmbWabaSync?.SelectedValue?.ToString());
        }

        private void btnSync_Click(object sender, EventArgs e) => SyncClicked?.Invoke(this, EventArgs.Empty);
        private void btnSearch_Click(object sender, EventArgs e) => SearchClicked?.Invoke(this, txtSearch.Text);
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        private void btnAdd_Click(object sender, EventArgs e) => AddClicked?.Invoke(this, EventArgs.Empty);
        private void btnDelete_Click(object sender, EventArgs e) => DeleteClicked?.Invoke(this, EventArgs.Empty);
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) SearchClicked?.Invoke(this, txtSearch.Text);
        }
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) {
                var templateId = dataGridView.Rows[e.RowIndex].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(templateId))
                {
                    EditClicked?.Invoke(this, templateId); 
                }
            }
        }
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView.Columns[e.ColumnIndex].Name == "colPreview")
            {
                var templateId = dataGridView.Rows[e.RowIndex].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(templateId))
                    PreviewClicked?.Invoke(this, templateId);
            }
        }
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var templateId = e.Row.Cells[0].Value?.ToString();
            if (!string.IsNullOrEmpty(templateId))
                UserDeletedRowItem?.Invoke(this, templateId);
        }
    }
}
