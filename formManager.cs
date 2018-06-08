
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  widgets forms generic management
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
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using YoctoVisualization.Properties;
using YColors;
using YDataRendering;

namespace YoctoVisualisation
{



  class formManager
  {
    Form myForm;
    private XmlNode initDataNode;

    private string formDesc;
    private StartForm mainForm;
    private GenericProperties settings;
    private bool Closing = false;
   

    public void AjustHint(string description)
    {
     
    }


    public formManager(Form form, StartForm main, XmlNode initnode, string description, GenericProperties formsettings)
    {
      formDesc = description;
      mainForm = main;
      initDataNode = initnode;
      settings = formsettings;
      myForm = form;
      form.FormClosing += targetFormClosing;
      form.Shown += target_Shown;
   
      
    

    }

    public void proportionalValuechanged(Proportional source)
    {
      Object parent = source.directParent;
      if (parent is YFontParams)
      {
        object userData = ((YFontParams)parent).userData;
        if (userData != null)
        {
          double newvalue = (Math.Round(source.value * 1000)) / 1000;
          if (((FontDescription)userData).size != newvalue)
          {
            ((FontDescription)userData).size = newvalue;
            mainForm.refreshPropertiesForm();
          }
        }
      }
    }

    private void target_Shown(object sender, EventArgs e)
    {


    }




    public void configureContextMenu(Form parentForm, ContextMenuStrip menu, EventHandler ShowConfig, EventHandler SwitchConfig, EventHandler Capture)
    {
      menu.Items.Add(new ToolStripMenuItem("Configure this " + this.formDesc, Resources.menu_configure_item, ShowConfig));
      menu.Items.Add(new ToolStripMenuItem("Delete this " + this.formDesc, Resources.deleted, deleteCurrentForm));
      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add(new ToolStripMenuItem("Add a new solid gauge", Resources.menu_add_solidgauge, AddNewSolidGauge));
      menu.Items.Add(new ToolStripMenuItem("Add a new angular gauge", Resources.rmenu_add_gauge, AddNewAngularGauge));
      menu.Items.Add(new ToolStripMenuItem("Add a new digital display", Resources.menu_add_digital, AddNewDigitalDisplay));
      menu.Items.Add(new ToolStripMenuItem("Add a new graph", Resources.menu_add_graph, AddNewGraph));
      menu.Items.Add(new ToolStripMenuItem("Show Raw data", Resources.menu_rawdata, ShowRawData));
      menu.Items.Add(new ToolStripMenuItem("Snapshot", Resources.snapshot, Capture));

      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add(new ToolStripMenuItem("Global configuration ", Resources.menu_configure, ConfigureUSBNet));
      menu.Items.Add(new ToolStripMenuItem("Show logs", Resources.menu_logs, showlogs));

      menu.Items.Add(new ToolStripMenuItem("Close the whole application", Resources.exit, ExitTheApplication));






    }

    private void showlogs(object sender, EventArgs e)
    { LogManager.Show(); }

    private void ConfigureUSBNet(object sender, EventArgs e)
    { mainForm.ConfigureUSBNet(); }

    private void ShowRawData(object sender, EventArgs e)
    { mainForm.ShowRawData(); }

    private void AddNewSolidGauge(object sender, EventArgs e)
    {

      mainForm.NewSolidGaugeForm(null);
    }


    private void AddNewAngularGauge(object sender, EventArgs e)
    {

      mainForm.NewAngularGaugeForm(null);
    }


    private void AddNewDigitalDisplay(object sender, EventArgs e)
    {

      mainForm.NewDigitalDisplayForm(null);
    }

    private void AddNewGraph(object sender, EventArgs e)
    { mainForm.NewGraphForm(null); }


    private void targetFormClosing(object sender, FormClosingEventArgs e)
    {
      formClosing(e);
    }


    private void ExitTheApplication(object sender, EventArgs e)
    {
      mainForm.Terminate();
    }

    private void deleteCurrentForm(object sender, EventArgs e)
    {
      mainForm.hidePropertyEditor();
      deleteForm(true);
    }

    public string getConfigData()
    {
      return "  <location x='" + myForm.Location.X.ToString() + "' y='" + myForm.Location.Y.ToString() + "'/>\n"
            +"  <size     w='" + myForm.Size.Width.ToString() + "' h='" + myForm.Size.Height.ToString() + "'/>\n"
            + settings.getXml(1);
    }

    public void setData(System.Reflection.PropertyInfo info, object target, string propertyName, string value)
    {
      switch (info.PropertyType.ToString())
      {
        case "System.Drawing.Color": info.SetValue(target, YColor.fromString(value).toColor(),null); break;
        default: info.SetValue(target, value,null); break;
      }
    }

    public bool initForm()
    {
      if (initDataNode != null)
      {
        StartForm.RestoreWindowPosition(myForm, initDataNode);


        foreach (XmlNode node in initDataNode.ChildNodes)
        {


          switch (node.Name)
          {
            default:
              if ((node.Name.Length >= 5) && (node.Name.Substring(0, 5) == "Form_"))
              {
                string propname = node.Name.Substring(5);
                string value = node.Attributes["value"].InnerText;
                System.Reflection.PropertyInfo info = myForm.GetType().GetProperty(propname);
                setData(info, myForm, propname, value);

              }
              break;
          }


        }
        return true;
      }
      return false;
    }

    public bool deleteForm(bool explicitCall)
    {



      if (MessageBox.Show("You really want to delete this " + formDesc + " ?", "deleting " + formDesc,
         MessageBoxButtons.YesNo, MessageBoxIcon.Question,
         MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
      {
        Closing = true;
        mainForm.removeForm(myForm);
        myForm.Close();
        return true;
      }
      return false;

    }



    public void formClosing(FormClosingEventArgs e)
    {

      if ((e.CloseReason == CloseReason.UserClosing) && (!mainForm.isClosing) && (!Closing))
      {
        mainForm.Terminate();
        e.Cancel = true;
      }
    }

  }
}
