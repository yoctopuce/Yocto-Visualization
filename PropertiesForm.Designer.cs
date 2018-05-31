namespace YoctoVisualisation
{
    partial class PropertiesForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm));
      this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
      this.SuspendLayout();
      // 
      // propertyGrid1
      // 
      this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.propertyGrid1.BackColor = System.Drawing.SystemColors.Control;
    
      this.propertyGrid1.Location = new System.Drawing.Point(3, 1);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new System.Drawing.Size(474, 431);
      this.propertyGrid1.TabIndex = 0;
      this.propertyGrid1.TabStop = false;
      this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
      this.propertyGrid1.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
      this.propertyGrid1.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid1_SelectedObjectsChanged);
      this.propertyGrid1.DoubleClick += new System.EventHandler(this.propertyGrid1_DoubleClick);
      this.propertyGrid1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.propertyGrid1_MouseClick);
      this.propertyGrid1.Validating += new System.ComponentModel.CancelEventHandler(this.propertyGrid1_Validating);
      // 
      // PropertiesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(477, 431);
      this.Controls.Add(this.propertyGrid1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "PropertiesForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Properties";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertiesForm_FormClosing);
      this.Load += new System.EventHandler(this.PropertiesForm_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PropertiesForm_KeyDown);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}