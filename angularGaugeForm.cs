
/*
 *  Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Angular gauge widget
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
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using LiveCharts.Wpf;

namespace YoctoVisualisation
{
  public partial class angularGaugeForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    private AngularGaugeFormProperties prop;
    private int sectionCount = 0;

    public angularGaugeForm(StartForm parent, XmlNode initData)
    {
      // dirty way to find out how many axis  can be defined;    
      foreach (var p in typeof(AngularGaugeFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("AngularGauge_Sections")) sectionCount++;

      }


      InitializeComponent();
      prop = new AngularGaugeFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "gauge", prop);
      mainForm = parent;
      initDataNode = initData;

      for (int i = 0; i < sectionCount; i++)
        angularGauge1.Sections.Add(new AngularSection());
      prop.ApplyAllProperties(this);
      prop.ApplyAllProperties(angularGauge1);

      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration);


    }

    private void angularGaugeForm_Load(object sender, EventArgs e)
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
        case "AngularGauge":
          GenericProperties.newSetProperty(angularGauge1, prop, fullpropname, path);
          string SectionPrefix = "AngularGauge_Sections";
          if ((fullpropname.StartsWith(SectionPrefix)) && (OriginalPropName == "Fill"))
          {
            int arrayIndex = int.Parse(fullpropname.Substring(SectionPrefix.Length));
            double v = angularGauge1.Sections[arrayIndex].FromValue;
            angularGauge1.Sections[arrayIndex].FromValue = v + 1;
            angularGauge1.Sections[arrayIndex].FromValue = v; // force a redraw with new color

          }



          break;
        case "DataSource":
          manager.AjustHint("");
          break;
      }

    }

    public string getConfigData()
    {
      return "<angularGaugeForm>\n"
            + manager.getConfigData()
            + "</angularGaugeForm>\n";
    }

    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }



    public void SourceChanged(CustomYSensor value)
    {
      if (value is NullYSensor)
      {
        angularGauge1.Value = 0;
        label1.Text = "N/A";
        label1.Visible = true;
        return;
      }

      if (!value.isOnline())

      {
        angularGauge1.Value = 0;
        label1.Text = "OFFLINE";
        label1.Visible = true;
      }
      else label1.Visible = false;

      value.registerCallback(this);

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;



      if (source == prop.DataSource_source)
      {
        if (prop.DataSource_source is NullYSensor)
        {
          label1.Text = "N/A";
          label1.Visible = true;
          angularGauge1.Value = 0;
          return;
        }
        else
        if (!prop.DataSource_source.isOnline())
        {
          angularGauge1.Value = 0;
          label1.Text = "OFFLINE";
          label1.Visible = true;
          return;
        }
        label1.Visible = false;
        angularGauge1.Value = source.isOnline() ? M.get_averageValue() : 0;

      }
    }

    private void gaugeForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {

    }


  }
}
