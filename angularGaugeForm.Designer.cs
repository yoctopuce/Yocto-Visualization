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
      this.rendererCanvas = new System.Windows.Forms.PictureBox();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.rendererCanvas)).BeginInit();
      this.SuspendLayout();
      // 
      // rendererCanvas
      // 
      this.rendererCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rendererCanvas.Location = new System.Drawing.Point(0, 0);
      this.rendererCanvas.Name = "rendererCanvas";
      this.rendererCanvas.Size = new System.Drawing.Size(373, 292);
      this.rendererCanvas.TabIndex = 0;
      this.rendererCanvas.TabStop = false;
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening_1);
      // 
      // angularGaugeForm
      // 
      this.ClientSize = new System.Drawing.Size(372, 290);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.rendererCanvas);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "angularGaugeForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Activated += new System.EventHandler(this.switchConfiguration);
      this.Load += new System.EventHandler(this.angularGaugeForm_Load_1);
      this.LocationChanged += new System.EventHandler(this.angularGaugeForm_LocationChanged);
      ((System.ComponentModel.ISupportInitialize)(this.rendererCanvas)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
 
    
  
    private System.Windows.Forms.PictureBox rendererCanvas;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
  }
}