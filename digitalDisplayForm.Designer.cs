namespace YoctoVisualisation
{
  partial class digitalDisplayForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(digitalDisplayForm));
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.rendererCanvas = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.rendererCanvas)).BeginInit();
      this.SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      // 
      // rendererCanvas
      // 
      this.rendererCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rendererCanvas.Location = new System.Drawing.Point(1, 0);
      this.rendererCanvas.Name = "rendererCanvas";
      this.rendererCanvas.Size = new System.Drawing.Size(414, 125);
      this.rendererCanvas.TabIndex = 1;
      this.rendererCanvas.TabStop = false;
      // 
      // digitalDisplayForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(414, 124);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.rendererCanvas);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "digitalDisplayForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "digitalDisplayForm";
      this.Activated += new System.EventHandler(this.switchConfiguration);
      this.Load += new System.EventHandler(this.digitalDisplayForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.rendererCanvas)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.PictureBox rendererCanvas;
  }
}