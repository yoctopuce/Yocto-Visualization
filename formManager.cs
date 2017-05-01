
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  widgets forms generic management
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using YoctoVisualisation.Properties;

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
      int panelWidth = 375;
      int panelHeight = 100;

      Panel panel = (Panel)myForm.Controls.Find("HintPanel", true)[0];
      Label label = (Label)myForm.Controls.Find("HintLabel", true)[0];

      if (description != "")
      {
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Text = "No Data source configured.\n 1 - Make sure you have a Yoctopuce sensor connected\n 2 - Do a right click on this window\n 3 - Choose \"configure this " + description + "\" to bring up the properties editor\n 4 - Choose a data source.";
        panel.Size = new Size(panelWidth, panelHeight);

      }

      panel.Location = new Point((myForm.ClientSize.Width - panelWidth) >> 1, (myForm.ClientSize.Height - panelHeight) >> 1);

      panel.Visible = !settings.IsDataSourceAssigned();

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
      AjustHint(description);
      form.SizeChanged += target_FormSizeChanged;


    }

    private void target_FormSizeChanged(object sender, EventArgs e)
    {
      AjustHint("");

    }

    private void target_Shown(object sender, EventArgs e)
    {


    }




    public void configureContextMenu(Form parentForm, ContextMenuStrip menu, EventHandler ShowConfig, EventHandler SwitchConfig)
    {
      menu.Items.Add(new ToolStripMenuItem("Configure this " + this.formDesc, Resources.menu_configure_item, ShowConfig));
      menu.Items.Add(new ToolStripMenuItem("Delete this " + this.formDesc, Resources.deleted, deleteCurrentForm));
      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add(new ToolStripMenuItem("Add a new solid gauge", Resources.menu_add_solidgauge, AddNewSolidGauge));
      menu.Items.Add(new ToolStripMenuItem("Add a new angular gauge", Resources.rmenu_add_gauge, AddNewAngularGauge));
      menu.Items.Add(new ToolStripMenuItem("Add a new digital display", Resources.menu_add_digital, AddNewDigitalDisplay));
      menu.Items.Add(new ToolStripMenuItem("Add a new graph ", Resources.menu_add_graph, AddNewGraph));
      menu.Items.Add(new ToolStripMenuItem("Show Raw data ", Resources.menu_rawdata, ShowRawData));


      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add(new ToolStripMenuItem("Configure USB / Network ", Resources.menu_configure, ConfigureUSBNet));
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
      return "<location x='" + myForm.Location.X.ToString() + "' y='" + myForm.Location.Y.ToString() + "'/>\n"
            + "<size     w='" + myForm.Size.Width.ToString() + "' h='" + myForm.Size.Height.ToString() + "'/>\n"
            + settings.getXml();
    }

    public void setData(System.Reflection.PropertyInfo info, object target, string propertyName, string value)
    {
      switch (info.PropertyType.ToString())
      {
        case "System.Drawing.Color": info.SetValue(target, (Color)ColorTranslator.FromHtml(value)); break;
        default: info.SetValue(target, value); break;
      }
    }

    public void initForm()
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

      }
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
