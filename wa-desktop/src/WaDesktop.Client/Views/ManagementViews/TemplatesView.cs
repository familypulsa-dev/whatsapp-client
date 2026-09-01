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
        public TemplatesView()
        {
            InitializeComponent();
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

        private void CmbWabaSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            WabaFilterChanged?.Invoke(this, cmbWabaSync?.SelectedValue?.ToString());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                RefreshClicked?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnSync_Click(object sender, EventArgs e) => SyncClicked?.Invoke(this, EventArgs.Empty);
        private void btnSearch_Click(object sender, EventArgs e) => SearchClicked?.Invoke(this, txtSearch.Text);
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) SearchClicked?.Invoke(this, txtSearch.Text);
        }
    }
}
