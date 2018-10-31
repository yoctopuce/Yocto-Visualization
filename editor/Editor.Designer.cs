namespace YoctoVisualisation
{
  partial class PropertiesForm2
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm2));
      this.label1 = new System.Windows.Forms.Label();
      this.outterpanel = new System.Windows.Forms.Panel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.outterpanel.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.BackColor = System.Drawing.SystemColors.Control;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(258, 64);
      this.label1.TabIndex = 0;
      this.label1.Text = "Please wait...";
      this.label1.Click += new System.EventHandler(this.label1_Click);
      // 
      // outterpanel
      // 
      this.outterpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.outterpanel.AutoScroll = true;
      this.outterpanel.BackColor = System.Drawing.SystemColors.Control;
      this.outterpanel.Controls.Add(this.panel1);
      this.outterpanel.ForeColor = System.Drawing.Color.DarkOrange;
      this.outterpanel.Location = new System.Drawing.Point(0, 0);
      this.outterpanel.Name = "outterpanel";
      this.outterpanel.Size = new System.Drawing.Size(272, 306);
      this.outterpanel.TabIndex = 3;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.SystemColors.Control;
      this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(265, 297);
      this.panel1.TabIndex = 2;
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel2.Controls.Add(this.label1);
      this.panel2.Location = new System.Drawing.Point(3, 309);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(266, 74);
      this.panel2.TabIndex = 4;
      // 
      // PropertiesForm2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(272, 387);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.outterpanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Location = new System.Drawing.Point(600, 0);
      this.Name = "PropertiesForm2";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertiesForm2_FormClosing);
      this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      this.outterpanel.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel outterpanel;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
  }
}

