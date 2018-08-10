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
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.openThisHubConfigurationPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.editThisHubConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.Menuitemremove = new System.Windows.Forms.ToolStripMenuItem();
      this.statusIcons = new System.Windows.Forms.ImageList(this.components);
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.deleteButton = new System.Windows.Forms.Button();
      this.editButton = new System.Windows.Forms.Button();
      this.AddBtn = new System.Windows.Forms.Button();
      this.hubWaiting = new System.Windows.Forms.PictureBox();
      this.hubOk = new System.Windows.Forms.PictureBox();
      this.hubFailed = new System.Windows.Forms.PictureBox();
      this.usbFailed = new System.Windows.Forms.PictureBox();
      this.usbOk = new System.Windows.Forms.PictureBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.listView1 = new System.Windows.Forms.ListView();
      this.Adresse = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.UseVirtualHub = new System.Windows.Forms.CheckBox();
      this.useUSB = new System.Windows.Forms.CheckBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.copyWarning = new System.Windows.Forms.Label();
      this.copyWarning2 = new System.Windows.Forms.Label();
      this.heightUnit = new System.Windows.Forms.Label();
      this.heightValue = new System.Windows.Forms.TextBox();
      this.heightLabel = new System.Windows.Forms.Label();
      this.widthUnit = new System.Windows.Forms.Label();
      this.widthValue = new System.Windows.Forms.TextBox();
      this.widthLabel = new System.Windows.Forms.Label();
      this.sizePolicy = new System.Windows.Forms.ComboBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.DpiTextBox = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.CaptureFolderbutton = new System.Windows.Forms.Button();
      this.targetFolder = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.ExportToPNG = new System.Windows.Forms.RadioButton();
      this.ExportToClipboard = new System.Windows.Forms.RadioButton();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.label17 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.maxPointsPerDatalogger = new System.Windows.Forms.TextBox();
      this.label16 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.memoryLabel = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.MaxDataPointsCount = new System.Windows.Forms.TextBox();
      this.MaxDataRecordsCount = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.MemoryTimer = new System.Windows.Forms.Timer(this.components);
      this.contextMenuStrip1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hubWaiting)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubOk)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubFailed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbFailed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbOk)).BeginInit();
      this.tabPage2.SuspendLayout();
      this.tabPage3.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openThisHubConfigurationPageToolStripMenuItem,
            this.editThisHubConnectionToolStripMenuItem,
            this.Menuitemremove});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(254, 92);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
      // 
      // openThisHubConfigurationPageToolStripMenuItem
      // 
      this.openThisHubConfigurationPageToolStripMenuItem.Name = "openThisHubConfigurationPageToolStripMenuItem";
      this.openThisHubConfigurationPageToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
      this.openThisHubConfigurationPageToolStripMenuItem.Text = "Open this hub configuration page";
      this.openThisHubConfigurationPageToolStripMenuItem.Click += new System.EventHandler(this.openThisHubConfigurationPageToolStripMenuItem_Click);
      // 
      // editThisHubConnectionToolStripMenuItem
      // 
      this.editThisHubConnectionToolStripMenuItem.Name = "editThisHubConnectionToolStripMenuItem";
      this.editThisHubConnectionToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
      this.editThisHubConnectionToolStripMenuItem.Text = "Edit This hub connection";
      this.editThisHubConnectionToolStripMenuItem.Click += new System.EventHandler(this.editThisHubConnectionToolStripMenuItem_Click);
      // 
      // Menuitemremove
      // 
      this.Menuitemremove.Name = "Menuitemremove";
      this.Menuitemremove.Size = new System.Drawing.Size(253, 22);
      this.Menuitemremove.Text = "Remove this hub connection";
      this.Menuitemremove.Click += new System.EventHandler(this.Menuitemremove_Click);
      // 
      // statusIcons
      // 
      this.statusIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("statusIcons.ImageStream")));
      this.statusIcons.TransparentColor = System.Drawing.Color.Transparent;
      this.statusIcons.Images.SetKeyName(0, "unknown.png");
      this.statusIcons.Images.SetKeyName(1, "failed.png");
      this.statusIcons.Images.SetKeyName(2, "ok.png");
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Location = new System.Drawing.Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(491, 339);
      this.tabControl1.TabIndex = 15;
      this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.deleteButton);
      this.tabPage1.Controls.Add(this.editButton);
      this.tabPage1.Controls.Add(this.AddBtn);
      this.tabPage1.Controls.Add(this.hubWaiting);
      this.tabPage1.Controls.Add(this.hubOk);
      this.tabPage1.Controls.Add(this.hubFailed);
      this.tabPage1.Controls.Add(this.usbFailed);
      this.tabPage1.Controls.Add(this.usbOk);
      this.tabPage1.Controls.Add(this.label2);
      this.tabPage1.Controls.Add(this.label1);
      this.tabPage1.Controls.Add(this.listView1);
      this.tabPage1.Controls.Add(this.UseVirtualHub);
      this.tabPage1.Controls.Add(this.useUSB);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(483, 313);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "USB / Network";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // deleteButton
      // 
      this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.deleteButton.Location = new System.Drawing.Point(382, 276);
      this.deleteButton.Name = "deleteButton";
      this.deleteButton.Size = new System.Drawing.Size(75, 20);
      this.deleteButton.TabIndex = 29;
      this.deleteButton.Text = "Delete";
      this.deleteButton.UseVisualStyleBackColor = true;
      this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
      // 
      // editButton
      // 
      this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.editButton.Location = new System.Drawing.Point(301, 276);
      this.editButton.Name = "editButton";
      this.editButton.Size = new System.Drawing.Size(75, 20);
      this.editButton.TabIndex = 28;
      this.editButton.Text = "Edit";
      this.editButton.UseVisualStyleBackColor = true;
      this.editButton.Click += new System.EventHandler(this.editButton_Click);
      // 
      // AddBtn
      // 
      this.AddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.AddBtn.Location = new System.Drawing.Point(220, 276);
      this.AddBtn.Name = "AddBtn";
      this.AddBtn.Size = new System.Drawing.Size(75, 20);
      this.AddBtn.TabIndex = 27;
      this.AddBtn.Text = "New";
      this.AddBtn.UseVisualStyleBackColor = true;
      this.AddBtn.Click += new System.EventHandler(this.Add_Click);
      // 
      // hubWaiting
      // 
      this.hubWaiting.Image = global::YoctoVisualization.Properties.Resources.unknown;
      this.hubWaiting.Location = new System.Drawing.Point(33, 60);
      this.hubWaiting.Name = "hubWaiting";
      this.hubWaiting.Size = new System.Drawing.Size(16, 16);
      this.hubWaiting.TabIndex = 26;
      this.hubWaiting.TabStop = false;
      this.hubWaiting.Visible = false;
      // 
      // hubOk
      // 
      this.hubOk.Image = global::YoctoVisualization.Properties.Resources.ok;
      this.hubOk.Location = new System.Drawing.Point(33, 59);
      this.hubOk.Name = "hubOk";
      this.hubOk.Size = new System.Drawing.Size(16, 16);
      this.hubOk.TabIndex = 25;
      this.hubOk.TabStop = false;
      this.hubOk.Visible = false;
      // 
      // hubFailed
      // 
      this.hubFailed.Image = global::YoctoVisualization.Properties.Resources.failed;
      this.hubFailed.Location = new System.Drawing.Point(33, 59);
      this.hubFailed.Name = "hubFailed";
      this.hubFailed.Size = new System.Drawing.Size(16, 16);
      this.hubFailed.TabIndex = 24;
      this.hubFailed.TabStop = false;
      this.hubFailed.Visible = false;
      // 
      // usbFailed
      // 
      this.usbFailed.Image = global::YoctoVisualization.Properties.Resources.failed;
      this.usbFailed.Location = new System.Drawing.Point(33, 37);
      this.usbFailed.Name = "usbFailed";
      this.usbFailed.Size = new System.Drawing.Size(16, 16);
      this.usbFailed.TabIndex = 23;
      this.usbFailed.TabStop = false;
      this.usbFailed.Visible = false;
      // 
      // usbOk
      // 
      this.usbOk.Image = global::YoctoVisualization.Properties.Resources.ok;
      this.usbOk.Location = new System.Drawing.Point(33, 37);
      this.usbOk.Name = "usbOk";
      this.usbOk.Size = new System.Drawing.Size(16, 16);
      this.usbOk.TabIndex = 22;
      this.usbOk.TabStop = false;
      this.usbOk.Visible = false;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.Location = new System.Drawing.Point(27, 93);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(430, 15);
      this.label2.TabIndex = 20;
      this.label2.Text = "You  can use devices from remote VirtualHub and YoctoHub as well. ";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(30, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(351, 13);
      this.label1.TabIndex = 19;
      this.label1.Text = "Choose if you want to use local  devices through USB  and/or VirtualHub";
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
      this.listView1.Location = new System.Drawing.Point(33, 111);
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(427, 157);
      this.listView1.SmallImageList = this.statusIcons;
      this.listView1.TabIndex = 18;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = System.Windows.Forms.View.Details;
      this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
      // 
      // Adresse
      // 
      this.Adresse.Text = "Address";
      this.Adresse.Width = 252;
      // 
      // Status
      // 
      this.Status.Text = "Status";
      this.Status.Width = 163;
      // 
      // UseVirtualHub
      // 
      this.UseVirtualHub.AutoSize = true;
      this.UseVirtualHub.Location = new System.Drawing.Point(62, 60);
      this.UseVirtualHub.Name = "UseVirtualHub";
      this.UseVirtualHub.Size = new System.Drawing.Size(183, 17);
      this.UseVirtualHub.TabIndex = 16;
      this.UseVirtualHub.Text = "Use Local  VirtualHub (127.0.0.1)";
      this.UseVirtualHub.UseVisualStyleBackColor = true;
      // 
      // useUSB
      // 
      this.useUSB.AutoSize = true;
      this.useUSB.Location = new System.Drawing.Point(62, 37);
      this.useUSB.Name = "useUSB";
      this.useUSB.Size = new System.Drawing.Size(138, 17);
      this.useUSB.TabIndex = 15;
      this.useUSB.Text = "Use local USB  devices";
      this.useUSB.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.copyWarning);
      this.tabPage2.Controls.Add(this.copyWarning2);
      this.tabPage2.Controls.Add(this.heightUnit);
      this.tabPage2.Controls.Add(this.heightValue);
      this.tabPage2.Controls.Add(this.heightLabel);
      this.tabPage2.Controls.Add(this.widthUnit);
      this.tabPage2.Controls.Add(this.widthValue);
      this.tabPage2.Controls.Add(this.widthLabel);
      this.tabPage2.Controls.Add(this.sizePolicy);
      this.tabPage2.Controls.Add(this.label9);
      this.tabPage2.Controls.Add(this.label8);
      this.tabPage2.Controls.Add(this.DpiTextBox);
      this.tabPage2.Controls.Add(this.label7);
      this.tabPage2.Controls.Add(this.CaptureFolderbutton);
      this.tabPage2.Controls.Add(this.targetFolder);
      this.tabPage2.Controls.Add(this.label6);
      this.tabPage2.Controls.Add(this.ExportToPNG);
      this.tabPage2.Controls.Add(this.ExportToClipboard);
      this.tabPage2.Controls.Add(this.label5);
      this.tabPage2.Controls.Add(this.label4);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(483, 313);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Screen capture";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // copyWarning
      // 
      this.copyWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.copyWarning.Location = new System.Drawing.Point(11, 272);
      this.copyWarning.Name = "copyWarning";
      this.copyWarning.Size = new System.Drawing.Size(456, 33);
      this.copyWarning.TabIndex = 19;
      this.copyWarning.Text = "(2) Due to technical limitations , images copied to clipboard can\'t keep their ba" +
    "ckground transparency, window background color will be used instead. \r\n\r\n";
      // 
      // copyWarning2
      // 
      this.copyWarning2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.copyWarning2.Location = new System.Drawing.Point(11, 252);
      this.copyWarning2.Name = "copyWarning2";
      this.copyWarning2.Size = new System.Drawing.Size(466, 20);
      this.copyWarning2.TabIndex = 18;
      this.copyWarning2.Text = "(1) PrintScreen key may not work if it is already captured by an other screen cap" +
    "ture utility, in that case,  use the right-clic contextual  menu.\r\n\r\n";
      this.copyWarning2.Click += new System.EventHandler(this.label10_Click_1);
      // 
      // heightUnit
      // 
      this.heightUnit.AutoSize = true;
      this.heightUnit.Location = new System.Drawing.Point(245, 220);
      this.heightUnit.Name = "heightUnit";
      this.heightUnit.Size = new System.Drawing.Size(18, 13);
      this.heightUnit.TabIndex = 17;
      this.heightUnit.Text = "px";
      // 
      // heightValue
      // 
      this.heightValue.Location = new System.Drawing.Point(164, 217);
      this.heightValue.Name = "heightValue";
      this.heightValue.Size = new System.Drawing.Size(75, 20);
      this.heightValue.TabIndex = 16;
      this.heightValue.Text = "1024";
      this.heightValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.heightValue.Leave += new System.EventHandler(this.heightValue_Leave);
      // 
      // heightLabel
      // 
      this.heightLabel.AutoSize = true;
      this.heightLabel.Location = new System.Drawing.Point(123, 220);
      this.heightLabel.Name = "heightLabel";
      this.heightLabel.Size = new System.Drawing.Size(39, 13);
      this.heightLabel.TabIndex = 15;
      this.heightLabel.Text = "height:";
      // 
      // widthUnit
      // 
      this.widthUnit.AutoSize = true;
      this.widthUnit.Location = new System.Drawing.Point(245, 194);
      this.widthUnit.Name = "widthUnit";
      this.widthUnit.Size = new System.Drawing.Size(18, 13);
      this.widthUnit.TabIndex = 14;
      this.widthUnit.Text = "px";
      // 
      // widthValue
      // 
      this.widthValue.Location = new System.Drawing.Point(164, 191);
      this.widthValue.Name = "widthValue";
      this.widthValue.Size = new System.Drawing.Size(75, 20);
      this.widthValue.TabIndex = 13;
      this.widthValue.Text = "1024";
      this.widthValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.widthValue.Leave += new System.EventHandler(this.widthValue_Leave);
      // 
      // widthLabel
      // 
      this.widthLabel.AutoSize = true;
      this.widthLabel.Location = new System.Drawing.Point(123, 194);
      this.widthLabel.Name = "widthLabel";
      this.widthLabel.Size = new System.Drawing.Size(35, 13);
      this.widthLabel.TabIndex = 12;
      this.widthLabel.Text = "width:";
      this.widthLabel.Click += new System.EventHandler(this.label10_Click);
      // 
      // sizePolicy
      // 
      this.sizePolicy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.sizePolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sizePolicy.FormattingEnabled = true;
      this.sizePolicy.Location = new System.Drawing.Point(102, 157);
      this.sizePolicy.Name = "sizePolicy";
      this.sizePolicy.Size = new System.Drawing.Size(354, 21);
      this.sizePolicy.TabIndex = 11;
      this.sizePolicy.SelectedIndexChanged += new System.EventHandler(this.sizePolicy_SelectedIndexChanged);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(22, 160);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(30, 13);
      this.label9.TabIndex = 10;
      this.label9.Text = "Size:";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(177, 125);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(21, 13);
      this.label8.TabIndex = 9;
      this.label8.Text = "dpi";
      // 
      // DpiTextBox
      // 
      this.DpiTextBox.Location = new System.Drawing.Point(102, 122);
      this.DpiTextBox.Name = "DpiTextBox";
      this.DpiTextBox.Size = new System.Drawing.Size(69, 20);
      this.DpiTextBox.TabIndex = 8;
      this.DpiTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.DpiTextBox.Leave += new System.EventHandler(this.DpiTextBox_Leave);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(22, 125);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(60, 13);
      this.label7.TabIndex = 7;
      this.label7.Text = "Resolution:";
      // 
      // CaptureFolderbutton
      // 
      this.CaptureFolderbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.CaptureFolderbutton.Location = new System.Drawing.Point(420, 88);
      this.CaptureFolderbutton.Name = "CaptureFolderbutton";
      this.CaptureFolderbutton.Size = new System.Drawing.Size(36, 23);
      this.CaptureFolderbutton.TabIndex = 6;
      this.CaptureFolderbutton.Text = "...";
      this.CaptureFolderbutton.UseVisualStyleBackColor = true;
      this.CaptureFolderbutton.Click += new System.EventHandler(this.CaptureFolderbutton_Click);
      // 
      // targetFolder
      // 
      this.targetFolder.Location = new System.Drawing.Point(102, 90);
      this.targetFolder.Name = "targetFolder";
      this.targetFolder.Size = new System.Drawing.Size(312, 20);
      this.targetFolder.TabIndex = 5;
      this.targetFolder.Leave += new System.EventHandler(this.targetFolder_Leave);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(22, 93);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(39, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "Folder:";
      // 
      // ExportToPNG
      // 
      this.ExportToPNG.AutoSize = true;
      this.ExportToPNG.Location = new System.Drawing.Point(102, 60);
      this.ExportToPNG.Name = "ExportToPNG";
      this.ExportToPNG.Size = new System.Drawing.Size(64, 17);
      this.ExportToPNG.TabIndex = 3;
      this.ExportToPNG.Text = "PNG file";
      this.ExportToPNG.UseVisualStyleBackColor = true;
      // 
      // ExportToClipboard
      // 
      this.ExportToClipboard.AutoSize = true;
      this.ExportToClipboard.Checked = true;
      this.ExportToClipboard.Location = new System.Drawing.Point(102, 37);
      this.ExportToClipboard.Name = "ExportToClipboard";
      this.ExportToClipboard.Size = new System.Drawing.Size(84, 17);
      this.ExportToClipboard.TabIndex = 2;
      this.ExportToClipboard.TabStop = true;
      this.ExportToClipboard.Text = "Clipboard (2)";
      this.ExportToClipboard.UseVisualStyleBackColor = true;
      this.ExportToClipboard.CheckedChanged += new System.EventHandler(this.ExportToClipboard_CheckedChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(22, 39);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(59, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = "Capture to:";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.Location = new System.Drawing.Point(22, 14);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(445, 20);
      this.label4.TabIndex = 0;
      this.label4.Text = "At any time, widget contents can be captured to an image by a PrintScreen key pre" +
    "ss (1)";
      // 
      // tabPage3
      // 
      this.tabPage3.Controls.Add(this.label17);
      this.tabPage3.Controls.Add(this.label3);
      this.tabPage3.Controls.Add(this.maxPointsPerDatalogger);
      this.tabPage3.Controls.Add(this.label16);
      this.tabPage3.Controls.Add(this.label15);
      this.tabPage3.Controls.Add(this.memoryLabel);
      this.tabPage3.Controls.Add(this.label14);
      this.tabPage3.Controls.Add(this.label13);
      this.tabPage3.Controls.Add(this.MaxDataPointsCount);
      this.tabPage3.Controls.Add(this.MaxDataRecordsCount);
      this.tabPage3.Controls.Add(this.label12);
      this.tabPage3.Controls.Add(this.label11);
      this.tabPage3.Controls.Add(this.label10);
      this.tabPage3.Location = new System.Drawing.Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Size = new System.Drawing.Size(483, 313);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "Ressources";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // label17
      // 
      this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label17.Location = new System.Drawing.Point(16, 138);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(446, 45);
      this.label17.TabIndex = 33;
      this.label17.Text = resources.GetString("label17.Text");
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(325, 193);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(143, 41);
      this.label3.TabIndex = 32;
      this.label3.Text = " (0=unlimited,\r\n  -1 = no datalogger access)";
      this.label3.Click += new System.EventHandler(this.label3_Click);
      // 
      // maxPointsPerDatalogger
      // 
      this.maxPointsPerDatalogger.Location = new System.Drawing.Point(245, 192);
      this.maxPointsPerDatalogger.Name = "maxPointsPerDatalogger";
      this.maxPointsPerDatalogger.Size = new System.Drawing.Size(70, 20);
      this.maxPointsPerDatalogger.TabIndex = 31;
      this.maxPointsPerDatalogger.Text = "0";
      this.maxPointsPerDatalogger.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.maxPointsPerDatalogger.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
      this.maxPointsPerDatalogger.Leave += new System.EventHandler(this.maxPointsPerDatalogger_Leave);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(16, 193);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(228, 13);
      this.label16.TabIndex = 30;
      this.label16.Text = "Max data points/series  loaded from datalogger";
      this.label16.Click += new System.EventHandler(this.label16_Click);
      // 
      // label15
      // 
      this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label15.Location = new System.Drawing.Point(16, 243);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(446, 22);
      this.label15.TabIndex = 29;
      this.label15.Text = "After a  limit increase, you\'ll have to restart the application to get your old d" +
    "ata back.";
      // 
      // memoryLabel
      // 
      this.memoryLabel.AutoSize = true;
      this.memoryLabel.Location = new System.Drawing.Point(16, 221);
      this.memoryLabel.Name = "memoryLabel";
      this.memoryLabel.Size = new System.Drawing.Size(93, 13);
      this.memoryLabel.TabIndex = 28;
      this.memoryLabel.Text = "Available Memory:";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(325, 99);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(69, 13);
      this.label14.TabIndex = 27;
      this.label14.Text = " (0=unlimited)";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(325, 73);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(69, 13);
      this.label13.TabIndex = 26;
      this.label13.Text = " (0=unlimited)";
      // 
      // MaxDataPointsCount
      // 
      this.MaxDataPointsCount.Location = new System.Drawing.Point(245, 96);
      this.MaxDataPointsCount.Name = "MaxDataPointsCount";
      this.MaxDataPointsCount.Size = new System.Drawing.Size(70, 20);
      this.MaxDataPointsCount.TabIndex = 25;
      this.MaxDataPointsCount.Text = "0";
      this.MaxDataPointsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.MaxDataPointsCount.TextChanged += new System.EventHandler(this.MaxDataPointsCount_TextChanged);
      this.MaxDataPointsCount.Leave += new System.EventHandler(this.MaxDataPointsCount_Leave);
      // 
      // MaxDataRecordsCount
      // 
      this.MaxDataRecordsCount.Location = new System.Drawing.Point(245, 70);
      this.MaxDataRecordsCount.Name = "MaxDataRecordsCount";
      this.MaxDataRecordsCount.Size = new System.Drawing.Size(72, 20);
      this.MaxDataRecordsCount.TabIndex = 24;
      this.MaxDataRecordsCount.Text = "0";
      this.MaxDataRecordsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.MaxDataRecordsCount.Leave += new System.EventHandler(this.MaxDataRecordsCount_Leave);
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(16, 99);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(184, 13);
      this.label12.TabIndex = 23;
      this.label12.Text = "Max count of graph series data points";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(16, 73);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(170, 13);
      this.label11.TabIndex = 22;
      this.label11.Text = "Max count of sensors data records";
      // 
      // label10
      // 
      this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label10.Location = new System.Drawing.Point(16, 10);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(446, 44);
      this.label10.TabIndex = 21;
      this.label10.Text = resources.GetString("label10.Text");
      // 
      // MemoryTimer
      // 
      this.MemoryTimer.Interval = 1000;
      this.MemoryTimer.Tick += new System.EventHandler(this.MemoryTimer_Tick);
      // 
      // ConfigForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(514, 363);
      this.Controls.Add(this.tabControl1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(470, 320);
      this.Name = "ConfigForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Configuration";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
      this.contextMenuStrip1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hubWaiting)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubOk)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.hubFailed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbFailed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.usbOk)).EndInit();
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.tabPage3.ResumeLayout(false);
      this.tabPage3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem Menuitemremove;
    private System.Windows.Forms.ImageList statusIcons;
    private System.Windows.Forms.ToolStripMenuItem openThisHubConfigurationPageToolStripMenuItem;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Button AddBtn;
    private System.Windows.Forms.PictureBox hubWaiting;
    private System.Windows.Forms.PictureBox hubOk;
    private System.Windows.Forms.PictureBox hubFailed;
    private System.Windows.Forms.PictureBox usbFailed;
    private System.Windows.Forms.PictureBox usbOk;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListView listView1;
    private System.Windows.Forms.ColumnHeader Adresse;
    private System.Windows.Forms.ColumnHeader Status;
    private System.Windows.Forms.CheckBox UseVirtualHub;
    private System.Windows.Forms.CheckBox useUSB;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.Label widthLabel;
    private System.Windows.Forms.ComboBox sizePolicy;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox DpiTextBox;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button CaptureFolderbutton;
    private System.Windows.Forms.TextBox targetFolder;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.RadioButton ExportToPNG;
    private System.Windows.Forms.RadioButton ExportToClipboard;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label widthUnit;
    private System.Windows.Forms.TextBox widthValue;
    private System.Windows.Forms.Label copyWarning2;
    private System.Windows.Forms.Label heightUnit;
    private System.Windows.Forms.TextBox heightValue;
    private System.Windows.Forms.Label heightLabel;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.Label copyWarning;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TextBox MaxDataPointsCount;
    private System.Windows.Forms.TextBox MaxDataRecordsCount;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label memoryLabel;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Timer MemoryTimer;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Button deleteButton;
    private System.Windows.Forms.Button editButton;
    private System.Windows.Forms.ToolStripMenuItem editThisHubConnectionToolStripMenuItem;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox maxPointsPerDatalogger;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label17;
  }
}