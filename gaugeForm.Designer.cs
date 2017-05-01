namespace YoctoVisualisation
{
    partial class gaugeForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(gaugeForm));
      this.solidGauge1 = new LiveCharts.WinForms.SolidGauge();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.HintPanel = new System.Windows.Forms.Panel();
      this.HintLabel = new System.Windows.Forms.Label();
      this.HintPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // solidGauge1
      // 
      this.solidGauge1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solidGauge1.Location = new System.Drawing.Point(12, 12);
      this.solidGauge1.Margin = new System.Windows.Forms.Padding(0);
      this.solidGauge1.Name = "solidGauge1";
      this.solidGauge1.Size = new System.Drawing.Size(367, 245);
      this.solidGauge1.TabIndex = 0;
      this.solidGauge1.TabStop = false;
      this.solidGauge1.Text = "solidGauge1";
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      // 
      // HintPanel
      // 
      this.HintPanel.BackColor = System.Drawing.SystemColors.Info;
      this.HintPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.HintPanel.Controls.Add(this.HintLabel);
      this.HintPanel.Location = new System.Drawing.Point(41, 94);
      this.HintPanel.Name = "HintPanel";
      this.HintPanel.Size = new System.Drawing.Size(209, 76);
      this.HintPanel.TabIndex = 2;
      this.HintPanel.Visible = false;
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
      // gaugeForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(388, 266);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.HintPanel);
      this.Controls.Add(this.solidGauge1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "gaugeForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "New gauge";
      this.Activated += new System.EventHandler(this.switchConfiguration);
      this.Load += new System.EventHandler(this.gaugeForm_Load);
      this.HintPanel.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.SolidGauge solidGauge1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.Panel HintPanel;
    private System.Windows.Forms.Label HintLabel;
  }
}