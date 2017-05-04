/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Digital display widget
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

namespace YoctoVisualisation
{
  public partial class digitalDisplayForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    private digitalDisplayFormProperties prop;




    public digitalDisplayForm(StartForm parent, XmlNode initData)
    {
      InitializeComponent();
      prop = new digitalDisplayFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "digital display", prop);
      mainForm = parent;
      initDataNode = initData;
      prop.ApplyAllProperties(this);
      prop.ApplyAllProperties(label1);

      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration);
    }

    private void digitalDisplayForm_Load(object sender, EventArgs e)
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
        case "Label":
          GenericProperties.newSetProperty(label1, prop, fullpropname, path);
          break;
        case "DataSource":
          manager.AjustHint("");
          break;
      }



    }
    public string getConfigData()
    {
      return "<digitalDisplayForm>\n"
            + manager.getConfigData()
            + "</digitalDisplayForm>\n";
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
      SensorValuecallback(value, null);

      if (value is NullYSensor) label1.Text = "N/A";
      else if (!value.isOnline()) label1.Text = "OFFLINE";

      value.registerCallback(this);

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      if (source == prop.DataSource_source)
      {
        Color c = prop.Label_ForeColor;
        string res = "";
        if (prop.DataSource_source is NullYSensor) res = "N/A";
        else if (!prop.DataSource_source.isOnline()) res = "OFFLINE";
        else if (M == null)
        {
          string unit = prop.DataSource_source.get_unit();
          res = "--" + unit;
        }
        else
        {
          string format = prop.DataSource_precision;
          int p = format.IndexOf('.');
          int n = 0;
          if (p >= 0) n = format.Length - p - 1;
          string unit = prop.DataSource_source.get_unit();
          double v = M.get_averageValue();

          if ((prop.Label_MinValue.value != Double.NaN) && (v < prop.Label_MinValue.value)) c = prop.Label_OORColor;
          else if ((prop.Label_MaxValue.value != Double.NaN) && (v > prop.Label_MaxValue.value)) c = prop.Label_OORColor;


          res = v.ToString("F" + n.ToString()) + unit;
        }
        label1.Text = res;
        label1.ForeColor = c;

      }

    }

    private void digitalDisplayForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }
  }


}

