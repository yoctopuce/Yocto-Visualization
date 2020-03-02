
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

using System.Xml;
using System.Windows.Forms;
using YDataRendering;
using System.Reflection;

namespace YoctoVisualisation
{
  public partial class angularGaugeForm : Form
  {
    private XmlNode initDataNode;
    private static int AnnotationPanelCount = 0;
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

      if (AnnotationPanelCount == 0)
        foreach (var p in typeof(AngularGaugeFormProperties).GetProperties())
        {
          string name = p.Name;
          if (name.StartsWith("AngularGauge_annotationPanel")) AnnotationPanelCount++;
        }
      for (int i = 0; i < AnnotationPanelCount; i++)
      { _angularGauge.addAnnotationPanel(); }

      _angularGauge.setPatchAnnotationCallback(AnnotationCallback);


      initDataNode = initData;   
      prop.ApplyAllProperties(this);
      if (!manager.initForm()) {
        Rectangle s = Screen.FromControl(this).Bounds;
        this.Location = new Point((s.Width - this.Width) >> 1, (s.Height - this.Height) >> 1);
      }

      YDataRenderer.minMaxCheckDisabled = true;
      try { prop.ApplyAllProperties(_angularGauge); }
      catch (TargetInvocationException e) { LogManager.Log("AngularGauge initialization raised an exception (" + e.InnerException.Message + ")"); }
      YDataRenderer.minMaxCheckDisabled = false;

      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration, capture);

      _angularGauge.resetRefrenceSize();
      _angularGauge.AllowPrintScreenCapture = true;
      _angularGauge.proportionnalValueChangeCallback = manager.proportionalValuechanged ;
      _angularGauge.resizeRule = Proportional.ResizeRule.RELATIVETOBOTH;
      _angularGauge.AllowRedraw();

      _angularGauge.OnDblClick += rendererCanvas_DoubleClick;
      if (constants.OSX_Running)
      {
        _angularGauge.OnRightClick += rendererCanvas_RightClick;
      }
    }


    private string AnnotationCallback(string text)
    {

      CustomYSensor sensor = prop.DataSource_source;
      string name = "None";
      string avgvalue = "N/A";
      string minvalue = "N/A";
      string maxvalue = "N/A";
      string unit = "";
      if (!(sensor is NullYSensor))
      {
        string resolution = sensor.get_resolution().ToString().Replace("1", "0");
        name = sensor.get_friendlyName();
        if (sensor.isOnline())
        {
          avgvalue = sensor.get_lastAvgValue().ToString(resolution);
          minvalue = sensor.get_lastMinValue().ToString(resolution);
          maxvalue = sensor.get_lastMaxValue().ToString(resolution);
        }
        unit = sensor.get_unit();
      }
      text = text.Replace("$NAME$", name);
      text = text.Replace("$AVGVALUE$", avgvalue);
      text = text.Replace("$MAXVALUE$", maxvalue);
      text = text.Replace("$MINVALUE$", minvalue);
      text = text.Replace("$UNIT$", unit);



      return text;
    }

    private void rendererCanvas_DoubleClick(object sender, EventArgs e)
    {
      if (!constants.dbleClickBringsUpContextMenu) return;
      MouseEventArgs m = (MouseEventArgs)e;
      ContextMenuStrip.Show(this, new Point(m.X, m.Y));

    }

    private void rendererCanvas_RightClick(object sender, EventArgs e)
    {
      MouseEventArgs m = (MouseEventArgs)e;
      ContextMenuStrip.Show(this, new Point(m.X, m.Y));
    }

    private void capture(object sender, EventArgs e)
    {
      _angularGauge.capture();
    }


    private void angularGaugeForm_Load(object sender, EventArgs e)
    {
      manager.initForm();
    }







    void PropertyChanged2(UIElement src)
    {
      string fullpropname = "";
      string propType = "";
      string OriginalPropName = "";

    
      List<string> path = src.ExtractPropPath( ref OriginalPropName, ref fullpropname, ref propType);



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
      double newCoef = 0;
      if (WindowState == FormWindowState.Maximized)
      { // if the window is maximized, saved size will be regular size, and maximize
        //  flag will be stored separately , so we need to save font
        //  size relative to  regular size (i.e. RestoreBounds.Size);

        int deltaX = Size.Width - _angularGauge.usableUiWidth();
        int deltaY = Size.Height - _angularGauge.usableUiHeight();
        newCoef = Proportional.resizeCoef(Proportional.ResizeRule.RELATIVETOBOTH, _angularGauge.refWidth, _angularGauge.refHeight, RestoreBounds.Size.Width - deltaX, RestoreBounds.Size.Height - deltaX);
        _angularGauge.resetProportionalSizeObjectsCachePush(newCoef);  // reset all size related cached objects    
      }
      string dt = manager.getConfigData();
      if (newCoef != 0) _angularGauge.resetProportionalSizeObjectsCachePop();
      return "<angularGaugeForm>\n"
            + dt
            + "</angularGaugeForm>\n";
    }

    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, false);
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
          source.get_unit();  // make sure unit is in cache before UI is refresh (redraw might call value formatter, which  will call get_unit) 
          _angularGauge.unit = source.get_unit();
          _angularGauge.value =  M.get_averageValue();
          _angularGauge.AllowRedraw();
        }
        _angularGauge.AllowRedraw();
      }
    }

    private void gaugeForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, false);
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

    private void angularGaugeForm_Deactivate(object sender, EventArgs e)
    {

     
        mainForm.widgetLostFocus(this);
      

    }
  }
}
