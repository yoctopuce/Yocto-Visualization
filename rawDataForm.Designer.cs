namespace YoctoVisualisation
{
  partial class RawDataForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RawDataForm));
      this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
      this.showMin = new System.Windows.Forms.CheckBox();
      this.showMax = new System.Windows.Forms.CheckBox();
      this.showAvg = new System.Windows.Forms.CheckBox();
      this.CsvBtn = new System.Windows.Forms.Button();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.button2 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.progressPanel = new System.Windows.Forms.Panel();
      this.label2 = new System.Windows.Forms.Label();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.progressPanel.SuspendLayout();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // checkedListBox1
      // 
      this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.checkedListBox1.ContextMenuStrip = this.contextMenuStrip1;
      this.checkedListBox1.FormattingEnabled = true;
      this.checkedListBox1.Location = new System.Drawing.Point(12, 13);
      this.checkedListBox1.Name = "checkedListBox1";
      this.checkedListBox1.Size = new System.Drawing.Size(378, 94);
      this.checkedListBox1.TabIndex = 0;
      this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
      // 
      // showMin
      // 
      this.showMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showMin.AutoSize = true;
      this.showMin.Location = new System.Drawing.Point(408, 13);
      this.showMin.Name = "showMin";
      this.showMin.Size = new System.Drawing.Size(131, 17);
      this.showMin.TabIndex = 1;
      this.showMin.Text = "Show Minimum values";
      this.showMin.UseVisualStyleBackColor = true;
      this.showMin.CheckedChanged += new System.EventHandler(this.CheckedChanged);
      // 
      // showMax
      // 
      this.showMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showMax.AutoSize = true;
      this.showMax.Location = new System.Drawing.Point(410, 59);
      this.showMax.Name = "showMax";
      this.showMax.Size = new System.Drawing.Size(134, 17);
      this.showMax.TabIndex = 2;
      this.showMax.Text = "Show Maximum values";
      this.showMax.UseVisualStyleBackColor = true;
      this.showMax.CheckedChanged += new System.EventHandler(this.CheckedChanged);
      // 
      // showAvg
      // 
      this.showAvg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showAvg.AutoSize = true;
      this.showAvg.Checked = true;
      this.showAvg.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showAvg.Location = new System.Drawing.Point(410, 36);
      this.showAvg.Name = "showAvg";
      this.showAvg.Size = new System.Drawing.Size(125, 17);
      this.showAvg.TabIndex = 3;
      this.showAvg.Text = "Show Average value";
      this.showAvg.UseVisualStyleBackColor = true;
      this.showAvg.CheckedChanged += new System.EventHandler(this.CheckedChanged);
      // 
      // CsvBtn
      // 
      this.CsvBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.CsvBtn.Location = new System.Drawing.Point(478, 82);
      this.CsvBtn.Name = "CsvBtn";
      this.CsvBtn.Size = new System.Drawing.Size(94, 22);
      this.CsvBtn.TabIndex = 4;
      this.CsvBtn.Text = "Export to CSV...";
      this.CsvBtn.UseVisualStyleBackColor = true;
      this.CsvBtn.Click += new System.EventHandler(this.button1_Click);
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Location = new System.Drawing.Point(12, 145);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.ShowEditingIcon = false;
      this.dataGridView1.Size = new System.Drawing.Size(560, 255);
      this.dataGridView1.TabIndex = 5;
      this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.button2.Location = new System.Drawing.Point(408, 82);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(64, 22);
      this.button2.TabIndex = 6;
      this.button2.Text = "Refresh";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click_1);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 120);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(398, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Here are the XXX last data row.  Use the export feature to  get the whole data s" +
    "et.";
      // 
      // saveFileDialog1
      // 
      this.saveFileDialog1.DefaultExt = "csv";
      this.saveFileDialog1.Filter = "Comma-separated values (.csv)|*.csv";
      this.saveFileDialog1.Title = "Export data to CSV file";
      // 
      // progressPanel
      // 
      this.progressPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.progressPanel.Controls.Add(this.label2);
      this.progressPanel.Controls.Add(this.progressBar1);
      this.progressPanel.Location = new System.Drawing.Point(103, 228);
      this.progressPanel.Name = "progressPanel";
      this.progressPanel.Size = new System.Drawing.Size(385, 53);
      this.progressPanel.TabIndex = 8;
      this.progressPanel.Visible = false;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(144, 33);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(116, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Exporting, please wait..";
      // 
      // progressBar1
      // 
      this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar1.Location = new System.Drawing.Point(21, 6);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(347, 23);
      this.progressBar1.TabIndex = 0;
      this.progressBar1.Visible = false;
      // 
      // timer1
      // 
      this.timer1.Interval = 1000;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
      // 
      // selectAllToolStripMenuItem
      // 
      this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
      this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.selectAllToolStripMenuItem.Text = "Select All";
      this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
      // 
      // selectNoneToolStripMenuItem
      // 
      this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
      this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.selectNoneToolStripMenuItem.Text = "Select None";
      this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
      // 
      // RawDataForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 412);
      this.Controls.Add(this.progressPanel);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.CsvBtn);
      this.Controls.Add(this.showAvg);
      this.Controls.Add(this.showMax);
      this.Controls.Add(this.showMin);
      this.Controls.Add(this.checkedListBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(600, 450);
      this.Name = "RawDataForm";
      this.Text = "Raw data";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RawDataForm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.progressPanel.ResumeLayout(false);
      this.progressPanel.PerformLayout();
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckedListBox checkedListBox1;
    private System.Windows.Forms.CheckBox showMin;
    private System.Windows.Forms.CheckBox showMax;
    private System.Windows.Forms.CheckBox showAvg;
    private System.Windows.Forms.Button CsvBtn;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.Panel progressPanel;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
  }
}