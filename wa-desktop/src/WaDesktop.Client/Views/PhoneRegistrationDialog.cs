using System;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Client.Extensions;

namespace WaDesktop.Client.Views
{
    public partial class PhoneRegistrationDialog : Form, IPhoneRegistrationView
    {
        public PhoneRegistrationDialog()
        {
            InitializeComponent();
        }

        public string WabaId { get; set; }

        public string PhoneNumberId 
        { 
            get => lblPhoneId.Text.Replace("Phone Number ID: ", "").Trim(); 
            set => this.InvokeIfRequired(() => lblPhoneId.Text = $"Phone Number ID: {value}");
        }

        public string Cc => txtCC.Text;
        public string PhoneNumber => txtPhoneNumber.Text;
        public string VerifiedName => txtVerifiedName.Text;
        public string CodeMethod => rbSMS.Checked ? "SMS" : "VOICE";
        public string VerificationCode => txtCode.Text;
        public string Pin => txtPin.Text;
        public string ConfirmPin => txtConfirmPin.Text;

        public event EventHandler ProcessStepClicked;
        public event EventHandler BackStepClicked;
        public event EventHandler CancelClicked;

        private void btnNext_Click(object sender, EventArgs e) => ProcessStepClicked?.Invoke(this, EventArgs.Empty);
        private void btnBack_Click(object sender, EventArgs e) => BackStepClicked?.Invoke(this, EventArgs.Empty);
        private void btnCancel_Click(object sender, EventArgs e) => CancelClicked?.Invoke(this, EventArgs.Empty);

        public void ShowStep(int step)
        {
            this.InvokeIfRequired(() =>
            {
                panelStep1.Visible = step == 1;
                panelStep2.Visible = step == 2;
                panelStep3.Visible = step == 3;
                panelStep4.Visible = step == 4;

                switch (step)
                {
                    case 1:
                        lblTitle.Text = "Step 1: Buat Nomor Telepon";
                        lblDescription.Text = "Masukkan informasi nomor telepon untuk didaftarkan ke WABA.";
                        break;
                    case 2:
                        lblTitle.Text = "Step 2: Minta Kode Verifikasi";
                        lblDescription.Text = "Pilih metode pengiriman kode verifikasi (SMS atau panggilan suara).";
                        break;
                    case 3:
                        lblTitle.Text = "Step 3: Verifikasi Kode OTP";
                        lblDescription.Text = "Masukkan kode verifikasi yang diterima melalui SMS atau panggilan suara.";
                        break;
                    case 4:
                        lblTitle.Text = "Step 4: Daftarkan dengan PIN";
                        lblDescription.Text = "Tetapkan PIN 6 digit untuk Two-Step Verification dan mendaftarkan nomor.";
                        break;
                    default:
                        lblTitle.Text = "";
                        lblDescription.Text = "";
                        break;
                }

                btnBack.Visible = step > 1;
                btnNext.Text = step < 4 ? "Selanjutnya >" : "Daftarkan";
            });
        }

        public void SetLoading(bool loading)
        {
            this.InvokeIfRequired(() =>
            {
                Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
                btnNext.Enabled = !loading;
                btnBack.Enabled = !loading;
                btnCancel.Enabled = !loading;
            });
        }

        public void ShowMessage(string title, string message)
        {
            this.InvokeIfRequired(() => MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information));
        }

        public void ShowError(string title, string error)
        {
            this.InvokeIfRequired(() => MessageBox.Show(this, error, title, MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        public void CloseDialog(bool isSuccess)
        {
            this.InvokeIfRequired(() =>
            {
                DialogResult = isSuccess ? DialogResult.OK : DialogResult.Cancel;
                Close();
            });
        }
    }
}
