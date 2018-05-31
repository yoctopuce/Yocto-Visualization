namespace YoctoVisualisation
{
    partial class StartForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
      this.label1 = new System.Windows.Forms.Label();
      this.YoctoTimer = new System.Windows.Forms.Timer(this.components);
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.button6 = new System.Windows.Forms.Button();
      this.button5 = new System.Windows.Forms.Button();
      this.button4 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(646, 56);
      this.label1.TabIndex = 3;
      this.label1.Text = "Hello, welcome to Yocto-Visualization  application, what would you like to do ?";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // YoctoTimer
      // 
      this.YoctoTimer.Interval = 10;
      this.YoctoTimer.Tick += new System.EventHandler(this.YoctoTimer_Tick);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLogsToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(129, 26);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
      // 
      // showLogsToolStripMenuItem
      // 
      this.showLogsToolStripMenuItem.Name = "showLogsToolStripMenuItem";
      this.showLogsToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
      this.showLogsToolStripMenuItem.Text = "Show logs";
      this.showLogsToolStripMenuItem.Click += new System.EventHandler(this.showLogsToolStripMenuItem_Click);
      // 
      // button6
      // 
      this.button6.Image = global::YoctoVisualization.Properties.Resources.rawdata;
      this.button6.Location = new System.Drawing.Point(56, 268);
      this.button6.Name = "button6";
      this.button6.Size = new System.Drawing.Size(180, 180);
      this.button6.TabIndex = 6;
      this.button6.Text = "Get raw data";
      this.button6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button6.UseVisualStyleBackColor = true;
      this.button6.Click += new System.EventHandler(this.button6_Click);
      // 
      // button5
      // 
      this.button5.Image = global::YoctoVisualization.Properties.Resources.digital;
      this.button5.Location = new System.Drawing.Point(242, 268);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(180, 180);
      this.button5.TabIndex = 5;
      this.button5.Text = "Create a new digital display";
      this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new System.EventHandler(this.button5_Click);
      // 
      // button4
      // 
      this.button4.Image = global::YoctoVisualization.Properties.Resources.new_angular;
      this.button4.Location = new System.Drawing.Point(428, 268);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(180, 180);
      this.button4.TabIndex = 4;
      this.button4.Text = "Create a new angular gauge";
      this.button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // button3
      // 
      this.button3.Image = global::YoctoVisualization.Properties.Resources.new_solidgauge1;
      this.button3.Location = new System.Drawing.Point(242, 82);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(180, 180);
      this.button3.TabIndex = 2;
      this.button3.Text = "Create a new solid gauge";
      this.button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // button2
      // 
      this.button2.Image = global::YoctoVisualization.Properties.Resources.new_graph;
      this.button2.Location = new System.Drawing.Point(428, 82);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(180, 180);
      this.button2.TabIndex = 1;
      this.button2.Text = "Create a new graph";
      this.button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // button1
      // 
      this.button1.Image = global::YoctoVisualization.Properties.Resources.configure1;
      this.button1.Location = new System.Drawing.Point(56, 82);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(180, 180);
      this.button1.TabIndex = 0;
      this.button1.Text = "Configure USB and network";
      this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // StartForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(668, 473);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.button6);
      this.Controls.Add(this.button5);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "StartForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Yocto-Visualization V2";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.startform_FormClosing);
      this.Load += new System.EventHandler(this.startform_Load);
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Timer YoctoTimer;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button6;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem showLogsToolStripMenuItem;
  }
}

