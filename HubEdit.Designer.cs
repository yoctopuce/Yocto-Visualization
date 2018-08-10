namespace YoctoVisualization
{
  partial class HubEdit
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HubEdit));
      this.protocolChooser = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.addressChooser = new System.Windows.Forms.TextBox();
      this.portChooser = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.pathChooser = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.usernameChooser = new System.Windows.Forms.TextBox();
      this.passwordChooser = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // protocolChooser
      // 
      this.protocolChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.protocolChooser.FormattingEnabled = true;
      this.protocolChooser.Items.AddRange(new object[] {
            "ws",
            "http"});
      this.protocolChooser.Location = new System.Drawing.Point(77, 55);
      this.protocolChooser.Name = "protocolChooser";
      this.protocolChooser.Size = new System.Drawing.Size(54, 21);
      this.protocolChooser.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 59);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(49, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Protocol:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(137, 59);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Address:";
      // 
      // addressChooser
      // 
      this.addressChooser.Location = new System.Drawing.Point(191, 56);
      this.addressChooser.Name = "addressChooser";
      this.addressChooser.Size = new System.Drawing.Size(104, 20);
      this.addressChooser.TabIndex = 3;
      // 
      // portChooser
      // 
      this.portChooser.Location = new System.Drawing.Point(329, 57);
      this.portChooser.Name = "portChooser";
      this.portChooser.Size = new System.Drawing.Size(37, 20);
      this.portChooser.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(302, 60);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(29, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Port:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(13, 89);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(32, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Path:";
      // 
      // pathChooser
      // 
      this.pathChooser.Location = new System.Drawing.Point(77, 86);
      this.pathChooser.Name = "pathChooser";
      this.pathChooser.Size = new System.Drawing.Size(289, 20);
      this.pathChooser.TabIndex = 7;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(13, 117);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(58, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Username:";
      // 
      // usernameChooser
      // 
      this.usernameChooser.Location = new System.Drawing.Point(77, 114);
      this.usernameChooser.Name = "usernameChooser";
      this.usernameChooser.Size = new System.Drawing.Size(103, 20);
      this.usernameChooser.TabIndex = 9;
      // 
      // passwordChooser
      // 
      this.passwordChooser.Location = new System.Drawing.Point(263, 112);
      this.passwordChooser.Name = "passwordChooser";
      this.passwordChooser.PasswordChar = '#';
      this.passwordChooser.Size = new System.Drawing.Size(103, 20);
      this.passwordChooser.TabIndex = 11;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(199, 115);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(56, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Password:";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(210, 143);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 12;
      this.button1.Text = "Ok";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(291, 143);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 13;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(16, 8);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(355, 45);
      this.label7.TabIndex = 14;
      this.label7.Text = "Enter the hub connection parameters. The only mandatory parameter is the address," +
    " you can leave the other fields empty if you want to use default values.";
      // 
      // HubEdit
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(383, 176);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.passwordChooser);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.usernameChooser);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.pathChooser);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.portChooser);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.addressChooser);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.protocolChooser);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "HubEdit";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "HubEdit";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox protocolChooser;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox addressChooser;
    private System.Windows.Forms.TextBox portChooser;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox pathChooser;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox usernameChooser;
    private System.Windows.Forms.TextBox passwordChooser;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label7;
  }
}