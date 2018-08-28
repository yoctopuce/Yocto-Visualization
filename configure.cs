/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  USB / Network configuation window
 * 
 *  - - - - - - - - - License information: - - - - - - - - -
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
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using System.Windows.Forms;
using System.Xml;
using YColors;
using YDataRendering;
using YoctoVisualization;

namespace YoctoVisualisation
{
  public partial class ConfigForm : Form
  {
    string VirtualHubSerial = "";
    List<Hub> hubList = new List<Hub>();
    string localIP = GetLocalIPAddress();
    bool initdone = false;
    System.Diagnostics.PerformanceCounter performance = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
    public ConfigForm()
    {
      InitializeComponent();

      editButton.Enabled = false;
      deleteButton.Enabled = false;

      if (constants.MonoRunning)
      {
        ExportToClipboard.Text = "Export to clipboard (2) (not available on linux).";
        ExportToClipboard.Enabled = false;
       
        ExportToPNG.Checked = true;

      }
    }

    public static string GetLocalIPAddress()
    {

      if (!NetworkInterface.GetIsNetworkAvailable())
        return "";

      var host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (var ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
          return ip.ToString();
        }
      }
      return "";
    }


    private void Add_Click(object sender, EventArgs e)
    {

      HubEdit editor = new HubEdit();
      Hub h;
      if (editor.newHub(out h)) AddHub(h);
    
      
    }



    private void RemoveHub(Hub.HubType t, string fullURL)
    {  if  (t == Hub.HubType.LOCALUSB)
      
        {
          LogManager.Log("unRegistering  USB ");
          YAPI.UnregisterHub("usb");
          usbFailed.Visible = false;
          usbOk.Visible = false;
          return;
        }
       else
       {
       if (t == Hub.HubType.LOCALHUB)
        {
            LogManager.Log("unRegistering  Local VirtualHub (127.0.0.1) ");
            YAPI.UnregisterHub("127.0.0.1");
            hubFailed.Visible = false;
            hubOk.Visible = false;
            hubWaiting.Visible = false;
            return;
          }

       }

      for (int i = hubList.Count-1;i>=0;i--)
      {
        if (hubList[i].get_obfuscatedURL() == fullURL)
        {
          Hub h = hubList[i];
          hubList.RemoveAt(i);
          string url = h.get_obfuscatedURL();
          LogManager.Log("unRegistering " + h.get_obfuscatedURL());
          YAPI.UnregisterHub( h.get_fullUrl());
          for (int j = listView1.Items.Count - 1; j >= 0; j--)
            if (listView1.Items[j].Text == url)
              listView1.Items.RemoveAt(j);
        }
      }





    }



    

    private void AddHub(Hub h)
    {
      string errmsg = "";
      if (h.hubType == Hub.HubType.LOCALUSB) 
      {
        LogManager.Log("preregistering  USB ");
        if (YAPI.RegisterHub("usb", ref errmsg) != YAPI.SUCCESS)
        {
          LogManager.Log("[!] preregistering USB failed failed (" + errmsg + ")");
          ToolTip tt = new ToolTip();
          tt.SetToolTip(usbFailed, errmsg);
          usbFailed.Visible = true;
        }
        else
        {
          usbOk.Visible = true;
          if (UseVirtualHub.Checked)
          {
            hubOk.Visible = false;
            hubWaiting.Visible = false;
            hubFailed.Visible = true;
            ToolTip tt = new ToolTip();
            tt.SetToolTip(hubFailed, "Local VirtualHub is OFFLINE");
          }
        }
        return;
      }

      if (h.hubType == Hub.HubType.LOCALHUB)
      {
        LogManager.Log("preregistering  local VirtualHub (127.0.0.1)" );
        if (YAPI.PreregisterHub("127.0.0.1", ref errmsg) != YAPI.SUCCESS)
        {
          ToolTip tt = new ToolTip();
          tt.SetToolTip(usbFailed, errmsg);
          hubFailed.Visible = true;
          LogManager.Log("[!] preregistering local VirtualHub failed failed (" + errmsg + ")");
        }
        else
        {
          hubWaiting.Visible = true;
        }

        return;
      }
      

      ListViewItem item = listView1.Items.Add(h.get_obfuscatedURL());
      hubList.Add(h);
      item.SubItems.Add("Contacting...");
      item.SubItems.Add("");
      item.ImageIndex = 0;
      LogManager.Log("preregistering  " + h.get_obfuscatedURL());
      if (YAPI.PreregisterHub(h.get_fullUrl(), ref errmsg) != YAPI.SUCCESS)
      {
        LogManager.Log("[!] preregistering  " + h.get_obfuscatedURL() + " failed (" + errmsg + ")");
        item.SubItems.Add(errmsg);
        item.ImageIndex = 1;
        return;
      }
    



    }

    public string GetXMLConfiguration()
    {
      string res = "<Config>\n"
         + "  <UseUSB value=\"" + (useUSB.Checked ? "TRUE" : "FALSE") + "\"/>\n"
         + "  <UseVirtualHub value=\"" + (UseVirtualHub.Checked ? "TRUE" : "FALSE") + "\"/>\n"
         + "  <Hubs>\n";
      for (int i = 0; i < hubList.Count; i++)
        res += "    "+hubList[i].XmlCode()+"\n";

      res += "  </Hubs>\n";

      res += "  <Colors>\n";
      List<YColor> c = ColorEditor.colorHistory;
      for (int i = c.Count - 1; i >= 0; i--) 
         res = res + "    <Color value=\"" + c[i].ToString() + "\"/>\n";
      res += "  </Colors>\n";
      res += "  <MemoryUsage>\n";
      res += "    <maxPointsPerGraphSerie value= \"" + constants.maxPointsPerGraphSerie.ToString() + "\"/>\n";
      res += "    <maxDataRecordsPerSensor value= \"" + constants.maxDataRecordsPerSensor.ToString() + "\"/>\n";
      res += "    <maxPointsPerDataloggerSerie value= \"" + constants.maxPointsPerDataloggerSerie.ToString() + "\"/>\n";
      res += "    <deviceListValidity value= \"" +  YAPI.GetDeviceListValidity().ToString() + "\"/>\n";
      res += "  </MemoryUsage>\n";
      res += "  <Capture>\n";
      res += "    <Target value= \"" + constants.captureTarget.ToString() + "\"/>\n";
      res += "    <Size value= \"" + constants.captureSizePolicy.ToString() + "\"/>\n";
      res += "    <Resolution value= \"" + constants.captureDPI.ToString() + "\"/>\n";
      res += "    <Folder value= \"" + System.Security.SecurityElement.Escape(constants.captureFolder) + "\"/>\n";
      res += "    <Width value= \"" + constants.captureWidth.ToString() + "\"/>\n";
      res += "    <Height value= \"" + constants.captureHeight.ToString() + "\"/>\n";    
      res += "  </Capture>\n";
      res += "  <UI>\n";
      res += "    <VerticalDragZoom value= \"" + YGraph.VerticalDragZoomEnabled.ToString() + "\"/>\n";
      res += "    <DbleClickContextMenu value= \"" + constants.dbleClickBringsUpContextMenu.ToString() + "\"/>\n";   
      res += "  </UI>\n";
      res += "</Config>\n";

      return res;
    }

    public void InitColorHistory(XmlNode ColorsNode)
    {
      foreach (XmlNode node in ColorsNode.ChildNodes)
      {
        switch (node.Name)
        {
          case "Color":
            ColorEditor.AddColorToHistory( YColor.fromString(node.Attributes["value"].InnerText));        
            break;
        }

      }
    }

    public void InitUIParams(XmlNode uiNode)
    {
      foreach (XmlNode node in uiNode.ChildNodes)
      {
        bool value;
        switch (node.Name)
        {
          case "VerticalDragZoom":
            value = false;
            if (Boolean.TryParse(node.Attributes["value"].InnerText, out value))          
               YGraph.VerticalDragZoomEnabled = value;                         
            break;

          case "DbleClickContextMenu":
            value = false;
            if (Boolean.TryParse(node.Attributes["value"].InnerText, out value))
              constants.dbleClickBringsUpContextMenu = value;
            break;

           

        }
      }
    }


    public void InitMemoryUsageParams(XmlNode memNode)
    {
      foreach (XmlNode node in memNode.ChildNodes)
      {
        int value;
        switch (node.Name)
        {
          case "maxPointsPerGraphSerie":
            value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if (value >= 0)
              {
                constants.maxPointsPerGraphSerie = value;
                DataSerie.MaxPointsPerSeries = value;
              }
            break;
          case "maxDataRecordsPerSensor":
            value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if (value >= 0)
              {
                constants.maxDataRecordsPerSensor = value;
                CustomYSensor.MaxDataRecords = value;
              }
            break;


          case "maxPointsPerDataloggerSerie":
               value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
               {
                constants.maxPointsPerDataloggerSerie = value;
                CustomYSensor.MaxLoggerRecords = value;
              }
            break;


          case "deviceListValidity":
            value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if (value > 0)
              {
                YAPI.SetDeviceListValidity(value);
              }
            break;


        }

      }
    }

    public void InitCaptureParams(XmlNode CaptureNode)
    { int value;
      foreach (XmlNode node in CaptureNode.ChildNodes)
      {
        switch (node.Name)
        {
          case "Target":
            if (node.Attributes["value"].InnerText == YDataRenderer.CaptureTargets.ToClipBoard.ToString()) constants.captureTarget =YDataRenderer.CaptureTargets.ToClipBoard;
            if ((node.Attributes["value"].InnerText == YDataRenderer.CaptureTargets.ToPng.ToString())||(constants.MonoRunning)) constants.captureTarget =YDataRenderer.CaptureTargets.ToPng;
          
            break;
          case "Size":
            if (node.Attributes["value"].InnerText == YDataRenderer.CaptureFormats.Fixed.ToString()) constants.captureSizePolicy =YDataRenderer.CaptureFormats.Fixed;
            if (node.Attributes["value"].InnerText == YDataRenderer.CaptureFormats.Keep.ToString()) constants.captureSizePolicy = YDataRenderer.CaptureFormats.Keep;
            if (node.Attributes["value"].InnerText == YDataRenderer.CaptureFormats.FixedWidth.ToString()) constants.captureSizePolicy = YDataRenderer.CaptureFormats.FixedWidth;
            if (node.Attributes["value"].InnerText == YDataRenderer.CaptureFormats.FixedHeight.ToString()) constants.captureSizePolicy =YDataRenderer.CaptureFormats.FixedHeight;
            break;
          case "Resolution":
             value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if (value > 0) constants.captureDPI = value;
            break;
          case "Width":
            value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if ((value >= 16) && (value <= 4096))  constants.captureWidth = value;
            break;
          case "Height":
            value = 0;
            if (Int32.TryParse(node.Attributes["value"].InnerText, out value))
              if ((value >= 16) && (value <= 4096)) constants.captureHeight = value;
            break;
          case "Folder":
            constants.captureFolder = node.Attributes["value"].InnerText;
            break;



        }

      }

    }

    public void updateCaptureUI()
    {
      targetFolder.Enabled = constants.captureTarget != YDataRenderer.CaptureTargets.ToClipBoard;
      CaptureFolderbutton.Enabled = targetFolder.Enabled;
      Color activeColor = SystemColors.ControlText;
      Color inactiveColor = SystemColors.GrayText;

      switch (sizePolicy.SelectedIndex)
      {
        case (int)YDataRendering.YDataRenderer.CaptureFormats.Fixed:
          widthValue.Enabled = true;
          widthLabel.ForeColor = activeColor;
          widthUnit.ForeColor = activeColor;
          heightValue.Enabled = true;
          heightLabel.ForeColor = activeColor;
          heightUnit.ForeColor = activeColor;
          break;
        case (int)YDataRendering.YDataRenderer.CaptureFormats.FixedHeight:
          widthValue.Enabled = false;
          widthLabel.ForeColor = inactiveColor;
          widthUnit.ForeColor = inactiveColor;
          heightValue.Enabled = true;
          heightLabel.ForeColor = activeColor;
          heightUnit.ForeColor = activeColor;
          break;
        case (int)YDataRendering.YDataRenderer.CaptureFormats.FixedWidth:
          widthValue.Enabled = true;
          widthLabel.ForeColor = activeColor;
          widthUnit.ForeColor = activeColor;
          heightValue.Enabled = false;
          heightLabel.ForeColor = inactiveColor;
          heightUnit.ForeColor = inactiveColor;
          break;
        case (int)YDataRendering.YDataRenderer.CaptureFormats.Keep:
          widthValue.Enabled = false;
          widthLabel.ForeColor = inactiveColor;
          widthUnit.ForeColor = inactiveColor;
          heightValue.Enabled = false;
          heightLabel.ForeColor = inactiveColor;
          heightUnit.ForeColor = inactiveColor;
          break;
      }

      heightUnit.Text = "px (" + (25.4 * (double)constants.captureHeight / (constants.captureDPI)).ToString("0.#") + "mm)";
      widthUnit.Text = "px (" + (25.4 * (double)constants.captureWidth / (constants.captureDPI)).ToString("0.#") + "mm)";
   

    }

    public void initCaptureParameters()
    {
      ExportToClipboard.Checked = constants.captureTarget == YDataRenderer.CaptureTargets.ToClipBoard;
      ExportToPNG.Checked = constants.captureTarget ==YDataRenderer.CaptureTargets.ToPng;
      targetFolder.Text = constants.captureFolder;
      VerticalDragZoom.Checked = YGraph.VerticalDragZoomEnabled;
      dbleClickBringsUpContextMenu.Checked = constants.dbleClickBringsUpContextMenu;


      DpiTextBox.Text = constants.captureDPI.ToString();
      int n = 0;
      foreach (YDataRenderer.CaptureFormats v in Enum.GetValues(typeof(YDataRenderer.CaptureFormats)))
      {
        FieldInfo fi = typeof(YDataRenderer.CaptureFormats).GetField(Enum.GetName(typeof(YDataRenderer.CaptureFormats), v));
        DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
        sizePolicy.Items.Add(dna.Description);
        if (constants.captureSizePolicy == v) sizePolicy.SelectedIndex = n;
        n++;
      }
      widthValue.Text = constants.captureWidth.ToString();
      heightValue.Text = constants.captureHeight.ToString();
      updateCaptureUI();
    }

    public void DefaultInit()
    {
      useUSB.Checked = true;
      AddHub(new Hub(Hub.HubType.LOCALUSB));
      UseVirtualHub.CheckedChanged += UseVirtualHub_CheckedChanged;
      useUSB.CheckedChanged += useUSB_CheckedChanged;

   
    }

    public void Init(XmlNode initData)
    {
      initdone = true;
      foreach (XmlNode node in initData.ChildNodes)
      {
        switch (node.Name)
        {
          case "UseUSB":
            useUSB.Checked = (node.Attributes["value"].InnerText.ToUpper() == "TRUE");
            if (useUSB.Checked) AddHub(new Hub(Hub.HubType.LOCALUSB));
            break;
          case "Colors":
            InitColorHistory(node);
            break;
          case "Capture":
            InitCaptureParams(node);
            break;
          case "UI":
            InitUIParams(node);
            break;
          case "MemoryUsage":
            InitMemoryUsageParams(node);
            break;
          case "UseVirtualHub":
            UseVirtualHub.Checked = (node.Attributes["value"].InnerText.ToUpper() == "TRUE");
            if (UseVirtualHub.Checked) AddHub(new Hub(Hub.HubType.LOCALHUB));
            break;
          case "Hubs":
            foreach (XmlNode subnode in node.ChildNodes)
              if (subnode.Name == "Hub")
              {

                AddHub(new Hub(subnode));
              } 
            break;
        }

      }
      UseVirtualHub.CheckedChanged += UseVirtualHub_CheckedChanged;
      useUSB.CheckedChanged += useUSB_CheckedChanged;
      initCaptureParameters();
    }

    

    public void NetworkArrival(YNetwork net)
    {
      string ip = net.get_ipAddress();
      string netname = net.get_logicalName();
      string loginame = net.get_module().get_logicalName();
      string url = net.get_module().get_url();

      for (int j=0;j<hubList.Count;j++)
        if (hubList[j].get_connexionUrl() == url)
        {
           for (int i = 0; i < listView1.Items.Count; i++)
            if (listView1.Items[i].Text == hubList[j].get_obfuscatedURL())
            {
              listView1.Items[i].ImageIndex = 2;
              listView1.Items[i].SubItems[1].Text = (loginame != "" ? loginame : netname) + " OK";
              listView1.Items[i].SubItems[2].Text = net.get_module().get_serialNumber();
                return;
            }

         }

      if ((ip == "127.0.0.1") || (ip == localIP) || (ip == "0.0.0.0"))
      {
        hubFailed.Visible = false;
        hubOk.Visible = true;
        hubWaiting.Visible = false;
        VirtualHubSerial = net.get_module().get_serialNumber();
        return;
      }
    
    }

    public void removal(string serialNumber)
    {

      


      if (serialNumber == VirtualHubSerial)
      {
        ToolTip tt = new ToolTip();
        tt.SetToolTip(usbFailed, "OFFLINE");
        hubFailed.Visible = true;
        hubOk.Visible = false;
        hubWaiting.Visible = false;
        return;
      }

      for (int i = 0; i < listView1.Items.Count; i++)
        if (listView1.Items[i].SubItems[2].Text == serialNumber)
        {
          listView1.Items[i].ImageIndex = 1;
          listView1.Items[i].SubItems[1].Text = "OFFLINE";

        }
    }



    private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }

    private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      Menuitemremove.Enabled = (listView1.SelectedItems.Count > 0);

    }

    private void Menuitemremove_Click(object sender, EventArgs e)
    {
      deleteButton_Click(sender, e);
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {

    }

    private void UseVirtualHub_CheckedChanged(object sender, EventArgs e)
    {
      if (UseVirtualHub.Checked) AddHub(new Hub(Hub.HubType.LOCALHUB)); else RemoveHub(Hub.HubType.LOCALHUB,"");
    }

    private void useUSB_CheckedChanged(object sender, EventArgs e)
    {
      if (useUSB.Checked) AddHub(new Hub(Hub.HubType.LOCALUSB)); else RemoveHub(Hub.HubType.LOCALUSB,"");
    }

    public void CheckInit()
    {
      if (!initdone) useUSB.Checked = true;

    }

    private void newEntry_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        Add_Click(null, null);
      }
    }

   


    private void openThisHubConfigurationPageToolStripMenuItem_Click(object sender, EventArgs e)
    {
      int lindex, hindex;
      findSelectedHub(out lindex, out hindex);
      if (hindex>=0)
        {
        System.Diagnostics.Process.Start(hubList[hindex].get_consoleUrl());

      }


    
    }




    private void label10_Click(object sender, EventArgs e)
    {

    }

    private void label10_Click_1(object sender, EventArgs e)
    {

    }

    private void sizePolicy_SelectedIndexChanged(object sender, EventArgs e)
    {
      int n = 0;
      foreach (YDataRenderer.CaptureFormats v in Enum.GetValues(typeof(YDataRenderer.CaptureFormats)))
      {

        if (sizePolicy.SelectedIndex == n)
        {
          constants.captureSizePolicy = v;
         
        }
        n++;
      }
      updateCaptureUI();
    }

    private void ExportToClipboard_CheckedChanged(object sender, EventArgs e)
    {
      constants.captureTarget = ExportToClipboard.Checked ? YDataRendering.YDataRenderer.CaptureTargets.ToClipBoard : YDataRendering.YDataRenderer.CaptureTargets.ToPng;
      updateCaptureUI();


    }

    private void CaptureFolderbutton_Click(object sender, EventArgs e)
    {
      if (Directory.Exists(targetFolder.Text)) folderBrowserDialog1.SelectedPath = targetFolder.Text;
      folderBrowserDialog1.Description = "Choose where screenshots will be saved.";
      folderBrowserDialog1.ShowDialog();
      targetFolder.Text = folderBrowserDialog1.SelectedPath;

    }

    private void targetFolder_Leave(object sender, EventArgs e)
    {
      if (!Directory.Exists(targetFolder.Text)) MessageBox.Show("Folder " + targetFolder.Text + " does not exists,\nCapture operations are likely to fail.",
                    "Parameter error", MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);

      constants.captureFolder = targetFolder.Text;
     
    

    
    }

    

    private void DpiTextBox_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(DpiTextBox.Text, out value))
      {
        if (value > 0) constants.captureDPI = value;
        else MessageBox.Show("Resolution must be a stricly positive integer,\nchange will be ignored.",
                   "Parameter error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

      }
      else
        MessageBox.Show("Invalide resolution value,\nchange will be ignored.",
                     "Parameter error", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
      updateCaptureUI();
    }

    private void widthValue_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(widthValue.Text, out value))
      {
        if ((value >= 16) && (value <=4096))  constants.captureWidth = value;
        else MessageBox.Show("Width must be in the 16..4096 range,\nchange will be ignored.",
                   "Parameter error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

      }
      else
        MessageBox.Show("Invalide width value,\nchange will be ignored.",
                     "Parameter error", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
      updateCaptureUI();
    }

    private void heightValue_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(heightValue.Text, out value))
      {
        if ((value >= 16) && (value <= 4096)) constants.captureHeight = value;
        else MessageBox.Show("Height must be in the 16..4096 range,\nchange will be ignored.",
                   "Parameter error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

      }
      else
        MessageBox.Show("Invalide height value,\nchange will be ignored.",
                     "Parameter error", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
      updateCaptureUI();
    }

    private void MaxDataRecordsCount_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(MaxDataRecordsCount.Text,out value) &&(value>=0))
        
      {
        constants.maxDataRecordsPerSensor = value;
        CustomYSensor.MaxDataRecords = value;
      }
      else MessageBox.Show("Invalid Max Data records,\nshould be a positive integer or Zero.\nChange will be ignored. ",
                           "Parameter error", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

    }

    private void MaxDataPointsCount_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(MaxDataPointsCount.Text, out value)&& (value >= 0))
      {
        constants.maxPointsPerGraphSerie = value;
       DataSerie.MaxPointsPerSeries = value;
      }
      else MessageBox.Show("Invalid Max Data points,\nshould be a positive integer or Zero.\nChange will be ignored. ",
                        "Parameter error", MessageBoxButtons.OK,
                         MessageBoxIcon.Warning);

    }

    private void MemoryTimer_Tick(object sender, EventArgs e)
    {
      if (constants.MonoRunning)
      {
        memoryLabel.Visible = false;
        MemoryTimer.Enabled = false;
      } 
      else memoryLabel.Text = "Available memory : " + performance.NextValue().ToString() +"MB";

    }

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (tabControl1.SelectedIndex == 2)
      {
        MaxDataPointsCount.Text = constants.maxPointsPerGraphSerie.ToString();
        MaxDataRecordsCount.Text = constants.maxDataRecordsPerSensor.ToString();
        maxPointsPerDatalogger.Text = constants.maxPointsPerDataloggerSerie.ToString();
        MemoryTimer_Tick(null, null);
        MemoryTimer.Enabled = true;
      }
      else MemoryTimer.Enabled = false;
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      editButton.Enabled = listView1.SelectedItems.Count > 0;
    
      deleteButton.Enabled = listView1.SelectedItems.Count > 0;
       
    }

   private void findSelectedHub(out int lindex, out int hindex  )
    {
       lindex = -1;
       hindex = -1;
       if (listView1.SelectedItems.Count <= 0) return;

       lindex = listView1.SelectedItems[0].Index;
       
      {
        for (int i = hubList.Count - 1; i >= 0; i--)
        {
          if (hubList[i].get_obfuscatedURL() == listView1.Items[lindex].Text) hindex = i;

        }
      }
    }

    private void editButton_Click(object sender, EventArgs e)
    { 
      int lindex, hindex;
      findSelectedHub(out lindex, out hindex);

      if (hindex>=0)
      {
        Hub h = new Hub(Hub.HubType.REMOTEHUB, hubList[hindex].protocol, hubList[hindex].user, "",
                        hubList[hindex].addr, hubList[hindex].port, hubList[hindex].path);
        h.encryptedPassword = hubList[hindex].encryptedPassword;
        HubEdit editor = new HubEdit();
        if (editor.editHub(ref h))
        {



          YAPI.UnregisterHub(hubList[hindex].get_fullUrl());
          hubList[hindex] = h;
          listView1.Items[lindex].Text = h.get_obfuscatedURL();
          listView1.Items[lindex].SubItems[1].Text="Contacting...";
          listView1.Items[lindex].ImageIndex = 0;
          string errmsg = "";
          YAPI.PreregisterHub(hubList[hindex].get_fullUrl(),ref errmsg);

        }
      }

    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
      int lindex, hindex;
      findSelectedHub(out lindex, out hindex);
      if (hindex >= 0) 
        if (MessageBox.Show("Do you really want to remove "+ hubList[hindex].get_obfuscatedURL()+" ?",
                          "Remove a Hub connection",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                           MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        {
        YAPI.UnregisterHub(hubList[hindex].get_fullUrl());
        hubList.RemoveAt(hindex);
        listView1.Items.RemoveAt(lindex);


      }
    }

    private void label16_Click(object sender, EventArgs e)
    {

    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
     
    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void maxPointsPerDatalogger_Leave(object sender, EventArgs e)
    {
      int value;
      if (Int32.TryParse(maxPointsPerDatalogger.Text, out value))
      {
        constants.maxPointsPerDataloggerSerie = value;
        CustomYSensor.MaxLoggerRecords = value;

      }
      else MessageBox.Show("Invalid Max Data points,\nshould be a integer.\nChange will be ignored. ",
                        "Parameter error", MessageBoxButtons.OK,
                         MessageBoxIcon.Warning);
    }

    private void MaxDataPointsCount_TextChanged(object sender, EventArgs e)
    {

    }

    private void label17_Click(object sender, EventArgs e)
    {

    }

    private void editThisHubConnectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      editButton_Click(sender, e);
    }

    private void ZoomVerticalDragZoom_CheckedChanged(object sender, EventArgs e)
    {
      YGraph.VerticalDragZoomEnabled = VerticalDragZoom.Checked;
    }

    private void dbleClickBringsUpContextMenu_CheckedChanged(object sender, EventArgs e)
    {
      constants.dbleClickBringsUpContextMenu = dbleClickBringsUpContextMenu.Checked;
    }
  }


  public class Hub
  {
    public enum HubType { LOCALUSB, LOCALHUB, REMOTEHUB };


    private HubType _hubType = HubType.LOCALUSB;
    public HubType hubType { get { return _hubType; } set { _hubType = value; } }
    private string _protocol = "";
    public string protocol { get { return _protocol; }  set { _protocol = value; }     }
    private string _user = "";
    public string user { get { return _user; } set { _user = value; } }
    private string _password = "";
    public string encryptedPassword { get { return _password; } set { _password = value; } }
    public string clearPassword     { get { return (_password=="")?"":StringCipher.Decrypt(_password, constants.loginCypherPassword); }
                                      set { _password = StringCipher.Encrypt(value, constants.loginCypherPassword); } }

    private string _addr = "";
    public string addr { get { return _addr; } set { _addr = value; } }
    private string _port = "";
    public string port { get { return _port; } set { _port = value; } }
    private string _path = "";
    public string path { get { return _path; } set { _path = value; } }

   


    public Hub(HubType hubType,string protocol, string user,  string password, string addr, string port, string path)
     {
      _hubType = hubType;
      _protocol = protocol;
      _user = user;
      _port = port;
      _password = password!="" ? StringCipher.Encrypt(password, constants.loginCypherPassword):"";
      _addr = addr;
      _path = path;
    }

    public Hub(HubType hubType)
    {
      if ((hubType  != HubType.LOCALUSB) && (hubType != HubType.LOCALHUB))
       throw new ArgumentException("local types types allowed");
      _hubType = hubType;
    }


    public Hub(XmlNode subnode)
    { _hubType = HubType.REMOTEHUB;
      _protocol = "http";
      if (subnode.Attributes["protocol"] != null) _protocol = subnode.Attributes["protocol"].InnerText;
      _user = "";
      if (subnode.Attributes["user"] != null) _user = subnode.Attributes["user"].InnerText;
      _password = "";
      if (subnode.Attributes["password"] != null) _password = subnode.Attributes["password"].InnerText;
       _port = "";
      if (subnode.Attributes["port"] != null) _port = subnode.Attributes["port"].InnerText;
      _path = "";
      if (subnode.Attributes["path"] != null) _path = subnode.Attributes["path"].InnerText;
      _addr = "";
      if (subnode.Attributes["addr"] != null) _addr = subnode.Attributes["addr"].InnerText;

    }


    public string  get_consoleUrl()
    {
      string fullurl = "http://"+ _addr;
      if (_port != "") fullurl = fullurl + ":" + _port; else fullurl = fullurl + ":4444";
      return fullurl;


    }



    public string get_fullUrl()
    {
        string fullurl = "";
        if (_protocol != "") fullurl = _protocol + "://";
        if (_user != "")
        {
          fullurl = fullurl + _user;
          if (_password != "") fullurl = fullurl + ":" + StringCipher.Decrypt(_password, constants.loginCypherPassword);
          fullurl = fullurl + "@";
        }
        fullurl = fullurl + _addr;
        if (_port != "") fullurl = fullurl + ":" + _port;
        if (_path!="") fullurl = fullurl + "/" + _path;
        return fullurl;
      
    }


    public string get_connexionUrl()
    {
      string fullurl = "";
      if (_protocol != "") fullurl = _protocol + "://"; else fullurl = "ws://";     
      fullurl = fullurl + _addr;
      if (_port != "") fullurl = fullurl + ":" + _port; else fullurl = fullurl + ":4444";
      fullurl = fullurl + "/";
      return fullurl.ToLower();

    }


    public string  get_obfuscatedURL()
    {
      string fullurl = "";
      if (_protocol != "") fullurl = _protocol + "://";
      if (_user != "")
      {
        fullurl = fullurl + _user;
        if (_password != "") fullurl = fullurl + ":#####";
        fullurl = fullurl + "@";
      }
      fullurl = fullurl + _addr;
      if (_port != "") fullurl = fullurl + ":" + _port;
      if (_path != "") fullurl = fullurl + "/" + _path;
      return fullurl;

    }


    public String XmlCode()
    {
      string NodeLine = "<Hub ";
      if (_protocol != "") NodeLine = NodeLine + "protocol=\"" + System.Security.SecurityElement.Escape(_protocol) + "\" ";
      if (_user != "") NodeLine = NodeLine + "user=\"" + System.Security.SecurityElement.Escape(_user) + "\" ";
      if (_password != "") NodeLine = NodeLine + "password=\"" + System.Security.SecurityElement.Escape(_password) + "\" ";
      NodeLine = NodeLine + "addr=\"" + System.Security.SecurityElement.Escape(_addr) + "\" ";
      if (_port != "") NodeLine = NodeLine + "port=\"" + System.Security.SecurityElement.Escape(_port) + "\" ";
      if (_path != "") NodeLine = NodeLine + "path=\"" + System.Security.SecurityElement.Escape(_path) + "\" ";
      return NodeLine + "/>";
    }


  }
  
  public static class StringCipher  // thanks CraigTP @ stackoverflow
  {
    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    private static byte[] Generate256BitsOfRandomEntropy()
    {
      var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
      RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
      
        // Fill the array with cryptographically secure random bytes.
        rngCsp.GetBytes(randomBytes);
      
      return randomBytes;
    }


    public static string Encrypt(string plainText, string passPhrase)
    {
      // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
      // so that the same Salt and IV values can be used when decrypting.  
      var saltStringBytes = Generate256BitsOfRandomEntropy();
      var ivStringBytes = Generate256BitsOfRandomEntropy();
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations);
      
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream())
            {
              using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
              {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                var cipherTextBytes = saltStringBytes;
                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
              }
            }
          
        }
      }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
      // Get the complete stream of bytes that represent:
      // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
      var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
      // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
      var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
      // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
      var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
      // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
      var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

      Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations);
      
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
              using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
              {
                var plainTextBytes = new byte[cipherTextBytes.Length];
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
              }
            }
          }
        
      }
    }


  }

}
