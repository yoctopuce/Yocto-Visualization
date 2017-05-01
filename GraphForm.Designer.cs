using System.Windows.Media;

namespace YoctoVisualisation
{
    partial class GraphForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
      this.cartesianChart2 = new LiveCharts.WinForms.CartesianChart();
      this.HintLabel = new System.Windows.Forms.Label();
      this.HintPanel = new System.Windows.Forms.Panel();
      this.HintPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      // 
      // cartesianChart1
      // 
      this.cartesianChart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cartesianChart1.Hoverable = true;
      this.cartesianChart1.Location = new System.Drawing.Point(12, 12);
      this.cartesianChart1.Name = "cartesianChart1";
      this.cartesianChart1.ScrollHorizontalFrom = 0D;
      this.cartesianChart1.ScrollHorizontalTo = 0D;
      this.cartesianChart1.ScrollMode = LiveCharts.ScrollMode.None;
      this.cartesianChart1.ScrollVerticalFrom = 0D;
      this.cartesianChart1.ScrollVerticalTo = 0D;
      this.cartesianChart1.Size = new System.Drawing.Size(677, 351);
      this.cartesianChart1.TabIndex = 1;
      this.cartesianChart1.Text = "cartesianChart1";
      // 
      // cartesianChart2
      // 
      this.cartesianChart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cartesianChart2.Hoverable = true;
      this.cartesianChart2.Location = new System.Drawing.Point(12, 355);
      this.cartesianChart2.Name = "cartesianChart2";
      this.cartesianChart2.ScrollBarFill = null;
      this.cartesianChart2.ScrollHorizontalFrom = 0D;
      this.cartesianChart2.ScrollHorizontalTo = 0D;
      this.cartesianChart2.ScrollMode = LiveCharts.ScrollMode.None;
      this.cartesianChart2.ScrollVerticalFrom = 0D;
      this.cartesianChart2.ScrollVerticalTo = 0D;
      this.cartesianChart2.Size = new System.Drawing.Size(677, 150);
      this.cartesianChart2.TabIndex = 2;
      this.cartesianChart2.Text = "scrollerChart";
      this.cartesianChart2.DataClick += new LiveCharts.Events.DataClickHandler(this.cartesianChart2_DataClick);
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
      this.HintPanel.Location = new System.Drawing.Point(246, 220);
      this.HintPanel.Name = "HintPanel";
      this.HintPanel.Size = new System.Drawing.Size(209, 76);
      this.HintPanel.TabIndex = 3;
      this.HintPanel.Visible = false;
      // 
      // GraphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(701, 517);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.HintPanel);
      this.Controls.Add(this.cartesianChart2);
      this.Controls.Add(this.cartesianChart1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "GraphForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "New Graph";
      this.Activated += new System.EventHandler(this.switchConfiguration);
      this.Load += new System.EventHandler(this.GraphForm_Load);
      this.HintPanel.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
    private LiveCharts.WinForms.CartesianChart cartesianChart2;
    private System.Windows.Forms.Label HintLabel;
    private System.Windows.Forms.Panel HintPanel;
  }
}