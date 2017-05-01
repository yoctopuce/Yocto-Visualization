namespace YoctoVisualisation
{
  partial class angularGaugeForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(angularGaugeForm));
      this.angularGauge1 = new LiveCharts.WinForms.AngularGauge();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.HintLabel = new System.Windows.Forms.Label();
      this.HintPanel = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.HintPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // angularGauge1
      // 
      this.angularGauge1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.angularGauge1.Enabled = false;
      this.angularGauge1.Location = new System.Drawing.Point(12, 12);
      this.angularGauge1.Name = "angularGauge1";
      this.angularGauge1.Size = new System.Drawing.Size(351, 330);
      this.angularGauge1.TabIndex = 0;
      this.angularGauge1.Text = "angularGauge1";
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
      // 
      // HintLabel
      // 
      this.HintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.HintLabel.Location = new System.Drawing.Point(2, 11);
      this.HintLabel.Name = "HintLabel";
      this.HintLabel.Size = new System.Drawing.Size(202, 54);
      this.HintLabel.TabIndex = 2;
      this.HintLabel.Text = "No Data source configured, do a right click and choose \"configure this digital di" +
    "splay\"";
      this.HintLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // HintPanel
      // 
      this.HintPanel.BackColor = System.Drawing.SystemColors.Info;
      this.HintPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.HintPanel.Controls.Add(this.HintLabel);
      this.HintPanel.Location = new System.Drawing.Point(38, 93);
      this.HintPanel.Name = "HintPanel";
      this.HintPanel.Size = new System.Drawing.Size(209, 76);
      this.HintPanel.TabIndex = 3;
      this.HintPanel.Visible = false;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(263, 322);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 23);
      this.label1.TabIndex = 4;
      this.label1.Text = "OFFLINE";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.label1.Visible = false;
      // 
      // angularGaugeForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(375, 354);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.HintPanel);
      this.Controls.Add(this.angularGauge1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "angularGaugeForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "angularGauge";
      this.Activated += new System.EventHandler(this.switchConfiguration);
      this.Load += new System.EventHandler(this.angularGaugeForm_Load);
      this.HintPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private LiveCharts.WinForms.AngularGauge angularGauge1;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.Label HintLabel;
    private System.Windows.Forms.Panel HintPanel;
    private System.Windows.Forms.Label label1;
  }
}