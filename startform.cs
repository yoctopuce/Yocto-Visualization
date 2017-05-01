/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Application Main form
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Windows.Forms;



namespace YoctoVisualisation
{
  public partial class StartForm : Form
  {

    static List<Form> formlist = new List<Form>();
    bool MustHide = false;
    bool ClosingNow = false;
     XmlNode propnode =null;
    static ConfigForm configWindow = new ConfigForm();
    static RawDataForm rawDataWindow = new RawDataForm();
    static PropertiesForm propWindow = null;
    static LogForm logWindow = new LogForm();
    static String DefaultWindowName = "New Window ";

    public StartForm()
    {

      InitializeComponent();

      LogManager.Log("Application start, Welcome to Yocto-Visualization.");

     
      LogManager.LogNoTS("---------------------------------------------------------------------------------");
      LogManager.LogNoTS("Optional command line parameters:");
      LogManager.LogNoTS("-config xmlFilePath      Create/Use alternate \"xmlFilePath\" configuration file.");
      LogManager.LogNoTS("-log                     Automatically open log window");
      LogManager.LogNoTS("---------------------------------------------------------------------------------");
      LogManager.Log("Current config file is " + constants.configfile);
      LogManager.Log("Yoctopuce API verion is " + YAPI.GetAPIVersion());
    




      if (File.Exists(constants.configfile))
      {
        XmlDocument doc = new XmlDocument();
         doc.Load(constants.configfile);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
          switch (node.Name)
          {
            case "GraphForm": NewGraphForm(node); MustHide = true; break;
            case "GaugeForm": NewSolidGaugeForm(node); MustHide = true; break;
            case "angularGaugeForm": NewAngularGaugeForm(node); MustHide = true; break;
            case "digitalDisplayForm": NewDigitalDisplayForm(node); MustHide = true; break;
            case "Config": configWindow.Init(node);break;
            case "PropertiesForm": propnode = node; break;


          }

        }
      } else configWindow.DefaultInit();
      propWindow = new PropertiesForm(propnode);

      configWindow.CheckInit();
      YoctoTimer.Interval = 100;
      YoctoTimer.Enabled = true;
      if (constants.OpenLogWindowAtStartUp) LogManager.Show();
    }

  

    public static void RestoreWindowPosition(Form f, XmlNode initNode)
    {
      if (initNode != null)
      {
        Size s = new Size(0, 0);
        Point p = new Point(0, 0);
        bool sizeFound = false;
        bool positionFound = false;

        foreach (XmlNode node in initNode.ChildNodes)
        {
          switch (node.Name)
          {
            case "location":
              p = new Point(int.Parse(node.Attributes["x"].InnerText),
                                    int.Parse(node.Attributes["y"].InnerText));
              positionFound = true;
              break;
            case "size":
              s = new Size(int.Parse(node.Attributes["w"].InnerText),
                               int.Parse(node.Attributes["h"].InnerText));
              sizeFound = true;
              break;
          }
        }
        if ((sizeFound) && (positionFound))
        {
          bool positionOk = false;
          Rectangle r = new Rectangle(p, s);
          foreach (Screen screen in Screen.AllScreens)
          {
            if (screen.WorkingArea.IntersectsWith(r))
              positionOk = true;

          }
          if (positionOk)
          {
            f.Location = p;
            f.Size = s;

          }

        }
      }
    }

    public static string newWindowName()
    {
      int n = 1;
      for (int i = 0; i < formlist.Count; i++)
      {
        string name = formlist[i].Text;
        if (name.Length > DefaultWindowName.Length)
        {
          int k;
          if (Int32.TryParse(name.Substring(DefaultWindowName.Length), out k))
          {
            if (k >= n) n = k + 1;
          }
        }
      }

      return DefaultWindowName + n.ToString();

    }


    public static void NetworkArrival(YNetwork net)
    {
      LogManager.Log("Network device detected: " + net.get_hardwareId());
      configWindow.NetworkArrival(net);

    }

    public static void   DeviceRemoval(string serial)
    {
    
      configWindow.removal(serial);


    }
    public void ShowPropertyForm(Form caller, GenericProperties prop, SetValueCallBack PropertyChanged, bool forceToShow)
    {
     
      if (propWindow!=null) propWindow.showWindow(prop, PropertyChanged, forceToShow);

    }

    public int formCount
    { get { return formlist.Count; } 

    }

    public void removeForm(Form f)
    {
      formlist.Remove(f);
      if (formlist.Count <= 0) Show();

    }


   

    public void NewGraphForm(XmlNode initData)
    {
      Form f = new GraphForm(this, initData);
      f.Show();
      formlist.Add(f);

    }

    public void NewSolidGaugeForm(XmlNode initData)
    {
      Form f = new gaugeForm(this, initData);
      f.Show();
      formlist.Add(f);

    }

    public void NewAngularGaugeForm(XmlNode initData)
    {
      Form f = new angularGaugeForm(this, initData);
      f.Show();
      formlist.Add(f);

    }

    public void NewDigitalDisplayForm(XmlNode initData)
    {
      Form f = new digitalDisplayForm(this, initData);
      f.Show();
      formlist.Add(f);

    }


    private void button2_Click(object sender, EventArgs e)
    {
      NewGraphForm(null);
      this.ShowInTaskbar = false;
      Hide();


    }

    private void button3_Click(object sender, EventArgs e)
    {
      NewSolidGaugeForm(null);
      this.ShowInTaskbar = false;
      Hide();


    }

    private void button4_Click(object sender, EventArgs e)
    {
      NewAngularGaugeForm(null);
      this.ShowInTaskbar = false;
      Hide();

    }

    private void button5_Click(object sender, EventArgs e)
    {
      NewDigitalDisplayForm(null);
      this.ShowInTaskbar = false;
      Hide();
    }



    public void SaveConfig()
    {
      string XmlConfigFile = "<?xml version=\"1.0\" ?>\n<ROOT>\n";
      XmlConfigFile += configWindow.GetXMLConfiguration();
      foreach (Form f in formlist)
      {
        if (f is GraphForm) XmlConfigFile += ((GraphForm)f).getConfigData();
        if (f is gaugeForm) XmlConfigFile += ((gaugeForm)f).getConfigData();
        if (f is angularGaugeForm) XmlConfigFile += ((angularGaugeForm)f).getConfigData();
        if (f is digitalDisplayForm) XmlConfigFile += ((digitalDisplayForm)f).getConfigData();
      }
      XmlConfigFile += propWindow.getConfigData();
      XmlConfigFile += "</ROOT>\n";
      try
      {
        TextWriter tw = File.CreateText(constants.configfile);
        tw.WriteLine(XmlConfigFile);
        tw.Close();
      }
      catch (Exception e)
      {
        MessageBox.Show("Can't save config file:\n"+e.Message+"\nSorry.");
      }

    }

    public bool isClosing
    {  get { return ClosingNow; }

    }

    public void Terminate()
    {
      ClosingNow = true;
      SaveConfig();



      Application.Exit();
    }

    private void startform_FormClosing(object sender, FormClosingEventArgs e)
    {
      SaveConfig();
    }

    private void startform_Load(object sender, EventArgs e)
    {
      this.Visible = !MustHide;
      this.ShowInTaskbar = !MustHide;

    }

    private void YoctoTimer_Tick(object sender, EventArgs e)
    {
      YoctoTimer.Enabled = false;
      sensorsManager.run();
      YoctoTimer.Enabled = true;
    }

    public void ConfigureUSBNet()
    { configWindow.Show();
    }

    private void button1_Click(object sender, EventArgs e)
    {
     ConfigureUSBNet();

    }
    public void hidePropertyEditor()
    {
      propWindow.Hide();

    }

    public void ShowRawData()
    {
      rawDataWindow.showData();
    }

    private void button6_Click(object sender, EventArgs e)
    {
      ShowRawData();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start("https://lvcharts.net");
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {

    }

    private void showLogsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      LogManager.Show();
    }
  }
}
