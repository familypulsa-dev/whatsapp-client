using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Views
{
    public partial class PhoneRegistrationDialog : Form
    {
        private readonly IApiClient _api;
        private readonly string _wabaId;

        public string PhoneNumberId { get; private set; }

        public PhoneRegistrationDialog(IApiClient api, string wabaId)
        {
            InitializeComponent();
            _api = api;
            _wabaId = wabaId;
            ShowStep(1);
        }

        public PhoneRegistrationDialog(IApiClient api, string wabaId, string phoneNumberId)
        {
            InitializeComponent();
            _api = api;
            _wabaId = wabaId;
            PhoneNumberId = phoneNumberId;
            lblPhoneId.Text = $"Phone Number ID: {PhoneNumberId}";
            ShowStep(2);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            SetLoading(true);
            try
            {
                switch (_currentStep)
                {
                    case 1:
                        await Step1_CreateNumber();
                        break;
                    case 2:
                        await Step2_RequestCode();
                        break;
                    case 3:
                        await Step3_VerifyCode();
                        break;
                    case 4:
                        await Step4_RegisterPhone();
                        return;
                }
                if (_currentStep < 4)
                    ShowStep(_currentStep + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (_currentStep > 1)
                ShowStep(_currentStep - 1);
        }

        private async Task Step1_CreateNumber()
        {
            var req = new CreatePhoneNumberRequest
            {
                Cc = txtCC.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                VerifiedName = txtVerifiedName.Text.Trim(),
            };
            if (string.IsNullOrEmpty(req.Cc) || string.IsNullOrEmpty(req.PhoneNumber) || string.IsNullOrEmpty(req.VerifiedName))
                throw new Exception("Semua field di Step 1 wajib diisi.");

            var resp = await _api.CreatePhoneNumberAsync(_wabaId, req);
            PhoneNumberId = resp.PhoneNumberId;
            lblPhoneId.Text = $"Phone Number ID: {PhoneNumberId}";
        }

        private async Task Step2_RequestCode()
        {
            var method = rbSMS.Checked ? "SMS" : "VOICE";
            await _api.RequestVerificationCodeAsync(PhoneNumberId, new RequestCodeRequest
            {
                CodeMethod = method,
                Language = "en_US"
            });
            MessageBox.Show(this, "Kode verifikasi berhasil dikirim.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task Step3_VerifyCode()
        {
            var code = txtCode.Text.Trim().Replace("-", "").Replace(" ", "");
            if (string.IsNullOrEmpty(code))
                throw new Exception("Kode verifikasi wajib diisi.");

            await _api.VerifyCodeAsync(PhoneNumberId, new VerifyCodeRequest { Code = code });
            MessageBox.Show(this, "Kode verifikasi berhasil dikonfirmasi.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task Step4_RegisterPhone()
        {
            var pin = txtPin.Text.Trim();
            if (pin.Length != 6 || !int.TryParse(pin, out _))
                throw new Exception("PIN harus 6 digit angka.");

            var confirmPin = txtConfirmPin.Text.Trim();
            if (pin != confirmPin)
                throw new Exception("PIN dan konfirmasi PIN tidak cocok.");

            await _api.RegisterPhoneAsync(PhoneNumberId, new RegisterPhoneRequest { Pin = pin });
            MessageBox.Show(this, "Nomor telepon berhasil didaftarkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private int _currentStep;
        private void ShowStep(int step)
        {
            _currentStep = step;
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
        }

        private void SetLoading(bool loading)
        {
            Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            btnNext.Enabled = !loading;
            btnBack.Enabled = !loading;
            btnCancel.Enabled = !loading;
        }
    }
}
