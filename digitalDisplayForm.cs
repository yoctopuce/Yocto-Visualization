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
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using YDataRendering;

namespace YoctoVisualisation
{
  public partial class digitalDisplayForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    private digitalDisplayFormProperties prop;
    private YDigitalDisplay _display;
    private MessagePanel noDataSourcepanel;
    string unit = "";
    public YDigitalDisplay display { get { return _display; } }


    public digitalDisplayForm(StartForm parent, XmlNode initData)
    {
      InitializeComponent();
      _display = new YDigitalDisplay(rendererCanvas, LogManager.Log);
      _display.getCaptureParameters = constants.getCaptureParametersCallback;
      _display.DisableRedraw();
      _display.valueFormater = valueFormater;
      noDataSourcepanel = _display.addMessagePanel();
      noDataSourcepanel.text = "No data source configured\n"
            + " 1 - Make sure you have a Yoctopuce sensor connected.\n"
            + " 2 - Do a right-click on this window.\n"
            + " 3 - Choose \"Configure this digital display\" to bring up the properties editor.\n"
            + " 4 - Choose a data source\n";
     

      prop = new digitalDisplayFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "digital display", prop);
      mainForm = parent;
      initDataNode = initData;
      prop.ApplyAllProperties(this);
      if (!manager.initForm())
      {
        Rectangle s = Screen.FromControl(this).Bounds;
        this.Location = new Point((s.Width - this.Width) >> 1, (s.Height - this.Height) >> 1);
      }

      prop.ApplyAllProperties(_display);

      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration, capture);
    
      _display.AllowPrintScreenCapture = true;
      _display.proportionnalValueChangeCallback = manager.proportionalValuechanged;
      _display.resizeRule = Proportional.ResizeRule.RELATIVETOBOTH;
      _display.AllowRedraw();
    }

    private void capture(object sender, EventArgs e)
    {
      _display.capture();
    }


    private void digitalDisplayForm_Load(object sender, EventArgs e)
    {
     
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
        case "display":
          GenericProperties.newSetProperty(_display, prop, fullpropname, path);
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
      noDataSourcepanel.enabled = (value is NullYSensor);
      SensorValuecallback(value, null);

      if (value is NullYSensor) _display.alternateValue = "N/A";
      else if (!value.isOnline()) _display.alternateValue = "OFFLINE";

      value.registerCallback(this);

    }

    public string  valueFormater(YDataRenderer source, double value)
    {
      if (prop.DataSource_source is NullYSensor) return "N/A";
      else if (!prop.DataSource_source.isOnline()) return  "OFFLINE";

      string format = prop.DataSource_precision;
      int p = format.IndexOf('.');
      int n = 0;
      if (p >= 0) n = format.Length - p - 1;
      unit = prop.DataSource_source.get_unit();
      return value.ToString("F" + n.ToString()) + unit;

    }


    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      if (prop.DataSource_source is NullYSensor) _display.alternateValue  = "N/A";
      else if (!prop.DataSource_source.isOnline()) _display.alternateValue  = "OFFLINE";
      else if (M == null)
      {
       
        _display.alternateValue = "--" + unit;
      }
      else  if (source == prop.DataSource_source)
      {
        _display.DisableRedraw();
        source.get_unit();  // make sure unit is in cache before ui is refresh (redraw might call value formater, which  will call get_unit) 
        _display.alternateValue = null;
         _display.value =  M.get_averageValue();
        _display.AllowRedraw();
      }
  
    }

    private void digitalDisplayForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }
  }


}

