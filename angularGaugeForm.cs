
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
using YDataRendering;


namespace YoctoVisualisation
{
  public partial class angularGaugeForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    private AngularGaugeFormProperties prop;
  
    private int zonesCount = 0;
    private YAngularGauge _angularGauge;

    private MessagePanel  noDataSourcepanel;

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public YAngularGauge angularGauge { get { return _angularGauge; } } 



    public angularGaugeForm(StartForm parent, XmlNode initData)
    {
      zonesCount = 0;
      foreach (var p in typeof(AngularGaugeFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("AngularGauge_zone")) zonesCount++;

      }

      InitializeComponent();
      _angularGauge = new YAngularGauge(rendererCanvas, LogManager.Log);
     
      _angularGauge.DisableRedraw();
      _angularGauge.resizeRule = Proportional.ResizeRule.FIXED;
      _angularGauge.getCaptureParameters = constants.getCaptureParametersCallback;



      noDataSourcepanel = _angularGauge.addMessagePanel();
    
      noDataSourcepanel.text = "No data source configured\n"
              + " 1 - Make sure you have a Yoctopuce sensor connected.\n"
              + " 2 - Do a right-click on this window.\n"
              + " 3 - Choose \"Configure this gauge\" to bring up the properties editor.\n"
              + " 4 - Choose a data source\n";
     
      for (int i = 0; i < zonesCount; i++) _angularGauge.AddZone();
      prop = new AngularGaugeFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "gauge", prop);
      mainForm = parent;
      initDataNode = initData;   
      prop.ApplyAllProperties(this);
      if (!manager.initForm()) {
        Rectangle s = Screen.FromControl(this).Bounds;
        this.Location = new Point((s.Width - this.Width) >> 1, (s.Height - this.Height) >> 1);
      }

      prop.ApplyAllProperties(_angularGauge);
      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration, capture);
 
    
      _angularGauge.AllowPrintScreenCapture = true;
      _angularGauge.proportionnalValueChangeCallback = manager.proportionalValuechanged ;
      _angularGauge.resizeRule = Proportional.ResizeRule.RELATIVETOBOTH;
      _angularGauge.AllowRedraw();


    }

    private void capture(object sender, EventArgs e)
    {
      _angularGauge.capture();
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
     
           GenericProperties.newSetProperty(_angularGauge, prop, fullpropname, path);

     

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

    private void showStatus(string status)
    {
      if (status!="") _angularGauge.value = 0;
      _angularGauge.showNeedle = status=="";
      _angularGauge.statusLine = status;


    }

    public void SourceChanged(CustomYSensor value)
    {
      _angularGauge.DisableRedraw();
      noDataSourcepanel.enabled = (value is NullYSensor);
      if (value is NullYSensor) { showStatus("N/A"); _angularGauge.unit = ""; }  
      else if (!value.isOnline()) showStatus("OFFLINE");
      else { showStatus("");  _angularGauge.unit = value.get_unit(); }
      _angularGauge.AllowRedraw();
      value.registerCallback(this);

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;



      if (source == prop.DataSource_source)
      {
        _angularGauge.DisableRedraw();
        if (prop.DataSource_source is NullYSensor) {  showStatus("N/A"); _angularGauge.unit = ""; }
        else if (!prop.DataSource_source.isOnline()) showStatus("OFFLINE");
        else
        {
          _angularGauge.DisableRedraw();
          showStatus("");
          source.get_unit();  // make sure unit is in cache before ui is refresh (redraw might call value formater, which  will call get_unit) 
          _angularGauge.unit = source.get_unit();
          _angularGauge.value =  M.get_averageValue();
          _angularGauge.AllowRedraw();
        }
        _angularGauge.AllowRedraw();
      }
    }

    private void gaugeForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }

  

    private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
    {
   
    }

    private void angularGaugeForm_LocationChanged(object sender, EventArgs e)
    {
     
    }

    private void angularGaugeForm_Load_1(object sender, EventArgs e)
    {
     // manager.initForm();
    }
  }
}
