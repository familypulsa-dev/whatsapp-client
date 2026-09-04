using System.Drawing;
using System.Windows.Forms;

namespace WaDesktop.Shell
{
    partial class FooterView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FooterView));
            StatusStrip1 = new StatusStrip();
            ToolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            DropDownLog = new System.Windows.Forms.ToolStripDropDownButton();
            logAppShow = new System.Windows.Forms.ToolStripMenuItem();
            logAppHide = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripVersion = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripNamaServer = new System.Windows.Forms.ToolStripStatusLabel();
            ToolStripWaktu = new System.Windows.Forms.ToolStripStatusLabel();
            ToolStripUpdate = new System.Windows.Forms.ToolStripStatusLabel();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            StatusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // StatusStrip1
            // 
            StatusStrip1.AutoSize = false;
            StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ToolStripStatus, DropDownLog, ToolStripVersion, toolStripNamaServer, ToolStripWaktu, ToolStripUpdate });
            StatusStrip1.Location = new Point(0, 0);
            StatusStrip1.Name = "StatusStrip1";
            StatusStrip1.Padding = new Padding(1, 0, 16, 0);
            StatusStrip1.RenderMode = ToolStripRenderMode.Professional;
            StatusStrip1.Size = new Size(1183, 29);
            StatusStrip1.TabIndex = 70;
            StatusStrip1.Text = "StatusStrip2";
            // 
            // ToolStripStatus
            // 
            ToolStripStatus.Name = "ToolStripStatus";
            ToolStripStatus.Size = new Size(835, 24);
            ToolStripStatus.Spring = true;
            ToolStripStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // DropDownLog
            // 
            DropDownLog.DisplayStyle = ToolStripItemDisplayStyle.Text;
            DropDownLog.DropDownItems.AddRange(new ToolStripItem[] { logAppShow, logAppHide });
            DropDownLog.Image = (Image)resources.GetObject("DropDownLog.Image");
            DropDownLog.ImageTransparentColor = Color.Magenta;
            DropDownLog.Name = "DropDownLog";
            DropDownLog.Size = new Size(65, 27);
            DropDownLog.Text = "Log App";
            DropDownLog.Visible = false;
            // 
            // logAppShow
            // 
            logAppShow.Name = "logAppShow";
            logAppShow.Size = new Size(180, 22);
            logAppShow.Text = "Tampilkan";
            // 
            // logAppHide
            // 
            logAppHide.Name = "logAppHide";
            logAppHide.Size = new Size(180, 22);
            logAppHide.Text = "Sembunyikan";
            // 
            // ToolStripVersion
            // 
            ToolStripVersion.Name = "ToolStripVersion";
            ToolStripVersion.Size = new Size(49, 24);
            ToolStripVersion.Text = "v 1.0.0.0";
            ToolStripVersion.ToolTipText = "Product Version";
            // 
            // toolStripNamaServer
            // 
            toolStripNamaServer.Name = "toolStripNamaServer";
            toolStripNamaServer.Size = new Size(46, 24);
            toolStripNamaServer.Text = "ISTANA";
            toolStripNamaServer.ToolTipText = "Nama Server";
            // 
            // ToolStripWaktu
            // 
            ToolStripWaktu.Name = "ToolStripWaktu";
            ToolStripWaktu.Size = new Size(124, 24);
            ToolStripWaktu.Text = "dd/MM/yy HH:mm:ss";
            ToolStripWaktu.ToolTipText = "Database Time";
            // 
            // ToolStripUpdate
            // 
            ToolStripUpdate.AutoToolTip = true;
            ToolStripUpdate.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolStripUpdate.Image = (Image)resources.GetObject("ToolStripUpdate.Image");
            ToolStripUpdate.IsLink = true;
            ToolStripUpdate.LinkBehavior = LinkBehavior.NeverUnderline;
            ToolStripUpdate.Name = "ToolStripUpdate";
            ToolStripUpdate.Size = new Size(16, 24);
            ToolStripUpdate.Text = "New version";
            ToolStripUpdate.ToolTipText = "New version is available";
            ToolStripUpdate.Visible = false;
            ToolStripUpdate.Click += ToolStripUpdate_Click;
            ToolStripUpdate.MouseLeave += ToolStripUpdate_MouseLeave;
            ToolStripUpdate.MouseHover += ToolStripUpdate_MouseHover;
            // 
            // Footer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(StatusStrip1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Footer";
            Size = new Size(1183, 29);
            StatusStrip1.ResumeLayout(false);
            StatusStrip1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatus;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripNamaServer;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripWaktu;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripUpdate;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripDropDownButton DropDownLog;
        private System.Windows.Forms.ToolStripMenuItem logAppShow;
        private System.Windows.Forms.ToolStripMenuItem logAppHide;
    }
}
