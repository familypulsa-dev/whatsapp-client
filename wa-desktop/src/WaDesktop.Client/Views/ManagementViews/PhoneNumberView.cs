using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Extensions;

namespace WaDesktop.Client.Views.ManagementViews
{
    public partial class PhoneNumberView : UserControl, IManagementView<PhoneNumberDetail>
    {
        public PhoneNumberView()
        {
            InitializeComponent();
        }

        public IList<PhoneNumberDetail> DataSource
        {
            set
            {
                this.InvokeIfRequired(() =>
                {
                    dataGridView.Rows.Clear();
                    foreach (var p in value)
                        dataGridView.Rows.Add(p.PhoneNumberId, p.DisplayPhone, p.DisplayName, p.QualityRating, p.CreatedAt,
                            FormatStatus(p.NameStatus), FormatStatus(p.CodeVerificationStatus), FormatStatus(p.MetaStatus));
                });
            }
        }

        public int SelectedIndex => dataGridView.SelectedRows.Count > 0 ? dataGridView.SelectedRows[0].Index : -1;
        public bool IsLoading { set => this.InvokeIfRequired(() => Cursor = value ? Cursors.WaitCursor : Cursors.Default); }

        public event EventHandler<string> SearchClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler AddClicked;
        public event EventHandler<string> EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler SaveClicked; // dummy for IManagementView constraint if any
        public event EventHandler SyncClicked;
        public event EventHandler<string> WabaFilterChanged;
        public event EventHandler<string> RegisterClicked;

        // Dummy SaveClicked handler (tidak terpakai, tapi wajib ada jika IManagementView punya SaveClicked)
        private void btnSave_Click(object sender, EventArgs e) => SaveClicked?.Invoke(this, EventArgs.Empty);

        public void SetWabaSyncDataSource(IList<Waba> wabas)
        {
            this.InvokeIfRequired(() =>
            {
                // Gunakan cmbWabaSync (pastikan Anda membuatnya di Visual Studio Designer)
                if (Controls.Find("cmbWabaSync", true).Length > 0)
                {
                    var cb = Controls.Find("cmbWabaSync", true)[0] as ComboBox;
                    if (cb != null)
                    {
                        // Temporarily detach event to prevent triggering during load
                        cb.SelectedIndexChanged -= CmbWabaSync_SelectedIndexChanged;

                        cb.DataSource = wabas;
                        cb.DisplayMember = "Name";
                        cb.ValueMember = "WabaId";
                        if (wabas != null && wabas.Count > 0) cb.SelectedIndex = 0;

                        // Attach event back
                        cb.SelectedIndexChanged += CmbWabaSync_SelectedIndexChanged;
                    }
                }
            });
        }

        private void CmbWabaSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            WabaFilterChanged?.Invoke(this, cb?.SelectedValue?.ToString());
        }

        public string SelectedWabaForSyncId
        {
            get
            {
                if (Controls.Find("cmbWabaSync", true).Length > 0)
                {
                    var cb = Controls.Find("cmbWabaSync", true)[0] as ComboBox;
                    return cb?.SelectedValue?.ToString();
                }
                return null;
            }
        }

        private void btnSync_Click(object sender, EventArgs e) => SyncClicked?.Invoke(this, EventArgs.Empty);

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var phoneId = dataGridView.SelectedRows[0].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(phoneId))
                    RegisterClicked?.Invoke(this, phoneId);
            }
            else
            {
                MessageBox.Show("Pilih nomor telepon yang ingin dilanjutkan registrasinya.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

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
            var phoneId = dataGridView.Rows[e.RowIndex].Cells[0].Value?.ToString();
            if (!string.IsNullOrEmpty(phoneId))
                EditClicked?.Invoke(this, phoneId);
        }

        private static string FormatStatus(string s) => string.IsNullOrEmpty(s) ? "-" : s;
    }
}
