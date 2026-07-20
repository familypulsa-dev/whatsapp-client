namespace WaDesktop.Client.Views
{
    partial class SoftwareUpdateView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.newVersion = new System.Windows.Forms.Label();
            this.currentVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.subtitleCanUpdate = new System.Windows.Forms.Label();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbLoader = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.lblNotes);
            this.groupBox1.Controls.Add(this.newVersion);
            this.groupBox1.Controls.Add(this.currentVersion);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.subtitleCanUpdate);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 257);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Version update";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 75);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(414, 178);
            this.textBox1.TabIndex = 6;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(9, 59);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(41, 13);
            this.lblNotes.TabIndex = 5;
            this.lblNotes.Text = "Notes :";
            // 
            // newVersion
            // 
            this.newVersion.AutoSize = true;
            this.newVersion.Location = new System.Drawing.Point(100, 46);
            this.newVersion.Name = "newVersion";
            this.newVersion.Size = new System.Drawing.Size(31, 13);
            this.newVersion.TabIndex = 4;
            this.newVersion.Text = "2.1.0";
            // 
            // currentVersion
            // 
            this.currentVersion.AutoSize = true;
            this.currentVersion.Location = new System.Drawing.Point(100, 33);
            this.currentVersion.Name = "currentVersion";
            this.currentVersion.Size = new System.Drawing.Size(31, 13);
            this.currentVersion.TabIndex = 3;
            this.currentVersion.Text = "1.1.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "New version :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current version :";
            // 
            // subtitleCanUpdate
            // 
            this.subtitleCanUpdate.AutoSize = true;
            this.subtitleCanUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subtitleCanUpdate.Location = new System.Drawing.Point(6, 16);
            this.subtitleCanUpdate.Name = "subtitleCanUpdate";
            this.subtitleCanUpdate.Size = new System.Drawing.Size(247, 13);
            this.subtitleCanUpdate.TabIndex = 0;
            this.subtitleCanUpdate.Text = "New version of SMS Gateway is available.";
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpgrade.Location = new System.Drawing.Point(278, 262);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 23);
            this.btnUpgrade.TabIndex = 10;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(356, 262);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(78, 23);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lbLoader
            // 
            this.lbLoader.AutoSize = true;
            this.lbLoader.Location = new System.Drawing.Point(10, 263);
            this.lbLoader.Name = "lbLoader";
            this.lbLoader.Size = new System.Drawing.Size(81, 13);
            this.lbLoader.TabIndex = 11;
            this.lbLoader.Text = "Fetching data...";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SoftwareUpdateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 288);
            this.Controls.Add(this.lbLoader);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoftwareUpdateView";
            this.Text = "Version update";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label newVersion;
        private System.Windows.Forms.Label currentVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label subtitleCanUpdate;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lbLoader;
        private System.Windows.Forms.Timer timer1;
    }
}