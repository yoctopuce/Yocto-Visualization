/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Application Main form
 * 
 *  
 *   - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2017 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using YoctoVisualization;
using System.Globalization;

namespace YoctoVisualisation
{
  public partial class StartForm : Form
  {

    static List<Form> formlist = new List<Form>();
    bool MustHide = false;
    bool ClosingNow = false;
  //   XmlNode propnode =null;
    static ConfigForm configWindow; 
    static RawDataForm rawDataWindow;
   
    static PropertiesForm2 propWindow2 = null;
    static LogForm logWindow = new LogForm();
    static String DefaultWindowName = "New Window ";
  	static Timer HideTimer;

 


    public StartForm(bool debuggerAttached)
    {

      configWindow = new ConfigForm();
      configWindow.initCaptureParameters();
      rawDataWindow = new RawDataForm();
      propWindow2 = new PropertiesForm2();

      InitializeComponent();
      version.Text = constants.buildVersion;
      MaximizeBox = false;
      LogManager.Log("Application start, Welcome to Yocto-Visualization.");

      if (debuggerAttached) LogManager.Log("A debugger is attached.");
      LogManager.LogNoTS("---------------------------------------------------------------------------------");
      LogManager.LogNoTS("Optional command line parameters:");
      LogManager.LogNoTS("-config xmlFilePath      Create/Use alternate \"xmlFilePath\" configuration file.");
      LogManager.LogNoTS("                         You can use configuration files from V1, but they will ");
      LogManager.LogNoTS("                         be overwritten in V2 format.");
      LogManager.LogNoTS("-log                     Automatically open log window");
      LogManager.LogNoTS("-check4updates           Re-enable automatic checks for updates");
      LogManager.LogNoTS("---------------------------------------------------------------------------------");
      LogManager.Log("Current configuration file is " + constants.configfile);
      LogManager.Log("Yocto-Visualization version is " + constants.buildVersion);
      LogManager.Log("Yoctopuce API version is " + YAPI.GetAPIVersion());
      LogManager.Log("Yoctopuce dynamic library path is " + YAPI.GetYAPIDllPath() );

      LogManager.Log("Architecture is " + (IntPtr.Size * 8).ToString() + " bits (platform " + Environment.OSVersion.Platform.ToString() + ")");

      

      if (constants.MonoRunning)
      {
        if (constants.OSX_Running)
        {
          LogManager.Log("Mono version is " + constants.MonoVersion + " (Mac OS X)");
        }
        else
        {
          LogManager.Log("Mono version is " + constants.MonoVersion);
        }
      }
      else
      {
        LogManager.Log("Running on .NET");
#if NET35
        LogManager.Log("WARNING! this .EXE was specifically compiled for .NET 3.5 (Windows XP compatibility)");
#endif
      }
      string cfgFile = constants.configfile;

      if (!File.Exists(cfgFile) && !constants.configfileOveridden)
      {
        string alternateCfgFile = Path.GetDirectoryName(cfgFile) + "\\..\\..\\YoctoVisualization\\1.0.0.0\\config.xml";
        if (File.Exists(alternateCfgFile))
          if (MessageBox.Show("No Yocto-Visualization V2 configuration available, but a configuration file from version 1 was found, do you want to import it?", "Yocto-Visualization V2",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
          {
            cfgFile = alternateCfgFile;
          }

      }

     

      if (File.Exists(cfgFile))
      {
        try
        {
          XmlDocument doc = new XmlDocument();

          doc.Load(cfgFile);

          double version = 1;
          XmlElement root = doc.DocumentElement;

          if (root.Attributes != null
             && root.Attributes["version"] != null)
            version = Double.Parse(root.Attributes["version"].Value, CultureInfo.InvariantCulture);

          GenericProperties.XmlFileVersion = version;

          if (version == 1.0)
          {
            string configdata = XMLConfigTranslator.TranslateFromV1(doc);
            doc = new XmlDocument();
            doc.LoadXml(configdata);
          }



          // sensor configuration must be loaded first
          foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            if (node.Name == "Sensors")
              sensorsManager.setKnownSensors(node);


          foreach (XmlNode node in doc.DocumentElement.ChildNodes)
          {
            switch (node.Name)
            {
              case "GraphForm": NewGraphForm(node); MustHide = true; break;
              case "GaugeForm": NewSolidGaugeForm(node); MustHide = true; break;
              case "angularGaugeForm": NewAngularGaugeForm(node); MustHide = true; break;
              case "digitalDisplayForm": NewDigitalDisplayForm(node); MustHide = true; break;
              case "Config": configWindow.Init(node); break;
              case "PropertiesForm": RestoreWindowPosition(propWindow2, node); break;

            }

          }
        }
        catch (Exception e) { LogManager.Log("Cannot load configuration file " + constants.configfile + ": " + e.Message); }

      }
      else configWindow.DefaultInit();



      sensorsManager.registerChangeCallback(sensorListHaschanged);

      configWindow.CheckInit();
      YoctoTimer.Interval = 100;
      YoctoTimer.Enabled = true;
      if (constants.OpenLogWindowAtStartUp) LogManager.Show();

      HideTimer = new Timer();

      HideTimer.Tick += HideTimer_Tick;


    }


    public void sensorListHaschanged()
    {
      propWindow2.refresh();
    }

    void HideTimer_Tick (object sender, EventArgs e)

    { HideTimer.Enabled = false;
		Hide();

    	
    }

    public void  refreshPropertiesForm()
    { 
      if (propWindow2 != null) propWindow2.refresh();
    }

    public static void RestoreWindowPosition(Form f, XmlNode initNode)
    {
      if (initNode != null)
      {
        Size s = new Size(0, 0);
        Point p = new Point(0, 0);
        bool sizeFound = false;
        bool positionFound = false;
        FormWindowState state = FormWindowState.Normal;

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

             
              if  (node.Attributes["state"]!=null)
                {  if (node.Attributes["state"].Value ==  FormWindowState.Maximized.ToString()) state = FormWindowState.Maximized; 
                   if (node.Attributes["state"].Value ==  FormWindowState.Minimized.ToString()) state = FormWindowState.Minimized;
                }

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
          if (constants.OSX_Running && r.Y == 0)
          {
            positionOk = false;
          }
          if (positionOk)
          {
            f.Location = p;
            f.Size = s;
            f.WindowState = state;
          }

        }
      }  else
      {
        f.Location = new Point( (Screen.FromControl(f).Bounds.Width - f.Size.Width)>>1 ,
                                (Screen.FromControl(f).Bounds.Height- f.Size.Height)>> 1 );
            

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

    public void widgetLostFocus(Form caller)
    {
      propWindow2.setTopmost(false);
    }

    public void ShowPropertyForm(Form caller, GenericProperties prop,  
               SetValueCallBack2 PropertyChanged2,
               GetValueCallBack2 getPropertyValue,
               bool forceToShow)
    {

      if (propWindow2 != null)
      {
        propWindow2.showWindow(caller, prop, PropertyChanged2, getPropertyValue, forceToShow);
        propWindow2.setTopmost(true);
      }

    }

   



    public int formCount
    { 
      get { return formlist.Count; } 
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

    public new  void Hide()
   { 

			WindowState = FormWindowState.Minimized;
	}

	public  new  void Show()
		{ WindowState = FormWindowState.Normal;
	}

    private void button2_Click(object sender, EventArgs e)
    {
      NewGraphForm(null);
	 if (!constants.MonoRunning) this.ShowInTaskbar = false;
      Hide();


    }

    private void button3_Click(object sender, EventArgs e)
    {
      NewSolidGaugeForm(null);
	  if (!constants.MonoRunning)this.ShowInTaskbar = false;
      Hide();


    }

    private void button4_Click(object sender, EventArgs e)
    {
      NewAngularGaugeForm(null);
	    if (!constants.MonoRunning) this.ShowInTaskbar = false;
      Hide();

    }

    private void button5_Click(object sender, EventArgs e)
    {
      NewDigitalDisplayForm(null);
	  if (!constants.MonoRunning)this.ShowInTaskbar = false;
      Hide();
    }



    public void SaveConfig()
    {
      string XmlConfigFile = "<?xml version=\"1.0\" ?>\n"
        +"<!--\n"
        +"   ******************************************\n"
        +"   Yocto-Visualization 2.0 Configuration file\n"
        +"   ******************************************\n"
        +"-->\n"
        +"<ROOT version='2.1'>\n"
        +"<!-- in 2.1 version, floating point values are saved in CultureInfo.InvariantCulture form -->\n";
      XmlConfigFile += configWindow.GetXMLConfiguration();
      foreach (Form f in formlist)
      {
        if (f is GraphForm) XmlConfigFile += ((GraphForm)f).getConfigData();
        if (f is gaugeForm) XmlConfigFile += ((gaugeForm)f).getConfigData();
        if (f is angularGaugeForm) XmlConfigFile += ((angularGaugeForm)f).getConfigData();
        if (f is digitalDisplayForm) XmlConfigFile += ((digitalDisplayForm)f).getConfigData();
      }
      XmlConfigFile += propWindow2.getConfigData();
      XmlConfigFile += sensorsManager.getXMLSensorsConfig();


      XmlConfigFile += "</ROOT>\n";
      try
      {
        TextWriter tw = File.CreateText(constants.configfile);
        tw.WriteLine(XmlConfigFile);
        tw.Close();
      }
      catch (Exception e)
      {
        MessageBox.Show("Can't save configuration file:\n"+e.Message+"\nSorry.");
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
      YAPI.FreeAPI();

    }

    private void startform_Load(object sender, EventArgs e)
    {
 	 if (MustHide) 
			{  if (constants.MonoRunning) 
				{  // cant get mono minimize this window while in load function
					HideTimer.Interval = 250;
					HideTimer.Enabled = true;
			} else Hide();
		}
	  if (!constants.MonoRunning) this.ShowInTaskbar = !MustHide;

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
      propWindow2.Hide();

    }

    public void ShowRawData()
    {
      rawDataWindow.showData();
    }

    private void button6_Click(object sender, EventArgs e)
    {
      ShowRawData();
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
