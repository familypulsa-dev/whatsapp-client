using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WaDesktop.Client.Extensions;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

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
                    {
                        int idx = dataGridView.Rows.Add(p.PhoneNumberId, p.DisplayPhone, p.DisplayName, p.QualityRating, p.CreatedAt,
                            FormatStatus(p.NameStatus), FormatStatus(p.CodeVerificationStatus), FormatStatus(p.MetaStatus));
                        dataGridView.Rows[idx].Tag = p;
                    }
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
        public event EventHandler<string> WebhookClicked;

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

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView.Columns[e.ColumnIndex].Name == colRegister.Name)
            {
                var row = dataGridView.Rows[e.RowIndex];
                var phoneId = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(phoneId))
                    RegisterClicked?.Invoke(this, phoneId);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                RefreshClicked?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static string FormatStatus(string s) => string.IsNullOrEmpty(s) ? "-" : s;

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView.ClearSelection();
                dataGridView.Rows[e.RowIndex].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[0];
                ctxMenu.Show(dataGridView, e.Location);
            }
        }

        private void ctxMenuWebhook_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var phoneId = dataGridView.SelectedRows[0].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(phoneId))
                    WebhookClicked?.Invoke(this, phoneId);
            }
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 1. Cek kolom button Anda
            if (e.RowIndex >= 0 && dataGridView.Columns[e.ColumnIndex].Name == colRegister.Name)
            {
                // 2. Ambil cell asli dan cast "as DataGridViewButtonCell"
                var buttonCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                if (buttonCell != null)
                {
                    // 3. Cek kondisi data dari kolom lain
                    string status = dataGridView.Rows[e.RowIndex].Cells[colRegister.Name].Value?.ToString();

                    object data = dataGridView.Rows[e.RowIndex].Tag;

                    status = ParseStatus(data);

                    if (status == "")
                    {
                        e.Value = ""; // Ubah label teks langsung via event argumen
                        buttonCell.FlatStyle = FlatStyle.Flat; 
                        e.CellStyle.SelectionForeColor = Color.White;
                    }
                    else
                    {
                        e.Value = status; // Teks saat aktif
                        buttonCell.FlatStyle = FlatStyle.Standard; // Gaya tombol normal
                        e.CellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private string ParseStatus(object tag)
        {
            if (tag is PhoneNumberDetail phone)
            {
                if(phone.MetaStatus == "CONNECTED")
                {
                    return "";
                }
                else if(phone.MetaStatus != "CONNECTED" && phone.CodeVerificationStatus == "VERIFIED")
                {
                    return "Input PIN";
                }else if(phone.MetaStatus != "CONNECTED")
                {
                    return "Register";
                }
                
            }

            return "-";
        }
    }
}
