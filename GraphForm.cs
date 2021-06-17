﻿/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  charts  widget
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
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using YDataRendering;
using YoctoVisualization.Properties;
using System.Drawing;
using System.Reflection;

namespace YoctoVisualisation
{


  public partial class GraphForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    //    Double FirstLiveValue = 0;
    private GraphFormProperties prop;
    public static int YAxisCount = 0;
    public static int AnnotationPanelCount = 0;
    private int SeriesCount = 0;

    private int ZoneCountPerYaxis = 0;
    private int MarkerCountPerXaxis = 0;
    List<Marker> markers = null;
    ToolStripMenuItem markersMenu = null;
    ToolStripMenuItem deleteMarkeOption = null;
    private MessagePanel noDataSourcepanel;
    private MessagePanel offLineSourcesPanel;
    private MessagePanel captureRunningPanel;

    private YGraph _cartesianChart;
    public YGraph cartesianChart { get { return _cartesianChart; } }
    private List<ChartSerie> seriesProperties;
    String[] offlineMessages;
    bool[] showOffline;

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public GraphForm(StartForm parent, XmlNode initData)
    {
      InitializeComponent();
      seriesProperties = new List<ChartSerie>();
      _cartesianChart = new YGraph(rendererCanvas, LogManager.Log);
      _cartesianChart.getCaptureParameters = constants.getCaptureParametersCallback;
      _cartesianChart.DisableRedraw();

      noDataSourcepanel = _cartesianChart.addMessagePanel();
      noDataSourcepanel.text = "No data source configured\n"
              + " 1 - Make sure you have a Yoctopuce sensor connected.\n"
              + " 2 - Do a right-click on this window.\n"
              + " 3 - Choose \"Configure this graph\" to bring up the properties editor.\n"
              + " 4 - Choose a data source\n";

      offLineSourcesPanel = _cartesianChart.addMessagePanel();
      offLineSourcesPanel.bgColor = System.Drawing.Color.FromArgb(192, 255, 192, 192);
      offLineSourcesPanel.borderColor = System.Drawing.Color.DarkRed;
      offLineSourcesPanel.font.color = System.Drawing.Color.DarkRed;
      offLineSourcesPanel.panelHrzAlign = MessagePanel.HorizontalAlignPos.RIGHT;
      offLineSourcesPanel.panelVrtAlign = MessagePanel.VerticalAlignPos.TOP;



      captureRunningPanel = _cartesianChart.addMessagePanel();
      captureRunningPanel.bgColor = System.Drawing.Color.FromArgb(240, 200, 255, 193); ;
      captureRunningPanel.borderColor = System.Drawing.Color.DarkGreen;
      captureRunningPanel.font.color = System.Drawing.Color.DarkGreen;
      captureRunningPanel.panelHrzAlign = MessagePanel.HorizontalAlignPos.LEFT;
      captureRunningPanel.panelVrtAlign = MessagePanel.VerticalAlignPos.TOP;


      _cartesianChart.setPatchAnnotationCallback(AnnotationCallback);


      foreach (var p in typeof(YaxisDescription).GetProperties())
      {
        if (p.Name.StartsWith("zones")) ZoneCountPerYaxis++;
      }

      foreach (var p in typeof(XaxisDescription).GetProperties())
      {
        if (p.Name.StartsWith("markers")) MarkerCountPerXaxis++;
      }

      


      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graph_series"))
        {

          SeriesCount++;
        }
      }

      if (YAxisCount == 0)
      {
        AnnotationPanelCount = 0;
        foreach (var p in typeof(GraphFormProperties).GetProperties())
        {
          string name = p.Name;
          if (name.StartsWith("Graph_yAxes")) YAxisCount++;
          if (name.StartsWith("Graph_annotationPanel")) AnnotationPanelCount++;
        }
      }


      for (int i = 0; i < YAxisCount; i++)
      {
        YAxis axis = _cartesianChart.addYAxis();
        for (int j = 0; j < ZoneCountPerYaxis; j++) axis.AddZone();
      }

      markers = new List<Marker>();
      for (int i = 0; i < MarkerCountPerXaxis; i++)
      {
        Marker m = _cartesianChart.xAxis.AddMarker();
        m.xposition= TimeConverter.ToUnixTime(DateTime.UtcNow) +i*60;
        markers.Add(m);
      }



      for (int i = 0; i < AnnotationPanelCount; i++)
      { _cartesianChart.addAnnotationPanel(); }

      prop = new GraphFormProperties(initData, this);

      for (int i = 0; i < SeriesCount; i++)
      {
        seriesProperties.Add((ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null));
        _cartesianChart.addSerie();
      }
      _cartesianChart.yAxes[0].visible = true;

      offlineMessages = new String[SeriesCount];
      showOffline = new bool[SeriesCount];

      manager = new formManager(this, parent, initData, "graph", prop);
      mainForm = parent;
      initDataNode = initData;
      prop.ApplyAllProperties(this);
      if (!manager.initForm())
      {
        Rectangle s = Screen.FromControl(this).Bounds;
        this.Location = new Point((s.Width - this.Width) >> 1, (s.Height - this.Height) >> 1);
      }

      YDataRenderer.minMaxCheckDisabled = true;
      try { prop.ApplyAllProperties(_cartesianChart); }
      catch (TargetInvocationException e) { LogManager.Log("Graph initialization raised an exception (" + e.InnerException.Message + ")"); }
      YDataRenderer.minMaxCheckDisabled = false;

      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration, capture);
      _cartesianChart.proportionnalValueChangeCallback = manager.proportionalValuechanged;

      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        s.Init(this, i);
        if (s.DataSource_source != null)
        {
          SourceChanged(i, s.DataSource_source);

        }

      }

      for (int i = 0; i < YAxisCount; i++)
      {

        _cartesianChart.yAxes[i].AxisChanged = AxisParamtersChangedAutomatically;
        _cartesianChart.yAxes[i].AllowAutoShow = true;
      }
      _cartesianChart.resetRefrenceSize();
      _cartesianChart.AllowPrintScreenCapture = true;
      _cartesianChart.proportionnalValueChangeCallback = manager.proportionalValuechanged;
      _cartesianChart.setMarkerCaptureCallbacks(MarkedCaptureStarted, MarkedCaptureStopped);
      _cartesianChart.AllowRedraw();
      deleteMarkeOption = new ToolStripMenuItem("Disable all markers", Resources.disable_marker, disableAllMarkers);
      contextMenuStrip1.Items.Insert(2, deleteMarkeOption);
       markersMenu = new ToolStripMenuItem("Place markers", Resources.add_marker);
      contextMenuStrip1.Items.Insert(2, markersMenu);
      Pen pIcon = new Pen(Color.DarkRed);
      SolidBrush bIcon = new SolidBrush(Color.LightYellow);
      StringFormat stringFormat = new StringFormat();
      stringFormat.Alignment = StringAlignment.Center;
      stringFormat.LineAlignment = StringAlignment.Center;
      Font font1 = new Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Pixel);
      for (int i = 0; i < markers.Count; i++)
      { int index = i;

        Bitmap bm = new Bitmap(16, 16);
        Graphics gr = Graphics.FromImage(bm);
        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        gr.FillEllipse(bIcon, new Rectangle(1, 1, 14, 14));
        gr.DrawEllipse(pIcon, new Rectangle(1, 1, 14, 14));
        gr.DrawString((i + 1).ToString(), font1, Brushes.Black,  new Rectangle(1,1,16, 16),stringFormat);
        markersMenu.DropDownItems.Add(new ToolStripMenuItem("Place marker #" + (i + 1).ToString(), bm, (object sender, EventArgs e) => { startMarkerCapture(index); }));
        gr.Dispose();
      }
      pIcon.Dispose();
      bIcon.Dispose();
      markersMenu.DropDownOpening += updateMakerList;
      contextMenuStrip1.Items.Insert(2, new ToolStripSeparator());
      contextMenuStrip1.Items.Insert(2, new ToolStripMenuItem("Clear dataloggers", Resources.cleardatalogger, clearDataLogger));
      contextMenuStrip1.Items.Insert(2, new ToolStripMenuItem("Reset data view", Resources.resetdataview, resetDataView));
      contextMenuStrip1.Items.Insert(2, new ToolStripSeparator());
      contextMenuStrip1.Opening += ContextMenuStrip1_Opening;


      _cartesianChart.OnDblClick += rendererCanvas_DoubleClick;
      if (constants.OSX_Running)
      {
        _cartesianChart.OnRightClick += rendererCanvas_RightClick;
      }
    }

    private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      bool state = false;
      for (int i = 0; i < markers.Count; i++)
        if (markers[i].enabled) state = true;
      deleteMarkeOption.Enabled = state;

    }

    private string AnnotationCallback(string text)
    {


      for (int i = 0; i < SeriesCount; i++)
      { ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        CustomYSensor sensor = s.DataSource_source;
        string name = "None";
        string avgvalue = "N/A";
        string minvalue = "N/A";
        string maxvalue = "N/A";
        string unit = "";
        if (!(sensor is NullYSensor))
        {
          string resolution = YDataRenderer.FloatToStrformats[Math.Min(sensor.get_resolution().ToString().Length,6)];
          name = s.legend!="" ? s.legend : sensor.get_friendlyName();
          if (sensor.isOnline())
          {
            avgvalue = sensor.get_lastAvgValue().ToString(resolution);
            minvalue = sensor.get_lastMinValue().ToString(resolution);
            maxvalue = sensor.get_lastMaxValue().ToString(resolution);
          }
          unit = sensor.get_unit();
        }
        text = text.Replace("$NAME"     + (i + 1).ToString() + "$", name);
        text = text.Replace("$AVGVALUE" + (i + 1).ToString() + "$", avgvalue);
        text = text.Replace("$MAXVALUE" + (i + 1).ToString() + "$", maxvalue);
        text = text.Replace("$MINVALUE" + (i + 1).ToString() + "$", minvalue);
        text = text.Replace("$UNIT"     + (i + 1).ToString() + "$", unit);

      }


      return text;
    }

    private void startMarkerCapture(int index)
    {
      markers[index].startCapture();

    }


    private void MarkedCaptureStarted(Marker src)
    {
      string str = src.text != "" ?  "Click to place the \"" + src.shortText + "\" marker." :  "Click to place the marker.";
      str += "\nRight-click to cancel the operation.";
      captureRunningPanel.text    = str;
      captureRunningPanel.enabled = true;
    }

    private void MarkedCaptureStopped(Marker src)
    {
      captureRunningPanel.enabled = false;
      prop.RefreshAllProperties(_cartesianChart); 
      mainForm.refreshPropertiesForm();
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
      _cartesianChart.capture();
    }

    public void AxisParamtersChangedAutomatically(GenericAxis source)
    {
      YaxisDescription yaxis = (YaxisDescription)source.userData;
      yaxis.visible = source.visible;
      mainForm.refreshPropertiesForm();

    }




    private void clearDataLogger(object sender, EventArgs e)
    {
      if (System.Windows.Forms.MessageBox.Show("Do you really want to erase contents of all dataloggers related to this graph?",
                                    "Erase dataloggers contents?",
                                    MessageBoxButtons.YesNo) == DialogResult.No) return;

      List<YDataLogger> loggers = new List<YDataLogger>();
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        CustomYSensor sensor = s.DataSource_source;
        if (!(sensor is NullYSensor))
        {
          sensor.stopDataloggerloading();
          string serial = sensor.get_hardwareId();
          int n = serial.IndexOf(".");
          serial = serial.Substring(0, n);
          YDataLogger d = YDataLogger.FindDataLogger(serial + ".dataLogger");
          if (d.isOnline())
            if (loggers.IndexOf(d) < 0)
              loggers.Add(d);
        }
      }

      for (int i = 0; i < loggers.Count; i++)
      {
        loggers[i].forgetAllDataStreams();
        loggers[i].set_recording(YDataLogger.RECORDING_ON);
      }

      bool tmp = prop.Graph_showRecordedData;
      prop.Graph_showRecordedData = false;
      truncateView();
      prop.Graph_showRecordedData = tmp;

    }

    private void truncateView()
    {
      //FirstLiveValue = now();
      for (int i = 0; i < SeriesCount; i++)
      {
        _cartesianChart.series[i].clear();
    
      }
      double FirstLiveValue = now();
      _cartesianChart.xAxis.set_minMax(FirstLiveValue, FirstLiveValue + _cartesianChart.xAxis.initialZoom); 




    }

    private void updateMakerList(object sender, EventArgs e)
    {
      for (int i = 0; i < markers.Count; i++)
        markersMenu.DropDownItems[i].Text =  markers[i].shortText + (markers[i].enabled?" (enabled)":"");
    }
    


    private void disableAllMarkers(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you really want to disable all markers?", "Disable all markers?",
         MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes) return;
      for (int i = 0; i < markers.Count; i++)
        markers[i].enabled = false;
    }

    private void resetDataView(object sender, EventArgs e)
    {
      prop.Graph_showRecordedData = false;
      truncateView();
      mainForm.refreshPropertiesForm();
    }



    private double now()
    {
      return Math.Round((double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }




    private void GraphForm_Load(object sender, EventArgs e)
    {

    }


    object GetPropertyValue(UIElement src)
    {
      string fullpropname = "";
      string propType = "";
      string OriginalPropName = "";
     // if src.t "DataSource_recording"


      List<string> path = src.ExtractPropPath(ref OriginalPropName, ref fullpropname, ref propType);
      switch (propType)
      {
        case "Form":
          return GenericProperties.newGetProperty(this, prop, fullpropname, path);
          
        case "Graph":
          return GenericProperties.newGetProperty(_cartesianChart, prop, fullpropname, path);

        case "DataSource":
          return null;

      }
      return null;

    }

    void PropertyChanged2(UIElement src)
    {
      string fullpropname = "";
      string propType = "";
      string OriginalPropName = "";
      List<string> path = src.ExtractPropPath(ref OriginalPropName, ref fullpropname, ref propType);
      switch (propType)
      {
        case "Form":
          GenericProperties.copyProperty_STT(this, prop, fullpropname, path);
          break;
        case "Graph":
          GenericProperties.copyProperty_STT(_cartesianChart, prop, fullpropname, path);
          break;

          //  case "DataSource":
          //    manager.AjustHint("");
          //    break;
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
        case "Graph":
          GenericProperties.copyProperty_STT(_cartesianChart, prop, fullpropname, path);
          break;

          //  case "DataSource":
          //    manager.AjustHint("");
          //    break;
      }

    }

    public string getConfigData()
    {

      double newCoef = 0;
      if (WindowState == FormWindowState.Maximized)
      { // if the window is maximized, saved size will be regular size, and maximize
        //  flag will be stored separately , so we need to save font
        //  size relative to  regular size (i.e. RestoreBounds.Size);

        int deltaX = Size.Width - _cartesianChart.usableUiWidth();
        int deltaY = Size.Height - _cartesianChart.usableUiHeight();
        newCoef = Proportional.resizeCoef(Proportional.ResizeRule.RELATIVETOBOTH, _cartesianChart.refWidth, _cartesianChart.refHeight, RestoreBounds.Size.Width - deltaX, RestoreBounds.Size.Height - deltaX);
        _cartesianChart.resetProportionalSizeObjectsCachePush(newCoef);  // reset all size related cached objects    
      }
      string dt = manager.getConfigData();
      if (newCoef != 0) _cartesianChart.resetProportionalSizeObjectsCachePop();
      return "<GraphForm>\n"
            + dt
            + "</GraphForm>\n";


    
    }

    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged2, GetPropertyValue,true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged2, GetPropertyValue,false);
    }
    public int getSensorDataType(int index)
    {
      ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + index.ToString()).GetValue(prop, null);
      return s.DataSource_datatype;
    }


    private List<pointXY[]> decomposeToSegments(List<TimedSensorValue> data, int start, int dataCount)
    {
      int n1 = start;
      int n2 = 0;
      List<pointXY[]> l = new List<pointXY[]>();
      double deltaT =0;
      while (n1 < start+dataCount - 1)
      {
        try
        {  deltaT = data[n1 + 1].DateTime - data[n1].DateTime; }
        catch (Exception )
        { }
        n2 = n1 + 1;
        while ((n2 < dataCount - 1) && (data[n2].DateTime - data[n2 - 1].DateTime < 2 * deltaT)) n2++;
        int count = n2 - n1;
        if (count > 0)
        {
          pointXY[] p = new pointXY[count];
          for (int i = 0; i < count; i++) p[i] = new YDataRendering.pointXY() { x = data[n1 + i].DateTime, y = data[n1 + i].Value };
          l.Add(p);
        }
        n1 = n2;
      }

      return l;

    }


    public void SourceChanged(int index, CustomYSensor value)
    {
      value.registerCallback(this);

      _cartesianChart.DisableRedraw();
      ChartSerie s;

      bool noDataSource = true;
      for (int i = 0; i < SeriesCount; i++)
      { s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        if (!(s.DataSource_source is NullYSensor)) noDataSource = false;
      }
      noDataSourcepanel.enabled = noDataSource;

      if (value != null)
      {
        if (!(value is NullYSensor))
        {
          if (value.isOnline())
            showOffline[index] = false;
          else
          {
            offlineMessages[index] = value.get_friendlyName() + " is OFFLINE";
            showOffline[index] = true;
            LogManager.Log(value.get_friendlyName() + " is OFFLINE");
          }
        }
        else showOffline[index] = false;
      }
      else showOffline[index] = false;

      updateOfflinePanel();


      _cartesianChart.series[index].clear();
      s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + index.ToString()).GetValue(prop, null);
      List<TimedSensorValue> data;
      switch (s.DataSource_datatype)
      { case 1: data = value.minData; break;
        case 2: data = value.maxData; break;
        default: data = value.curData; break;


      }


      List<pointXY[]> l = decomposeToSegments(data, 0, data.Count);
      for (int i = l.Count - 1; i >= 0; i--) _cartesianChart.series[index].InsertPoints(l[i]);


      _cartesianChart.AllowRedraw();


    }

    public void showRecordedDatachanged()
    {

      if (prop == null) return;
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        SourceChanged(i, s.DataSource_source);
      }





    }



    public void DataloggerCompleted(CustomYSensor Source)
    {
      if (!prop.Graph_showRecordedData) return;

      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graph_series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(prop, null);
          if (s.DataSource_source == Source)
          {
            int index = int.Parse(name.Substring(12));
            SourceChanged(index, Source);

          }
        }
      }
    }

    public void DataLoggerProgress()
    {
      int progress = 0;
      int sensorCount = 0;

      if (!prop.Graph_showRecordedData)
      {
        Text = prop.Form_Text;
        return;
      }

      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graph_series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(prop, null);
          if (!(s.DataSource_source is NullYSensor))
          {
            progress += s.DataSource_source.getGetaLoadProgress();
            sensorCount++;

          }

        }
      }

      if ((progress < 100 * sensorCount) && (sensorCount > 0))
        Text = prop.Form_Text + " (loading from datalogger, " + (progress / sensorCount).ToString() + "%)";
      else
        Text = prop.Form_Text;

    }


  

    public void SensorNewDataBlock(CustomYSensor source, int sourceFromIndex, int sourcetoIndex, int targetIndex, bool fromDataLogger)
    {



      if ((fromDataLogger) && (!prop.Graph_showRecordedData)) return;

      if (prop == null) return;

      List<pointXY[]> l;
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graph_series" + i.ToString()).GetValue(prop, null);
        if (s.DataSource_source == source)
        {

          int count = sourcetoIndex - sourceFromIndex + 1;
          pointXY[] datablock = new pointXY[count];

          if (count>1)
          switch (s.DataSource_datatype)
          {
            case 1:

              l = decomposeToSegments(s.DataSource_source.minData, sourceFromIndex, count);
              for (int j = l.Count - 1; j >= 0; j--) _cartesianChart.series[i].InsertPoints(l[j]);



              break;
            case 2:

              l = decomposeToSegments(s.DataSource_source.maxData, sourceFromIndex, count);
              for (int j = l.Count - 1; j >= 0; j--) _cartesianChart.series[i].InsertPoints(l[j]);



              break;
            default:
              l = decomposeToSegments(s.DataSource_source.curData, sourceFromIndex, count);
              for (int j = l.Count - 1; j >= 0; j--) _cartesianChart.series[i].InsertPoints(l[j]);


              break;
          }

        }
      }

    }

    public void updateOfflinePanel()
    {
      string message = "";
      for (int i = 0; i < SeriesCount; i++)
      { if (showOffline[i]) message = message + ((message != "") ? "\n" : "") + offlineMessages[i];

      }
      if (message == "" && offLineSourcesPanel.enabled) offLineSourcesPanel.enabled = false;
      if (message != "" && ((offLineSourcesPanel.text != message) || (!offLineSourcesPanel.enabled))) { offLineSourcesPanel.text = message; offLineSourcesPanel.enabled = true; }


    }



    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      //     if (FirstLiveValue == 0) FirstLiveValue = M.get_endTimeUTC();

      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = seriesProperties[i];
        if (s.DataSource_source == source)
        {
          if (!s.DataSource_source.isOnline())
          {
            offlineMessages[i] = s.DataSource_source.get_friendlyName() + " is OFFLINE";
            showOffline[i] = true;
            updateOfflinePanel();


            return;
          }

          showOffline[i] = false;
          updateOfflinePanel();

          int index = s.DataSource_source.curData.Count - 1;


          switch (s.DataSource_datatype)
          {
            case 1: _cartesianChart.series[i].AddPoint(new pointXY { x = s.DataSource_source.minData[index].DateTime, y = s.DataSource_source.minData[index].Value }); break;
            case 2: _cartesianChart.series[i].AddPoint(new pointXY { x = s.DataSource_source.maxData[index].DateTime, y = s.DataSource_source.maxData[index].Value }); break;
            default:
              _cartesianChart.series[i].AddPoint(new pointXY { x = s.DataSource_source.curData[index].DateTime, y = s.DataSource_source.curData[index].Value }); break;
          }
          _cartesianChart.series[i].unit = s.DataSource_source.get_unit();

        }
      }
    }



    private void GraphForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged2, GetPropertyValue,false);
    }

    private void GraphForm_Deactivate(object sender, EventArgs e)
    {
      mainForm.widgetLostFocus(this);
    }
  

  private void GraphForm_Load_1(object sender, EventArgs e)
  {
    manager.initForm();
  }

} 

}
