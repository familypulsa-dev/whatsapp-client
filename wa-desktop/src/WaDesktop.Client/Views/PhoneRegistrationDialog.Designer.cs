namespace WaDesktop.Client.Views
{
    partial class PhoneRegistrationDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            // Step 1
            this.panelStep1 = new System.Windows.Forms.Panel();
            this.lblCC = new System.Windows.Forms.Label();
            this.txtCC = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.lblVerifiedName = new System.Windows.Forms.Label();
            this.txtVerifiedName = new System.Windows.Forms.TextBox();
            this.lblPhoneId = new System.Windows.Forms.Label();

            // Step 2
            this.panelStep2 = new System.Windows.Forms.Panel();
            this.rbSMS = new System.Windows.Forms.RadioButton();
            this.rbVOICE = new System.Windows.Forms.RadioButton();

            // Step 3
            this.panelStep3 = new System.Windows.Forms.Panel();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();

            // Step 4
            this.panelStep4 = new System.Windows.Forms.Panel();
            this.lblPin = new System.Windows.Forms.Label();
            this.txtPin = new System.Windows.Forms.TextBox();
            this.lblConfirmPin = new System.Windows.Forms.Label();
            this.txtConfirmPin = new System.Windows.Forms.TextBox();

            SuspendLayout();

            // form
            this.Text = "Registrasi Nomor Telepon WABA";
            this.ClientSize = new System.Drawing.Size(480, 320);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            // lblTitle
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Size = new System.Drawing.Size(440, 24);
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Text = "Step 1: Buat Nomor Telepon";

            // lblDescription
            this.lblDescription.Location = new System.Drawing.Point(20, 44);
            this.lblDescription.Size = new System.Drawing.Size(440, 36);
            this.lblDescription.Text = "Deskripsi langkah.";

            // ── panelStep1 ──
            this.panelStep1.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblCC, txtCC,
                lblPhoneNumber, txtPhoneNumber,
                lblVerifiedName, txtVerifiedName,
                lblPhoneId
            });
            this.panelStep1.Location = new System.Drawing.Point(20, 84);
            this.panelStep1.Size = new System.Drawing.Size(440, 160);

            this.lblCC.Text = "Kode Negara (CC):";
            this.lblCC.Location = new System.Drawing.Point(0, 4);
            this.lblCC.Size = new System.Drawing.Size(140, 20);
            this.txtCC.Location = new System.Drawing.Point(140, 2);
            this.txtCC.Size = new System.Drawing.Size(80, 22);
            this.txtCC.Text = "62";

            this.lblPhoneNumber.Text = "Nomor Telepon:";
            this.lblPhoneNumber.Location = new System.Drawing.Point(0, 32);
            this.lblPhoneNumber.Size = new System.Drawing.Size(140, 20);
            this.txtPhoneNumber.Location = new System.Drawing.Point(140, 30);
            this.txtPhoneNumber.Size = new System.Drawing.Size(280, 22);

            this.lblVerifiedName.Text = "Nama Tampilan:";
            this.lblVerifiedName.Location = new System.Drawing.Point(0, 60);
            this.lblVerifiedName.Size = new System.Drawing.Size(140, 20);
            this.txtVerifiedName.Location = new System.Drawing.Point(140, 58);
            this.txtVerifiedName.Size = new System.Drawing.Size(280, 22);

            this.lblPhoneId.Location = new System.Drawing.Point(0, 92);
            this.lblPhoneId.Size = new System.Drawing.Size(440, 20);
            this.lblPhoneId.Text = "";

            // ── panelStep2 ──
            this.panelStep2.Controls.AddRange(new System.Windows.Forms.Control[] { rbSMS, rbVOICE });
            this.panelStep2.Location = new System.Drawing.Point(20, 84);
            this.panelStep2.Size = new System.Drawing.Size(440, 160);

            this.rbSMS.Text = "SMS";
            this.rbSMS.Location = new System.Drawing.Point(0, 8);
            this.rbSMS.Size = new System.Drawing.Size(200, 28);
            this.rbSMS.Checked = true;

            this.rbVOICE.Text = "Panggilan Suara (Voice)";
            this.rbVOICE.Location = new System.Drawing.Point(0, 40);
            this.rbVOICE.Size = new System.Drawing.Size(200, 28);

            // ── panelStep3 ──
            this.panelStep3.Controls.AddRange(new System.Windows.Forms.Control[] { lblCode, txtCode });
            this.panelStep3.Location = new System.Drawing.Point(20, 84);
            this.panelStep3.Size = new System.Drawing.Size(440, 160);

            this.lblCode.Text = "Kode Verifikasi (tanpa strip):";
            this.lblCode.Location = new System.Drawing.Point(0, 8);
            this.lblCode.Size = new System.Drawing.Size(200, 20);
            this.txtCode.Location = new System.Drawing.Point(0, 32);
            this.txtCode.Size = new System.Drawing.Size(280, 22);

            // ── panelStep4 ──
            this.panelStep4.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPin, txtPin,
                lblConfirmPin, txtConfirmPin
            });
            this.panelStep4.Location = new System.Drawing.Point(20, 84);
            this.panelStep4.Size = new System.Drawing.Size(440, 160);

            this.lblPin.Text = "PIN 6 Digit:";
            this.lblPin.Location = new System.Drawing.Point(0, 8);
            this.lblPin.Size = new System.Drawing.Size(140, 20);
            this.txtPin.Location = new System.Drawing.Point(140, 6);
            this.txtPin.Size = new System.Drawing.Size(140, 22);
            this.txtPin.PasswordChar = '*';
            this.txtPin.MaxLength = 6;

            this.lblConfirmPin.Text = "Konfirmasi PIN:";
            this.lblConfirmPin.Location = new System.Drawing.Point(0, 40);
            this.lblConfirmPin.Size = new System.Drawing.Size(140, 20);
            this.txtConfirmPin.Location = new System.Drawing.Point(140, 38);
            this.txtConfirmPin.Size = new System.Drawing.Size(140, 22);
            this.txtConfirmPin.PasswordChar = '*';
            this.txtConfirmPin.MaxLength = 6;

            // ── Buttons ──
            this.btnBack.Text = "< Kembali";
            this.btnBack.Location = new System.Drawing.Point(20, 260);
            this.btnBack.Size = new System.Drawing.Size(100, 32);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            this.btnNext.Text = "Selanjutnya >";
            this.btnNext.Location = new System.Drawing.Point(360, 260);
            this.btnNext.Size = new System.Drawing.Size(100, 32);
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);

            this.btnCancel.Text = "Batal";
            this.btnCancel.Location = new System.Drawing.Point(126, 260);
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Controls
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, lblDescription,
                panelStep1, panelStep2, panelStep3, panelStep4,
                btnBack, btnNext, btnCancel
            });

            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Panel panelStep1;
        private System.Windows.Forms.Panel panelStep2;
        private System.Windows.Forms.Panel panelStep3;
        private System.Windows.Forms.Panel panelStep4;
        private System.Windows.Forms.Label lblCC;
        private System.Windows.Forms.TextBox txtCC;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label lblVerifiedName;
        private System.Windows.Forms.TextBox txtVerifiedName;
        private System.Windows.Forms.Label lblPhoneId;
        private System.Windows.Forms.RadioButton rbSMS;
        private System.Windows.Forms.RadioButton rbVOICE;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblPin;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Label lblConfirmPin;
        private System.Windows.Forms.TextBox txtConfirmPin;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnCancel;
    }
}
