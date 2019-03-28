namespace UpdateAppLibrary
{
    partial class NewRelease
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
            if (disposing && (components != null)) {
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
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.linkLabel = new System.Windows.Forms.LinkLabel();
      this.labelNewVersion = new System.Windows.Forms.Label();
      this.buttonUpdate = new System.Windows.Forms.Button();
      this.buttonRemindeMe = new System.Windows.Forms.Button();
      this.buttonIgnore = new System.Windows.Forms.Button();
      this.buttonRelease = new System.Windows.Forms.Button();
      this.labelLinkDecoration = new System.Windows.Forms.Label();
      this.checkUpdate = new System.Windows.Forms.CheckBox();
      this.labelVersion = new System.Windows.Forms.Label();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.DownloadLabel = new System.Windows.Forms.Label();
      this.CancelDownloadButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(224, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Updates are avaiable for the xxxxx Application";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(108, 39);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(0, 13);
      this.label3.TabIndex = 2;
      // 
      // linkLabel
      // 
      this.linkLabel.AutoSize = true;
      this.linkLabel.Location = new System.Drawing.Point(74, 61);
      this.linkLabel.Name = "linkLabel";
      this.linkLabel.Size = new System.Drawing.Size(138, 13);
      this.linkLabel.TabIndex = 3;
      this.linkLabel.TabStop = true;
      this.linkLabel.Text = "http://www.yoctopuce.com";
      this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
      // 
      // labelNewVersion
      // 
      this.labelNewVersion.AutoSize = true;
      this.labelNewVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelNewVersion.Location = new System.Drawing.Point(114, 39);
      this.labelNewVersion.Name = "labelNewVersion";
      this.labelNewVersion.Size = new System.Drawing.Size(64, 13);
      this.labelNewVersion.TabIndex = 4;
      this.labelNewVersion.Text = "1.10.1234";
      // 
      // buttonUpdate
      // 
      this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonUpdate.Location = new System.Drawing.Point(12, 114);
      this.buttonUpdate.Name = "buttonUpdate";
      this.buttonUpdate.Size = new System.Drawing.Size(128, 23);
      this.buttonUpdate.TabIndex = 6;
      this.buttonUpdate.Text = "Update Now";
      this.buttonUpdate.UseVisualStyleBackColor = true;
      this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
      // 
      // buttonRemindeMe
      // 
      this.buttonRemindeMe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRemindeMe.Location = new System.Drawing.Point(387, 114);
      this.buttonRemindeMe.Name = "buttonRemindeMe";
      this.buttonRemindeMe.Size = new System.Drawing.Size(108, 23);
      this.buttonRemindeMe.TabIndex = 7;
      this.buttonRemindeMe.Text = "Remind Me Later";
      this.buttonRemindeMe.UseVisualStyleBackColor = true;
      this.buttonRemindeMe.Click += new System.EventHandler(this.buttonRemindeMe_Click);
      // 
      // buttonIgnore
      // 
      this.buttonIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonIgnore.Location = new System.Drawing.Point(270, 114);
      this.buttonIgnore.Name = "buttonIgnore";
      this.buttonIgnore.Size = new System.Drawing.Size(111, 23);
      this.buttonIgnore.TabIndex = 8;
      this.buttonIgnore.Text = "Ignore This Update ";
      this.buttonIgnore.UseVisualStyleBackColor = true;
      this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
      // 
      // buttonRelease
      // 
      this.buttonRelease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRelease.Location = new System.Drawing.Point(146, 114);
      this.buttonRelease.Name = "buttonRelease";
      this.buttonRelease.Size = new System.Drawing.Size(118, 23);
      this.buttonRelease.TabIndex = 9;
      this.buttonRelease.Text = "Release Notes";
      this.buttonRelease.UseVisualStyleBackColor = true;
      this.buttonRelease.Click += new System.EventHandler(this.buttonRelease_Click);
      // 
      // labelLinkDecoration
      // 
      this.labelLinkDecoration.AutoSize = true;
      this.labelLinkDecoration.Location = new System.Drawing.Point(38, 61);
      this.labelLinkDecoration.Name = "labelLinkDecoration";
      this.labelLinkDecoration.Size = new System.Drawing.Size(30, 13);
      this.labelLinkDecoration.TabIndex = 10;
      this.labelLinkDecoration.Text = "Link:";
      // 
      // checkUpdate
      // 
      this.checkUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkUpdate.AutoSize = true;
      this.checkUpdate.Location = new System.Drawing.Point(15, 91);
      this.checkUpdate.Name = "checkUpdate";
      this.checkUpdate.Size = new System.Drawing.Size(177, 17);
      this.checkUpdate.TabIndex = 11;
      this.checkUpdate.Text = "Automatically check for updates";
      this.checkUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
      this.checkUpdate.UseVisualStyleBackColor = true;
      // 
      // labelVersion
      // 
      this.labelVersion.AutoSize = true;
      this.labelVersion.Location = new System.Drawing.Point(38, 38);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new System.Drawing.Size(69, 13);
      this.labelVersion.TabIndex = 12;
      this.labelVersion.Text = "New version:";
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(75, 46);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(377, 23);
      this.progressBar1.TabIndex = 13;
      // 
      // DownloadLabel
      // 
      this.DownloadLabel.Location = new System.Drawing.Point(-1, 76);
      this.DownloadLabel.Name = "DownloadLabel";
      this.DownloadLabel.Size = new System.Drawing.Size(507, 19);
      this.DownloadLabel.TabIndex = 14;
      this.DownloadLabel.Text = "Downloading update, please wait..";
      this.DownloadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // CancelButton
      // 
      this.CancelDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.CancelDownloadButton.Location = new System.Drawing.Point(387, 114);
      this.CancelDownloadButton.Name = "CancelButton";
      this.CancelDownloadButton.Size = new System.Drawing.Size(108, 23);
      this.CancelDownloadButton.TabIndex = 15;
      this.CancelDownloadButton.Text = "Cancel";
      this.CancelDownloadButton.UseVisualStyleBackColor = true;
      this.CancelDownloadButton.Click += new System.EventHandler(this.CancelButton_Click);
      // 
      // NewRelease
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(507, 145);
      this.Controls.Add(this.CancelDownloadButton);
      this.Controls.Add(this.DownloadLabel);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.labelVersion);
      this.Controls.Add(this.checkUpdate);
      this.Controls.Add(this.labelLinkDecoration);
      this.Controls.Add(this.buttonRelease);
      this.Controls.Add(this.buttonIgnore);
      this.Controls.Add(this.buttonRemindeMe);
      this.Controls.Add(this.buttonUpdate);
      this.Controls.Add(this.labelNewVersion);
      this.Controls.Add(this.linkLabel);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewRelease";
      this.ShowIcon = false;
      this.Text = "Updates";
      this.Load += new System.EventHandler(this.NewRelease_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Label labelNewVersion;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonRemindeMe;
        private System.Windows.Forms.Button buttonIgnore;
        private System.Windows.Forms.Button buttonRelease;
        private System.Windows.Forms.Label labelLinkDecoration;
        private System.Windows.Forms.CheckBox checkUpdate;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label DownloadLabel;
        private System.Windows.Forms.Button CancelDownloadButton;
  }
}