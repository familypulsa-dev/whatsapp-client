using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Extensions;

namespace WaDesktop.Client.Views.ManagementViews
{
    /// <summary>
    /// Grid WABA bersifat read-only: tidak ada proses update dari sini.
    /// Penugasan company (kolom Server) dikelola lewat CompanyView.
    /// </summary>
    public partial class WabaView : UserControl, IManagementView<Waba>
    {
        public WabaView()
        {
            InitializeComponent();
        }

        public IList<Waba> DataSource
        {
            set
            {
                this.InvokeIfRequired(() =>
                {
                    dataGridView.Rows.Clear();

                    foreach (var w in value)
                    {
                        int idx = dataGridView.Rows.Add(
                            w.WabaId, w.Name,
                            string.IsNullOrEmpty(w.CompanyName) ? "-" : w.CompanyName,
                            w.CreatedAt
                        );
                        dataGridView.Rows[idx].DefaultCellStyle.BackColor = Color.White;
                        dataGridView.Rows[idx].Tag = w;
                    }
                });
            }
        }

        public int SelectedIndex => dataGridView.SelectedRows.Count > 0 ? dataGridView.SelectedRows[0].Index : -1;

        public string SelectedWabaId
        {
            get
            {
                try
                {
                    if (SelectedIndex < 0) return null;
                    return dataGridView.Rows[SelectedIndex].Cells["WabaId"].Value?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting selected WABA ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        public bool IsLoading { set => this.InvokeIfRequired(() => Cursor = value ? Cursors.WaitCursor : Cursors.Default); }

        public event EventHandler<string> SearchClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler AddClicked;
        public event EventHandler<string> EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler SyncClicked;

        private void btnSearch_Click(object sender, EventArgs e) => SearchClicked?.Invoke(this, txtSearch.Text);
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        private void btnAdd_Click(object sender, EventArgs e) => AddClicked?.Invoke(this, EventArgs.Empty);
        private void btnDelete_Click(object sender, EventArgs e) => DeleteClicked?.Invoke(this, EventArgs.Empty);
        private void btnSync_Click(object sender, EventArgs e) => SyncClicked?.Invoke(this, EventArgs.Empty);

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) SearchClicked?.Invoke(this, txtSearch.Text);
        }
    }
}
