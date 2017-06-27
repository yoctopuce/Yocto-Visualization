namespace YoctoVisualisation
{
  partial class ConfigForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
      this.useUSB = new System.Windows.Forms.CheckBox();
      this.UseVirtualHub = new System.Windows.Forms.CheckBox();
      this.newEntry = new System.Windows.Forms.TextBox();
      this.AddBtn = new System.Windows.Forms.Button();
      this.listView1 = new System.Windows.Forms.ListView();
      this.Adresse = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.Menuitemremove = new System.Windows.Forms.ToolStripMenuItem();
      this.openThisHubConfigurationPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusIcons = new System.Windows.Forms.ImageList(this.components);
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.hubWaiting = new System.Windows.Forms.PictureBox();
      this.hubOk = new System.Windows.Forms.PictureBox();
      this.hubFailed = new System.Windows.Forms.PictureBox();
      this.usbFailed = new System.Windows.Forms.PictureBox();
      this.usbOk = new System.Windows.Forms.PictureBox();
      this.contextMenuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hubWaiting)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubOk)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubFailed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbFailed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbOk)).BeginInit();
      this.SuspendLayout();
      // 
      // useUSB
      // 
      this.useUSB.AutoSize = true;
      this.useUSB.Location = new System.Drawing.Point(44, 33);
      this.useUSB.Name = "useUSB";
      this.useUSB.Size = new System.Drawing.Size(138, 17);
      this.useUSB.TabIndex = 0;
      this.useUSB.Text = "Use local USB  devices";
      this.useUSB.UseVisualStyleBackColor = true;
      // 
      // UseVirtualHub
      // 
      this.UseVirtualHub.AutoSize = true;
      this.UseVirtualHub.Location = new System.Drawing.Point(44, 56);
      this.UseVirtualHub.Name = "UseVirtualHub";
      this.UseVirtualHub.Size = new System.Drawing.Size(183, 17);
      this.UseVirtualHub.TabIndex = 1;
      this.UseVirtualHub.Text = "Use Local  VirtualHub (127.0.0.1)";
      this.UseVirtualHub.UseVisualStyleBackColor = true;
      // 
      // newEntry
      // 
      this.newEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.newEntry.Location = new System.Drawing.Point(162, 293);
      this.newEntry.Name = "newEntry";
      this.newEntry.Size = new System.Drawing.Size(245, 20);
      this.newEntry.TabIndex = 3;
      this.newEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newEntry_KeyDown);
      // 
      // AddBtn
      // 
      this.AddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.AddBtn.Location = new System.Drawing.Point(413, 293);
      this.AddBtn.Name = "AddBtn";
      this.AddBtn.Size = new System.Drawing.Size(75, 23);
      this.AddBtn.TabIndex = 4;
      this.AddBtn.Text = "Add";
      this.AddBtn.UseVisualStyleBackColor = true;
      this.AddBtn.Click += new System.EventHandler(this.Add_Click);
      // 
      // listView1
      // 
      this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Adresse,
            this.Status});
      this.listView1.ContextMenuStrip = this.contextMenuStrip1;
      this.listView1.FullRowSelect = true;
      this.listView1.GridLines = true;
      this.listView1.LargeImageList = this.statusIcons;
      this.listView1.Location = new System.Drawing.Point(12, 129);
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(476, 158);
      this.listView1.SmallImageList = this.statusIcons;
      this.listView1.TabIndex = 5;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = System.Windows.Forms.View.Details;
      // 
      // Adresse
      // 
      this.Adresse.Text = "Address";
      this.Adresse.Width = 200;
      // 
      // Status
      // 
      this.Status.Text = "Status";
      this.Status.Width = 200;
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menuitemremove,
            this.openThisHubConfigurationPageToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(254, 48);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
      // 
      // Menuitemremove
      // 
      this.Menuitemremove.Name = "Menuitemremove";
      this.Menuitemremove.Size = new System.Drawing.Size(253, 22);
      this.Menuitemremove.Text = "Remove this hub";
      this.Menuitemremove.Click += new System.EventHandler(this.Menuitemremove_Click);
      // 
      // openThisHubConfigurationPageToolStripMenuItem
      // 
      this.openThisHubConfigurationPageToolStripMenuItem.Name = "openThisHubConfigurationPageToolStripMenuItem";
      this.openThisHubConfigurationPageToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
      this.openThisHubConfigurationPageToolStripMenuItem.Text = "Open this hub configuration page";
      this.openThisHubConfigurationPageToolStripMenuItem.Click += new System.EventHandler(this.openThisHubConfigurationPageToolStripMenuItem_Click);
      // 
      // statusIcons
      // 
      this.statusIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("statusIcons.ImageStream")));
      this.statusIcons.TransparentColor = System.Drawing.Color.Transparent;
      this.statusIcons.Images.SetKeyName(0, "unknown.png");
      this.statusIcons.Images.SetKeyName(1, "failed.png");
      this.statusIcons.Images.SetKeyName(2, "ok.png");
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(351, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Choose if you want to use local  devices through USB  and/or VirtualHub";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.Location = new System.Drawing.Point(9, 85);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(479, 29);
      this.label2.TabIndex = 8;
      this.label2.Text = "You  can use devices from remote VirtualHub and YoctoHub as well. Just enter the " +
    "address and click \"Add\". Do a right click to remove hubs from the list.";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 296);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(144, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "IP address or Network name:";
      // 
      // hubWaiting
      // 
      this.hubWaiting.Image = global::YoctoVisualisation.Properties.Resources.unknown;
      this.hubWaiting.Location = new System.Drawing.Point(15, 56);
      this.hubWaiting.Name = "hubWaiting";
      this.hubWaiting.Size = new System.Drawing.Size(16, 16);
      this.hubWaiting.TabIndex = 14;
      this.hubWaiting.TabStop = false;
      this.hubWaiting.Visible = false;
      // 
      // hubOk
      // 
      this.hubOk.Image = global::YoctoVisualisation.Properties.Resources.ok;
      this.hubOk.Location = new System.Drawing.Point(15, 55);
      this.hubOk.Name = "hubOk";
      this.hubOk.Size = new System.Drawing.Size(16, 16);
      this.hubOk.TabIndex = 13;
      this.hubOk.TabStop = false;
      this.hubOk.Visible = false;
      // 
      // hubFailed
      // 
      this.hubFailed.Image = global::YoctoVisualisation.Properties.Resources.failed;
      this.hubFailed.Location = new System.Drawing.Point(15, 55);
      this.hubFailed.Name = "hubFailed";
      this.hubFailed.Size = new System.Drawing.Size(16, 16);
      this.hubFailed.TabIndex = 12;
      this.hubFailed.TabStop = false;
      this.hubFailed.Visible = false;
      // 
      // usbFailed
      // 
      this.usbFailed.Image = global::YoctoVisualisation.Properties.Resources.failed;
      this.usbFailed.Location = new System.Drawing.Point(15, 33);
      this.usbFailed.Name = "usbFailed";
      this.usbFailed.Size = new System.Drawing.Size(16, 16);
      this.usbFailed.TabIndex = 11;
      this.usbFailed.TabStop = false;
      this.usbFailed.Visible = false;
      // 
      // usbOk
      // 
      this.usbOk.Image = global::YoctoVisualisation.Properties.Resources.ok;
      this.usbOk.Location = new System.Drawing.Point(15, 33);
      this.usbOk.Name = "usbOk";
      this.usbOk.Size = new System.Drawing.Size(16, 16);
      this.usbOk.TabIndex = 10;
      this.usbOk.TabStop = false;
      this.usbOk.Visible = false;
      this.usbOk.Click += new System.EventHandler(this.pictureBox1_Click);
      // 
      // ConfigForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(500, 325);
      this.Controls.Add(this.hubWaiting);
      this.Controls.Add(this.hubOk);
      this.Controls.Add(this.hubFailed);
      this.Controls.Add(this.usbFailed);
      this.Controls.Add(this.usbOk);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.listView1);
      this.Controls.Add(this.AddBtn);
      this.Controls.Add(this.newEntry);
      this.Controls.Add(this.UseVirtualHub);
      this.Controls.Add(this.useUSB);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(470, 320);
      this.Name = "ConfigForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Configuration";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
      this.contextMenuStrip1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.hubWaiting)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubOk)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubFailed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbFailed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbOk)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox useUSB;
    private System.Windows.Forms.CheckBox UseVirtualHub;
    private System.Windows.Forms.TextBox newEntry;
    private System.Windows.Forms.Button AddBtn;
    private System.Windows.Forms.ListView listView1;
    private System.Windows.Forms.ColumnHeader Adresse;
    private System.Windows.Forms.ColumnHeader Status;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem Menuitemremove;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.PictureBox usbOk;
    private System.Windows.Forms.PictureBox usbFailed;
    private System.Windows.Forms.PictureBox hubFailed;
    private System.Windows.Forms.PictureBox hubOk;
    private System.Windows.Forms.PictureBox hubWaiting;
    private System.Windows.Forms.ImageList statusIcons;
    private System.Windows.Forms.ToolStripMenuItem openThisHubConfigurationPageToolStripMenuItem;
  }
}