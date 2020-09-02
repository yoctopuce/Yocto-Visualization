﻿/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Solid gauge widget
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
using System.Collections.Generic;

using System.Xml;
using System.Windows.Forms;
using YDataRendering;
using System.Drawing;
using System.Reflection;

namespace YoctoVisualisation
{



  public partial class gaugeForm : Form
  {
    private XmlNode initDataNode;
    private static int AnnotationPanelCount = 0;
    private StartForm mainForm;
    private formManager manager;
    private GaugeFormProperties prop;
    private YSolidGauge _solidGauge;
    private MessagePanel noDataSourcepanel;
    private YSolidGauge solidGauge { get { return _solidGauge; } }

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public string valueformatter(YDataRenderer source,double value)
    {


 
      if (prop.DataSource_source is NullYSensor) return "N/A";
      if (!prop.DataSource_source.isOnline())     return "OFFLINE";

    

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
      _solidGauge = new YSolidGauge(rendererCanvas, YSolidGauge.DisplayMode.DISPLAY180, LogManager.Log);
      _solidGauge.getCaptureParameters = constants.getCaptureParametersCallback;
      _solidGauge.DisableRedraw();
      _solidGauge.valueFormater = valueformatter;
      noDataSourcepanel = _solidGauge.addMessagePanel();

      noDataSourcepanel.text = "No data source configured\n"
              + " 1 - Make sure you have a Yoctopuce sensor connected.\n"
              + " 2 - Do a right-click on this window.\n"
              + " 3 - Choose \"Configure this gauge\" to bring up the properties editor.\n"
              + " 4 - Choose a data source\n";
     
      prop = new GaugeFormProperties(initData, this);
      manager = new formManager(this, parent, initData, "gauge", prop);
      mainForm = parent;

      if (AnnotationPanelCount == 0)
        foreach (var p in typeof(GaugeFormProperties).GetProperties())
        {
          string name = p.Name;
          if (name.StartsWith("SolidGauge_annotationPanels")) AnnotationPanelCount++;
        }
      for (int i = 0; i < AnnotationPanelCount; i++)
      { _solidGauge.addAnnotationPanel(); }

      _solidGauge.setPatchAnnotationCallback(AnnotationCallback);


      initDataNode = initData;
      prop.ApplyAllProperties(this);
      if (!manager.initForm())
      {
        Rectangle s = Screen.FromControl(this).Bounds;
        this.Location = new Point((s.Width - this.Width) >> 1, (s.Height - this.Height) >> 1);
      }
      YDataRenderer.minMaxCheckDisabled = true;
      try { prop.ApplyAllProperties(_solidGauge); }
      catch (TargetInvocationException e) { LogManager.Log("SolidGauge initialization raised an exception (" + e.InnerException.Message + ")"); }
      YDataRenderer.minMaxCheckDisabled = false;




      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration,capture);
      _solidGauge.resetRefrenceSize();
      _solidGauge.AllowPrintScreenCapture = true;
      _solidGauge.proportionnalValueChangeCallback = manager.proportionalValuechanged;
      _solidGauge.resizeRule = Proportional.ResizeRule.RELATIVETOBOTH;
      _solidGauge.AllowRedraw();
      _solidGauge.OnDblClick += RendererCanvas_DoubleClick;
      if (constants.OSX_Running)
      {
        _solidGauge.OnRightClick += rendererCanvas_RightClick;
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
        string resolution = YDataRenderer.FloatToStrformats[Math.Min(sensor.get_resolution().ToString().Length, 6)];
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

    private void RendererCanvas_DoubleClick(object sender, EventArgs e)
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
      _solidGauge.capture();
    }



    private void gaugeForm_Load(object sender, EventArgs e)
    {
    }

    object  GetPropertyValue(UIElement src)
    {
      string fullpropname = "";
      string propType = "";
      string OriginalPropName = "";
      List<string> path = src.ExtractPropPath(ref OriginalPropName, ref fullpropname, ref propType);

      switch (propType)
      {
        case "Form":
          return GenericProperties.newGetProperty(this, prop, fullpropname, path);
         
        case "SolidGauge":
          return GenericProperties.newGetProperty(_solidGauge, prop, fullpropname, path);
       
       
      }
      return null;

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
          GenericProperties.copyProperty_STT(this, prop, fullpropname, path);
          break;
        case "SolidGauge":
          GenericProperties.copyProperty_STT(_solidGauge, prop, fullpropname, path);
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
          GenericProperties.copyProperty_STT(this, prop, fullpropname, path);
          break;
        case "SolidGauge":
          GenericProperties.copyProperty_STT(_solidGauge, prop, fullpropname, path);
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

        int deltaX = Size.Width - _solidGauge.usableUiWidth();
        int deltaY = Size.Height - _solidGauge.usableUiHeight();
        newCoef = Proportional.resizeCoef(Proportional.ResizeRule.RELATIVETOBOTH, _solidGauge.refWidth, _solidGauge.refHeight, RestoreBounds.Size.Width - deltaX, RestoreBounds.Size.Height - deltaX);
        _solidGauge.resetProportionalSizeObjectsCachePush(newCoef);  // reset all size related cached objects    
      }
      string dt = manager.getConfigData();
      if (newCoef != 0) _solidGauge.resetProportionalSizeObjectsCachePop();
      return "<GaugeForm>\n"
            + dt
            + "</GaugeForm>\n";
    }

    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, GetPropertyValue,true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    {  mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, GetPropertyValue,false);
    }

    public void SourceChanged(CustomYSensor value)
    {
      noDataSourcepanel.enabled = (value is NullYSensor);
      if (value is NullYSensor) _solidGauge.value = 0;
      else value.registerCallback(this);

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      if (source == prop.DataSource_source)
        _solidGauge.value = source.isOnline() ? M.get_averageValue() : 0;
    }

    private void gaugeForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop,  PropertyChanged2, GetPropertyValue,false);
    }

    private void gaugeForm_Activated(object sender, EventArgs e)
    {

    }

    private void gaugeForm_Load_1(object sender, EventArgs e)
    {
      manager.initForm();
    }

    private void gaugeForm_Deactivate(object sender, EventArgs e)
    {
      mainForm.widgetLostFocus(this);
    }
  }




}
