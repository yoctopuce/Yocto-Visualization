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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace YoctoVisualisation
{
  public partial class ConfigForm : Form
  {
    string VirtualHubSerial = "";
    string localIP = GetLocalIPAddress();
    bool initdone = false;

    public ConfigForm()
    {
      InitializeComponent();
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
      string addr = newEntry.Text.Trim();
      if (addr != "")
      {
        if (addr.ToUpper() == "USB") { MessageBox.Show("USB  is already defined"); return; }
        if (addr.ToUpper() == "127.0.0.1") { MessageBox.Show("Local VirtualHub (127.0.0.1) is already defined"); return; }



        for (int i = 0; i < listView1.Items.Count; i++)
          if (listView1.Items[i].Text == addr)
          {
            MessageBox.Show(addr + " is already defined");
            return;
          }
        AddHub(addr);
        newEntry.Text = "";
      }
    }





    private void RemoveHub(string addr)
    {
      if (addr == "usb")
      {
        LogManager.Log("unRegistering  USB ");
        YAPI.UnregisterHub("usb");
        usbFailed.Visible = false;
        usbOk.Visible = false;
        return;
      }
      if (addr == "127.0.0.1")
      {
        LogManager.Log("unRegistering  Local VirtualHub (127.0.0.1) ");
        YAPI.UnregisterHub("127.0.0.1");
        hubFailed.Visible = false;
        hubOk.Visible = false;
        hubWaiting.Visible = false;
        return;
      }
      LogManager.Log("unRegistering " + addr);
      YAPI.UnregisterHub(addr);
      for (int i = listView1.Items.Count - 1; i >= 0; i--)
        if (listView1.Items[i].Text == addr)
          listView1.Items.RemoveAt(i);

    }

    private void AddHub(string addr)
    {
      string errmsg = "";
      if (addr == "usb")
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

      if (addr == "127.0.0.1")
      {
        LogManager.Log("preregistering  local VirtualHub (127.0.0.1)" + addr);
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

      ListViewItem item = listView1.Items.Add(addr);
      item.SubItems.Add("Contacting...");
      item.SubItems.Add("");
      item.ImageIndex = 0;
      LogManager.Log("preregistering  " + addr);
      if (YAPI.PreregisterHub(addr, ref errmsg) != YAPI.SUCCESS)
      {
        LogManager.Log("[!] preregistering  " + addr + " failed (" + errmsg + ")");
        item.SubItems.Add(errmsg);
        item.ImageIndex = 1;
        return;
      }




    }

    public string GetXMLConfiguration()
    {
      string res = "<Config>"
         + "<UseUSB value=\"" + (useUSB.Checked ? "TRUE" : "FALSE") + "\"/>\n"
         + "<UseVirtualHub value=\"" + (UseVirtualHub.Checked ? "TRUE" : "FALSE") + "\"/>\n"
         + "<Hubs>\n";
      for (int i = 0; i < listView1.Items.Count; i++)
        res += "<Hub addr=\"" + System.Security.SecurityElement.Escape(listView1.Items[i].Text) + "\"/>\n";

      res += "</Hubs>\n"
          + "</Config>\n";
      return res;
    }

    public void DefaultInit()
    {
      useUSB.Checked = true;
      AddHub("usb");
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
            if (useUSB.Checked) AddHub("usb");


            break;
          case "UseVirtualHub":
            UseVirtualHub.Checked = (node.Attributes["value"].InnerText.ToUpper() == "TRUE");
            if (UseVirtualHub.Checked) AddHub("127.0.0.1");
            break;
          case "Hubs":
            foreach (XmlNode subnode in node.ChildNodes)
              if (subnode.Name == "Hub")
              {
                string addr = subnode.Attributes["addr"].InnerText;

                AddHub(addr);
              }
            break;
        }

      }
      UseVirtualHub.CheckedChanged += UseVirtualHub_CheckedChanged;
      useUSB.CheckedChanged += useUSB_CheckedChanged;
    }

    public void NetworkArrival(YNetwork net)
    {
      string ip = net.get_ipAddress();
      string netname = net.get_logicalName();
      string loginame = net.get_module().get_logicalName();
      if ((ip == "127.0.0.1") || (ip == localIP) || (ip == "0.0.0.0"))
      {
        hubFailed.Visible = false;
        hubOk.Visible = true;
        hubWaiting.Visible = false;
        VirtualHubSerial = net.get_module().get_serialNumber();
        return;
      }
      for (int i = 0; i < listView1.Items.Count; i++)
      {
        if ((listView1.Items[i].Text == ip) || (listView1.Items[i].Text == netname))
        {
          listView1.Items[i].ImageIndex = 2;
          listView1.Items[i].SubItems[1].Text = (loginame != "" ? loginame : netname) + " OK";
          listView1.Items[i].SubItems[2].Text = net.get_module().get_serialNumber();
        }
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
      for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
      {
        string addr = listView1.SelectedItems[i].Text;
        listView1.Items.Remove(listView1.SelectedItems[i]);
      }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {

    }

    private void UseVirtualHub_CheckedChanged(object sender, EventArgs e)
    {
      if (UseVirtualHub.Checked) AddHub("127.0.0.1"); else RemoveHub("127.0.0.1");
    }

    private void useUSB_CheckedChanged(object sender, EventArgs e)
    {
      if (useUSB.Checked) AddHub("usb"); else RemoveHub("usb");
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
      if (listView1.SelectedItems.Count > 0)

        for (int i = 0; i < listView1.SelectedItems.Count; i++)
        {
          string addr = listView1.SelectedItems[i].Text;
          System.Diagnostics.Process.Start("http://" + addr + ":4444");
        }
    }
  }
}
