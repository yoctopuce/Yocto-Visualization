using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace UpdateAppLibrary
{
  public partial class NewRelease : Form
  {
    private YAppRelease release;
    public YAppInterface _appInterface;
    public YAppReleaseManager _appReleaseManager;
    private string installerFile;

    public NewRelease(YAppInterface appInterface, YAppReleaseManager appReleaseManager)
    {
      InitializeComponent();
      _appInterface = appInterface;
      _appReleaseManager = appReleaseManager;
      checkUpdate.Checked = _appInterface.GetCheckUpdateSettings();
      this.checkUpdate.CheckedChanged += new System.EventHandler(this.checkUpdate_CheckedChanged);
    }

    enum UIstates { UPDATEAVAILABLE, UPDATENOTAVAILABLE, UPDATEDOWNLOADING };

    private void refreshUI(UIstates state)
    {
      switch (state)
      {
        case (UIstates.UPDATENOTAVAILABLE):
          progressBar1.Visible = false;
          CancelDownloadButton.Visible = false;
          DownloadLabel.Visible = false;
          buttonUpdate.Visible = true;
          buttonIgnore.Visible = true;
          buttonRemindeMe.Visible = true;
          buttonRelease.Visible = true;
          label1.Visible = true;
          labelNewVersion.Visible = true;
          labelLinkDecoration.Visible = true;
          linkLabel.Visible = true;
          checkUpdate.Visible = true;
          labelVersion.Visible = true;
          buttonUpdate.Enabled = false;
          buttonIgnore.Enabled = false;
          buttonRemindeMe.Enabled = false;
          buttonRelease.Enabled = false;
          label1.Text = String.Format("{0} is up to date:", _appInterface.GetAppName());
          labelNewVersion.Text = _appInterface.GetVersion();
          labelVersion.Text = "Current version:";
          labelLinkDecoration.Visible = false;
          linkLabel.Visible = false;
          break;

        case (UIstates.UPDATEAVAILABLE):
          progressBar1.Visible = false;
          CancelDownloadButton.Visible = false;
          DownloadLabel.Visible = false;
          buttonUpdate.Visible = true;
          buttonIgnore.Visible = true;
          buttonRemindeMe.Visible = true;
          buttonRelease.Visible = true;
          checkUpdate.Visible = true;
          label1.Visible = true;
          labelVersion.Visible = true;
          labelNewVersion.Visible = true;
          labelLinkDecoration.Visible = true;
          linkLabel.Visible = true;
          label1.Text = String.Format("Updates are available for the {0} application:", _appInterface.GetAppName());
          buttonUpdate.Enabled = true;
          buttonIgnore.Enabled = true;
          buttonRemindeMe.Enabled = true;
          linkLabel.Text = release.link;
          labelNewVersion.Text = _appReleaseManager.FormatVersion(release.version);
          break;

        case (UIstates.UPDATEDOWNLOADING):
          progressBar1.Visible = true;
          CancelDownloadButton.Visible = true;
          DownloadLabel.Visible = true;
          buttonUpdate.Visible = false;
          buttonIgnore.Visible = false;
          buttonRemindeMe.Visible = false;
          buttonRelease.Visible = false;
          label1.Visible = false;
          labelNewVersion.Visible = false;
          labelLinkDecoration.Visible = false;
          linkLabel.Visible = false;
          checkUpdate.Visible = false;
          labelVersion.Visible = false;
          break;

      }
    }



    private void NewRelease_Load(object sender, EventArgs e)
    {
      this.Text = String.Format("{0} Updates", _appInterface.GetAppName());
      this.CenterToScreen();
      release = _appReleaseManager.CheckLatestRelease();
      refreshUI(release == null ? UIstates.UPDATENOTAVAILABLE : UIstates.UPDATEAVAILABLE);
    }

    private void label3_Click(object sender, EventArgs e)
    { }

    private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(release.link);
    }

    public void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
    }

    // The event that will trigger when the WebClient is completed
    public void Completed(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        DownloadLabel.Text = "Download failed:" + e.Error.Message;
        return;
      }

      if (!e.Cancelled)
      {
        _appInterface.StopRunningProcess();
        System.Diagnostics.Process.Start(installerFile);
        System.Windows.Forms.Application.Exit();
      }
      else
      {
        File.Delete(installerFile);
        Close();
      }

    }


    private void buttonUpdate_Click(object sender, EventArgs e)
    {
      if (release.link.EndsWith(".msi"))
      {
        refreshUI(UIstates.UPDATEDOWNLOADING);
        installerFile = _appReleaseManager.DownloadInstaller(release.link, new DownloadProgressChangedEventHandler(ProgressChanged), new AsyncCompletedEventHandler(Completed));
        return;

      }
      else
      {
        System.Diagnostics.Process.Start(release.link);
      }
      System.Windows.Forms.Application.Exit();
    }

    private void buttonRelease_Click(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start(_appReleaseManager.GetReleaseNotes());
    }

    private void buttonIgnore_Click(object sender, EventArgs e)
    {
      _appInterface.SetBuildNumberToIgnoreSettings(release.version);
      this.Close();
    }

    private void checkUpdate_CheckedChanged(object sender, EventArgs e)
    {
      _appInterface.SetCheckUpdateSettings(checkUpdate.Checked);
    }

    private void buttonRemindeMe_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      _appReleaseManager.CancelDownload();
      Close();
    }
  }


  // a basic emulation of windows' balloontips  
  public class ShortMessage : Form
  {


    private string infoIconPNGbase64 =  "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABHNCSVQIC"
                             + "AgIfAhkiAAAAAlwSFlzAAABdQAAAXUBRHdm5gAAABl0RVh0U29mdHdhcm"
                             + "UAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAATASURBVFiFvVdNaFxVFP7u3/t"
                             + "JZtKYDpSaaguFIlpqFDdCdSGkdVMopFMsxaVgId1KpRoesf6tu1eQtIlM"
                             + "o1VBaLNRENpmIRoKtgGt9r+mdNJkZt579717j4vXxCaZyTx/mm/37j3fO"
                             + "d/lnXvOuQw5US4f67U2PgKT9BtrNxHIt2Q5AHDGLQMLuZTXJDDhdHR+OD"
                             + "Jy9FYev6ydwb59wS5j4uMm1duICIwxCKHAOQfjAgBAxsCSRZomADIbKZ1"
                             + "pIdVgpTI88a8ElMtBjzHJmUSHL4AxuMqD4/qQ0gVjzWlEhCSNoXUEHTcA"
                             + "xqCkN2lJ7T59OpjNLaBcHupP4+jr1BrPcTz4fhFCyCU227b1on/X8wCAi"
                             + "bM/Ynr6xpJ9Y1OEjTloHUMIWVPMf6lyOviprYD9+4cOxFE4QmR5Z2c3HM"
                             + "dfIfDx3vUYHn4dUma/IE0Nht79DDdv3lthq3WIen0WjDHjuN5rlcqxUw/"
                             + "v8+Unj6NwhABe7Co1DQ4Az/VtXQwOAFIK9PVtbWrrOD6KxRIITOg4GhsY"
                             + "CHY0FbB3b9Cd6Ogray0vFnoghWrqEACq1fkma7WW9lIqdBV7QETCpI1z5"
                             + "XLQs0IA58mEMcYvFLohZevgAHDhwmVMTV1Z/J76+QomJy+vyhFCobOzG8"
                             + "akHdYkXy6sMyC7anE0d8ZxfBQKj63qaJHIgN7eEgDgxo27IMpFQ612D1r"
                             + "H5LjFvvHxYEoCgDHxcTAG3y/m8wKACLh+/W5u+wX4fhe0/pMRxScBbGfl"
                             + "8rHeOKped5SHzpyn/6+o16vQOkLP+uJGaW18hIjguM0zvhW2b9+CcnknA"
                             + "KBS+QEXL/6em6uUjzgOMT9vj3CYpD8rnW5uB56nMHh4DzZv2YDNWzZg8P"
                             + "AeeN7qibtUgAuAgWy6ixtrNwmhWpbXZiiV1sHznIcEOSiV1uXmZwdWsGS"
                             + "e4ATrc87bs5Y5yLO2GgSXIGM6uLWWL3S1tQTjDESUHZ3lvMOPApxzbi2Z"
                             + "NQ9MlsAYs5wxEVpr11yAsSkYFw3OhbiWpgkoby39H0BEMEZDcHGVS2ACy"
                             + "CaZtUKSxCACGOdnOInuDxhj0DpaOwE6BGMMyvE/5pXKW7eldKZ13ICx6S"
                             + "MPbkyCWIeQyr00Onr0DgcAIdUgGEPYmHvkAhqNbJgRQrwJPBhIKpXhCSW"
                             + "9Sa1jaB22ddIsYfMksdYhkiSC43jnK5X3vl8UAACW1G4hZK1eryJN9aqO"
                             + "ZmbuIwz/TtowjDEzc39VjjEJ6vVZSKHmPd/ZvbC+WIMvXfouevbpV741Z"
                             + "N6IdcSVdMFblGhjLH779RY2buxBtVrDp5+cxe3b1dbBU425WhWMwSi3+8"
                             + "WxsaE/FvZWdJBy+Z19Oo7GrCVRKDQfy/8JtA5Rq82CM2Y8t7D/81PBFw/"
                             + "vN21hAwPBU9aE59M0WSeVi06/C6LNoLoc2cNkHlpHkELOc+nvHB8Pppbb"
                             + "teyhBw8GXVGYfJMk0ctEBNf1oZQPpdo8zZIYiQ4RP0hmpbxzfofz6okTQ"
                             + "dMr1raJDwwEO4jik2min8kyPRsmBJdgPKOTJRibwhidVbhswvpFSHFoId"
                             + "tbIfcUceDA+xu0jt+GTfsNmSfJmI4lz3MhGoKJq0yws0p1fDQ6evROHr9"
                             + "/AXm1KIVEVusYAAAAAElFTkSuQmCC";



    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    //private System.Windows.Forms.Label label3;
    private Timer closeTimer;
    private EventHandler _onclick;
    private int n = 0;
    //private int fullwidth = 300;  //pixels
    private int displayDelay = 10;  // secondes
    private int speedfactor = 4;

    PictureBox pictureBox1;



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

    private void InitializeComponent(String Title, String body)
    {
     


      AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      ClientSize = new System.Drawing.Size(300, 50);
      label1 = new System.Windows.Forms.Label();
      label2 = new System.Windows.Forms.Label();
      SuspendLayout();
      FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "NewRelease";
      ShowIcon = false;
      Text = "Updates";
      BackColor = Color.LightGoldenrodYellow;
      label1.Text = Title;
      label1.AutoSize = true;
      label1.Click += BreafMessage_Click;
      label1.Font = new Font(label1.Font, FontStyle.Bold);
      label1.Location = new System.Drawing.Point(48, 5);
      Controls.Add(label1);
      label2.Text = body;
      label2.AutoSize = true;
      label2.Click += BreafMessage_Click;
      label2.Location = new System.Drawing.Point(label1.Left, label1.Bottom + 5);
      Controls.Add(label2);


      // 
      // pictureBox1
      // 
      pictureBox1 = new System.Windows.Forms.PictureBox();
      pictureBox1.Image = Image.FromStream(new MemoryStream(Convert.FromBase64String(infoIconPNGbase64)));
      pictureBox1.Location = new System.Drawing.Point(10, 10);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new System.Drawing.Size(32, 32);
      pictureBox1.TabIndex = 16;
      pictureBox1.TabStop = false;
      Controls.Add(pictureBox1);
      Click += BreafMessage_Click;
      StartPosition = FormStartPosition.Manual;

      Location = new Point(Screen.PrimaryScreen.Bounds.Right - this.Width, Screen.PrimaryScreen.Bounds.Top - this.Height);
      Size = new Size(15 + label1.Left+ Math.Max(label1.Width, label2.Width), 25 + label1.Height + label2.Height);
      ResumeLayout(false);
      PerformLayout();

    }

    private void Label3_Click(object sender, EventArgs e)
    {
      closeTimer.Enabled = false;
      Close();
    }

    private void BreafMessage_Click(object sender, EventArgs e)
    {
      _onclick(sender, e);
      closeTimer.Enabled = false;
      Close();

    }

    public ShortMessage(String Title, String body, EventHandler onclick)
    {

      InitializeComponent(Title, body);
      _onclick = onclick;
      closeTimer = new Timer();
      closeTimer.Interval = 10;
      closeTimer.Tick += CloseTimer_Tick;
      closeTimer.Enabled = true;
    }

    private void CloseTimer_Tick(object sender, EventArgs e)
    {
      n++;

      if (n < 2 + this.Height / speedfactor)
      {
        //this.Width = speedfactor * n;
        this.Top = Screen.PrimaryScreen.Bounds.Top - this.Height + speedfactor * n;
      }

      if (n > displayDelay * 100 - (this.Height / speedfactor))
      {
        int h = speedfactor * ((displayDelay * 100) - n);



        this.Top = Screen.PrimaryScreen.Bounds.Top - this.Height + h;
        if (h == 0)
        {
          closeTimer.Enabled = false;
          Close();
        }

      }


    }
  }

}