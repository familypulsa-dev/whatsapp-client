using System;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Client.Extensions;
using System.Globalization;

namespace WaDesktop.Client.Views
{
    public partial class LimitBillingView : Form, ILimitBillingView
    {
        private bool _isUpdatingText = false;
        private readonly CultureInfo _idCulture = new CultureInfo("id-ID");

        public LimitBillingView()
        {
            InitializeComponent();

            tbMarketing.TextChanged += OnTextBoxTextChanged;
            tbUtility.TextChanged += OnTextBoxTextChanged;
            tbAuthentication.TextChanged += OnTextBoxTextChanged;
            tbService.TextChanged += OnTextBoxTextChanged;

            btnSimpan.Click += (s, e) => SaveClicked?.Invoke(this, EventArgs.Empty);
            
            // Set harga hardcode info ke label cost
            lbCostMarketing.Text = "Rp 586,33";
            lbCostUtility.Text = "Rp 356,65";
            lbCostAuthentication.Text = "Rp 356,65";
            lbCostService.Text = "Rp 0,00";
        }

        public event EventHandler SaveClicked;
        public event EventHandler LimitsChanged;

        public bool IsLoading
        {
            set => this.InvokeIfRequired(() => Cursor = value ? Cursors.WaitCursor : Cursors.Default);
        }

        public void CloseDialog(bool success)
        {
            this.InvokeIfRequired(() =>
            {
                this.DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            });
        }

        public void ShowError(string message)
        {
            this.InvokeIfRequired(() => MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        public void ShowMessage(string message)
        {
            this.InvokeIfRequired(() => MessageBox.Show(this, message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information));
        }

        private void OnTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingText) return;
            LimitsChanged?.Invoke(this, EventArgs.Empty);
        }

        private int? ParseTextBox(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text)) return null;
            if (int.TryParse(tb.Text, out int val)) return val;
            return null;
        }

        private void SetTextBox(TextBox tb, int? value)
        {
            _isUpdatingText = true;
            tb.Text = value.HasValue ? value.Value.ToString() : "";
            _isUpdatingText = false;
        }

        public int? LimitMarketing { get => ParseTextBox(tbMarketing); set => SetTextBox(tbMarketing, value); }
        public int? LimitUtility { get => ParseTextBox(tbUtility); set => SetTextBox(tbUtility, value); }
        public int? LimitAuthentication { get => ParseTextBox(tbAuthentication); set => SetTextBox(tbAuthentication, value); }
        public int? LimitService { get => ParseTextBox(tbService); set => SetTextBox(tbService, value); }

        public decimal MaxMarketingCost { set => this.InvokeIfRequired(() => lbMaxMarketingCost.Text = value.ToString("C2", _idCulture)); }
        public decimal MaxUtilityCost { set => this.InvokeIfRequired(() => lbMaxUtilityCost.Text = value.ToString("C2", _idCulture)); }
        public decimal MaxAuthenticationCost { set => this.InvokeIfRequired(() => lbMaxAuthenticationCost.Text = value.ToString("C2", _idCulture)); }
        public decimal MaxServiceCost { set => this.InvokeIfRequired(() => lbMaxServiceCost.Text = value.ToString("C2", _idCulture)); }
        public decimal MaxTotalCost { set => this.InvokeIfRequired(() => lbMaxTotalCost.Text = value.ToString("C2", _idCulture)); }
    }
}
