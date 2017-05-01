/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Solid gauge widget
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace YoctoVisualisation
{



  public partial class gaugeForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    private GaugeFormProperties prop;

    public string valueformatter(double value)
    {
      if (prop.DataSource_source is NullYSensor) return "N/A";


      if (!prop.DataSource_source.isOnline())
        return "OFFLINE";

      string format = prop.DataSource_precision;
      int p = format.IndexOf('.');
      int n = 0;
      if (p >= 0) n = format.Length - p - 1;
      string unit = prop.DataSource_source.get_unit();


      return value.ToString("F" + n.ToString()) + unit;
    }

    public gaugeForm(StartForm parent, XmlNode initData)
    {
      InitializeComponent();
      prop = new GaugeFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "gauge", prop);
      mainForm = parent;
      initDataNode = initData;
      prop.ApplyAllProperties(this);
      prop.ApplyAllProperties(solidGauge1);
      solidGauge1.LabelFormatter = value => valueformatter(value);
      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration);

    }

    private void gaugeForm_Load(object sender, EventArgs e)
    {



      manager.initForm();
    }


    void PropertyChanged(PropertyValueChangedEventArgs e)
    {
      string fullpropname = "";
      string propType = "";
      string OriginalPropName = "";

      GridItem p = e.ChangedItem;
      List<string> path = GenericProperties.ExtractPropPath(p, ref OriginalPropName, ref fullpropname, ref propType);


      switch (propType)
      {
        case "Form":
          GenericProperties.newSetProperty(this, prop, fullpropname, path);
          break;
        case "SolidGauge":
          GenericProperties.newSetProperty(solidGauge1, prop, fullpropname, path);
          break;
        case "DataSource":
          manager.AjustHint("");
          break;
      }

    }


    public string getConfigData()
    {
      return "<GaugeForm>\n"
            + manager.getConfigData()
            + "</GaugeForm>\n";
    }


    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    { //MessageBox.Show("pouet");
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }




    public void SourceChanged(CustomYSensor value)
    {
      if (value is NullYSensor) solidGauge1.Value = 0;
      else value.registerCallback(this);

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      if (source == prop.DataSource_source)
        solidGauge1.Value = source.isOnline() ? M.get_averageValue() : 0;
    }

    private void gaugeForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }

    private void gaugeForm_Activated(object sender, EventArgs e)
    {

    }
  }




}
