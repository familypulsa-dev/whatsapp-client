using System;
using System.Windows.Forms;

namespace WaDesktop.Client.Views.ManagementViews
{
    public partial class WebhookInputDialog : Form
    {
        public WebhookInputDialog()
        {
            InitializeComponent();
        }

        public string DialogTitle
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        public string PhoneNumberId
        {
            get => lblPhoneId.Text;
            set => lblPhoneId.Text = $"Phone Number ID: {value}";
        }

        public string CurrentWebhookUrl
        {
            get => txtCurrentUrl.Text;
            set => txtCurrentUrl.Text = value;
        }

        public string WebhookUrl
        {
            get => txtWebhookUrl.Text;
            set => txtWebhookUrl.Text = value;
        }
    }
}
