/*
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Windows.Data;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveCharts.Events;
using System.Windows.Media;
using LiveCharts.Geared;
using Binding = System.Windows.Data.Binding;
using System.Windows;
using System.Windows.Media.Imaging;
using YoctoVisualisation.Properties;

namespace YoctoVisualisation
{




  public partial class GraphForm : Form
  {
    private XmlNode initDataNode;

    private StartForm mainForm;
    private formManager manager;
    Double FirstLiveValue = 0;
    private GraphFormProperties prop;
    public static int YAxisCount = 0;
    static int SeriesCount = 0;
    static int SectionCount = 0;
    private double minDate = 0;
    private double maxDate = 60;
    private Axis _syncedAxis = new Axis();
    List<Label> offlinelist = new List<Label>();



    public static bool AppliesToSecondaryGraph(string property)
    {
      if (property == "StrokeThickness") return false;
      if (property == "ScalesYAt") return false;
      return true;

    }




    public GraphForm(StartForm parent, XmlNode initData)
    {
      InitializeComponent();


      var dayConfig = Mappers.Xy<TimedSensorValue>()
.X(dayModel => (double)dayModel.DateTime)
.Y(dayModel => dayModel.Value);

      // dirty way to find out how many Yaxis and series can be defined;
      int YAC = 0;
      int SC = 0;
      int SCT = 0;
      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graph_AxisY")) YAC++;
        if (name.StartsWith("Graphs_Series")) SC++;
      }

      foreach (var p in typeof(YAxisParam).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Sections")) SCT++;

      }

      YAxisCount = YAC;
      SeriesCount = SC;
      SectionCount = SCT;

      for (int i = 0; i < YAxisCount; i++)
      {
        int n = i;
        cartesianChart1.AxisY.Add(new Axis());
        cartesianChart1.AxisY[i].LabelFormatter = value => YAxisLabelFormater(n, value);
        for (int j = 0; j < SectionCount; j++)
        {
          cartesianChart1.AxisY[i].Sections.Add(new AxisSection());
          cartesianChart1.AxisY[i].Sections[j].Value = 0;
          cartesianChart1.AxisY[i].Sections[j].SectionWidth = 0;
          cartesianChart1.AxisY[i].Sections[j].Fill = System.Windows.Media.Brushes.LightGreen;
        }

        cartesianChart2.AxisY.Add(new Axis()
        {
          ShowLabels = false

        });


      }


      cartesianChart1.Series = new SeriesCollection(dayConfig);
      cartesianChart2.Series = new SeriesCollection(dayConfig);
      for (int i = 0; i < SeriesCount; i++)
      {
        Label t = new Label();
        t.Size = new System.Drawing.Size(this.Width - 20, 16);
        t.Location = new System.Drawing.Point(0, 10 + i * t.Size.Height);

        t.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
        t.TextAlign = ContentAlignment.TopRight;
        t.AutoSize = false;

        t.Text = "line" + i.ToString();
        t.Visible = false;
        this.Controls.Add(t);

        this.Controls.SetChildIndex(t, 0);
        offlinelist.Add(t);

        cartesianChart1.Series.Add(

                    new LineSeries
                    {
                      Values = new GearedValues<TimedSensorValue>(),
                      Fill = System.Windows.Media.Brushes.Transparent,
                      LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                      PointGeometry = Geometry.Parse(""),
                      PointGeometrySize = 50,
                      ScalesXAt = 0
                    });




        cartesianChart2.Series.Add(

                  new LineSeries
                  {
                    Values = new GearedValues<TimedSensorValue>(),
                    Fill = System.Windows.Media.Brushes.Transparent,
                    LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                    PointGeometry = Geometry.Parse(""),
                    PointGeometrySize = 50,
                    StrokeThickness = 1,
                    ScalesXAt = 0
                  });


      }

      _syncedAxis.Separator = new Separator { IsEnabled = false };
      _syncedAxis.RangeChanged += Axis_OnRangeChanged;
      _syncedAxis.LabelFormatter = value => XAxisLabelFormater(value);

      cartesianChart1.Zoom = ZoomingOptions.X;
      // cartesianChart1.DisableAnimations = false;
      cartesianChart1.Base.Hoverable = false;
      cartesianChart1.Tag = "main";

      cartesianChart2.Base.ScrollMode = ScrollMode.X;
      cartesianChart2.DataTooltip = null;
      cartesianChart2.Base.Hoverable = false;
      cartesianChart2.DataTooltip = null;
      cartesianChart2.DisableAnimations = true;
      cartesianChart2.Tag = "secondary";
      cartesianChart2.ScrollBarFill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 200, 200, 200));



      cartesianChart1.AxisX.Add(_syncedAxis);

      //cartesianChart2.Base.MouseLeftButtonUp += zoomWindowMoved;
      cartesianChart2.AxisX.Add(new Axis
      {
        LabelFormatter = value => XAxisLabelFormater(value)

      });

      cartesianChart2.AxisY.Add(new Axis { Separator = new Separator { IsEnabled = true }, ShowLabels = false });

      var assistant = new BindingAssistant();
      cartesianChart1.AxisX[0].SetBinding(Axis.MinValueProperty,
      new Binding { Path = new PropertyPath("From"), Source = assistant, Mode = BindingMode.TwoWay });
      cartesianChart1.AxisX[0].SetBinding(Axis.MaxValueProperty,
          new Binding { Path = new PropertyPath("To"), Source = assistant, Mode = BindingMode.TwoWay });

      cartesianChart2.Base.SetBinding(CartesianChart.ScrollHorizontalFromProperty,
          new Binding { Path = new PropertyPath("From"), Source = assistant, Mode = BindingMode.TwoWay });
      cartesianChart2.Base.SetBinding(CartesianChart.ScrollHorizontalToProperty,
          new Binding { Path = new PropertyPath("To"), Source = assistant, Mode = BindingMode.TwoWay });


      prop = new GraphFormProperties(initData, this);
      ApplyNavigatorVisibility();
      manager = new formManager(this, parent, initData, "graph", prop);
      mainForm = parent;
      initDataNode = initData;
      prop.ApplyAllProperties(this);
      prop.ApplyAllProperties(cartesianChart1);
      prop.ApplyAllProperties(cartesianChart2, AppliesToSecondaryGraph);
      manager.configureContextMenu(this, contextMenuStrip1, showConfiguration, switchConfiguration);

      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
        s.Init(this, i);
        if (s.DataSource_source != null)
        {
          SourceChanged(i, s.DataSource_source);

          if (s.DataSource_source.curData.Count > 0)
          {
            int last = s.DataSource_source.curData.Count - 1;
            double min = s.DataSource_source.curData[0].DateTime;
            if ((minDate == 0) || (minDate > min)) minDate = min;
            if (maxDate < s.DataSource_source.curData[last].DateTime) maxDate = s.DataSource_source.curData[last].DateTime + 60;

          }
        }
      }
      cartesianChart1.AxisX[0].MinValue = now();
      cartesianChart1.AxisX[0].MaxValue = cartesianChart1.AxisX[0].MinValue + prop.Graph_AxisX0.initialZoom;
      cartesianChart1.AxisX[0].MinRange = 15;
      cartesianChart2.AxisX[0].MinValue = cartesianChart1.AxisX[0].MinValue;
      cartesianChart2.AxisX[0].MaxValue = cartesianChart1.AxisX[0].MaxValue;
      cartesianChart2.AxisX[0].FontSize = 16;

      SetXAxislabelsFormat(0, prop.Graph_AxisX0.initialZoom);
      SetXAxislabelsFormat(1, prop.Graph_AxisX0.initialZoom);

      contextMenuStrip1.Items.Insert(2, new ToolStripMenuItem("Clear dataloggers", Resources.cleardatalogger, clearDataLogger));
      contextMenuStrip1.Items.Insert(2, new ToolStripMenuItem("Reset dataview", Resources.resetdataview, resetDataView));
      contextMenuStrip1.Items.Insert(2, new ToolStripSeparator());

    }

    private void AdjustXaxisRightLimit()
    {

      if (prop.Graph_showRecordedData)
      {
        double max = 0;
        for (int i = 0; i < SeriesCount; i++)
        {
          ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
          double smax = s.DataSource_source.get_lastDataTimeStamp();
          if ((smax > 0) && (smax > max)) max = smax;
        }
        if (max > 0) cartesianChart2.AxisX[0].MaxValue = max;

      }
    }





    private void AdjustXaxisLeftLimit()
    {

      if (prop.Graph_showRecordedData)
      {
        double min = now();
        for (int i = 0; i < SeriesCount; i++)
        {
          ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
          double smin = s.DataSource_source.get_firstDataloggerTimeStamp();
          if ((smin > 0) && (smin < min)) min = smin;
        }
        cartesianChart2.AxisX[0].MinValue = min;
      }
      else
      {
        double min = now();
        for (int i = 0; i < SeriesCount; i++)
        {
          ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
          double smin = s.DataSource_source.get_firstLiveDataTimeStamp();
          if ((smin > 0) && (smin < min)) min = smin;
        }
        cartesianChart2.AxisX[0].MinValue = min;
      }
    }

    private void clearDataLogger(object sender, EventArgs e)
    {
      if (System.Windows.Forms.MessageBox.Show("Do you really want to erase contents of all dataloggers related to this graph?",
                                    "Erase dataloggers contents?",
                                    MessageBoxButtons.YesNo) == DialogResult.No) return;

      List<YDataLogger> loggers = new List<YDataLogger>();
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
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
      FirstLiveValue = now();
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
        SourceChanged(i, s.DataSource_source);
      }

      cartesianChart1.AxisX[0].MinValue = now();
      cartesianChart1.AxisX[0].MaxValue = cartesianChart1.AxisX[0].MinValue + prop.Graph_AxisX0.initialZoom;
      cartesianChart1.AxisX[0].MinRange = 15;
      cartesianChart2.AxisX[0].MinValue = cartesianChart1.AxisX[0].MinValue;
      cartesianChart2.AxisX[0].MaxValue = cartesianChart1.AxisX[0].MaxValue;
      SetXAxislabelsFormat(1, cartesianChart1.AxisX[0].MaxValue - cartesianChart1.AxisX[0].MinValue);
      SetXAxislabelsFormat(2, cartesianChart2.AxisX[0].MaxValue - cartesianChart2.AxisX[0].MinValue);


    }



    private void resetDataView(object sender, EventArgs e)
    {
      //prop.Graph_showRecordedData = false;
      //truncateView();

      cartesianChart1.AxisX[0].MinValue = now();
      cartesianChart1.AxisX[0].MaxValue = cartesianChart1.AxisX[0].MinValue + prop.Graph_AxisX0.initialZoom;
      cartesianChart1.AxisX[0].MinRange = 15;
      SetXAxislabelsFormat(1, cartesianChart1.AxisX[0].MaxValue - cartesianChart1.AxisX[0].MinValue);
    }

    private void ApplyNavigatorVisibility()
    {
      if (prop.Graph_showNavigator)
      {
        cartesianChart2.Visible = true;
        cartesianChart1.Height = ClientSize.Height - 32 - prop.Graph_navigatorHeight;
        cartesianChart2.Location = new System.Drawing.Point(12, ClientSize.Height - 24 - prop.Graph_navigatorHeight);
        cartesianChart2.Height = prop.Graph_navigatorHeight;

      }
      else
      {
        cartesianChart2.Visible = false;
        cartesianChart1.Height = ClientSize.Height - 24;

      }
    }

    private double now()
    {
      return Math.Round((double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }

    private void setZoom(double zoomValue)
    {
      cartesianChart1.AxisX[0].MinValue = cartesianChart1.AxisX[0].MaxValue - zoomValue;
    }

    private void UpdateScrollBar()
    {

    }





    private void Axis_OnRangeChanged(RangeChangedEventArgs eventargs)
    {

      SetXAxislabelsFormat(1, eventargs.Range);
    }

    private void SetXAxislabelsFormat(int graphsIndex, double currentRange)
    {
      Axis a = graphsIndex == 1 ? cartesianChart1.AxisX[0] : cartesianChart2.AxisX[0];

      if (currentRange < 60)
      {
        a.LabelFormatter = x => constants.UnixTimeStampToDateTime(x).ToString("HH:mm:ss.ff");

        return;
      }

      if (currentRange < 3600)
      {
        a.LabelFormatter = x => constants.UnixTimeStampToDateTime(x).ToString("HH:mm:ss");

        return;
      }

      if (currentRange < 86400)
      {
        a.LabelFormatter = x => constants.UnixTimeStampToDateTime(x).ToString("HH:mm");

        return;
      }


      if (currentRange < 7 * 86400)
      {
        a.LabelFormatter = x => constants.UnixTimeStampToDateTime(x).ToString("dd/MM  HH:mm");

        return;
      }



      a.LabelFormatter = x => constants.UnixTimeStampToDateTime(x).ToString("dd/MM/yyy");


    }


    string YAxisLabelFormater(int axisIndex, double value)
    {
      YAxisParam axis = (YAxisParam)prop.GetType().GetProperty("Graph_AxisY" + axisIndex.ToString()).GetValue(prop, null);
      string format = axis.Labels_precision;
      int p = format.IndexOf('.');
      int n = 0;
      if (p >= 0) n = format.Length - p - 1;

      return value.ToString("F" + n.ToString());





    }

    string XAxisLabelFormater(double value)
    {

      if (maxDate - minDate < 5 * 60) return constants.UnixTimeStampToDateTime(value).ToString("T");
      else if (maxDate - minDate < 24 * 3600) return constants.UnixTimeStampToDateTime(value).ToString("t");
      else return constants.UnixTimeStampToDateTime(value).ToString("g");
    }


    private void GraphForm_Load(object sender, EventArgs e)
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
        case "Graph":
          if (OriginalPropName == "initialZoom") setZoom(prop.Graph_AxisX0.initialZoom);
          else if ((OriginalPropName == "Graph_navigatorHeight")
                || (OriginalPropName == "Graph_showNavigator")) ApplyNavigatorVisibility();
          else

            GenericProperties.newSetProperty(cartesianChart1, prop, fullpropname, path);
          break;
        case "Graph2":
          GenericProperties.newSetProperty(cartesianChart2, prop, fullpropname, path);
          break;
        case "Graphs":
          GenericProperties.newSetProperty(cartesianChart1, prop, fullpropname, path);
          GenericProperties.newSetProperty(cartesianChart2, prop, fullpropname, path, AppliesToSecondaryGraph);
          break;
        case "DataSource":
          manager.AjustHint("");
          break;
      }

    }

    public string getConfigData()
    {
      return "<GraphForm>\n"
            + manager.getConfigData()
            + "</GraphForm>\n";
    }

    private void showConfiguration(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, true);
    }

    private void switchConfiguration(object sender, EventArgs e)
    { //MessageBox.Show("pouet");
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }
    public int getSensorDataType(int index)
    {
      ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + index.ToString()).GetValue(prop, null);
      return s.DataSource_datatype;
    }

    public void SourceChanged(int index, CustomYSensor value)
    {
      value.registerCallback(this);


      cartesianChart1.Series[index].Values.Clear();
      cartesianChart2.Series[index].Values.Clear();

      if (value != null)
      {
        if (!(value is NullYSensor))
        {
          if (value.isOnline())
            offlinelist[index].Visible = false;
          else
          {
            offlinelist[index].Text = value.get_friendlyName() + " is OFFLINE";
            offlinelist[index].Visible = true;
          }
        }
        else offlinelist[index].Visible = false;

        if (prop.Graph_showRecordedData)
        {
          if (value.curData.Count > 1)
          {
            cartesianChart2.AxisX[0].MinValue = value.curData[0].DateTime;
            cartesianChart2.AxisX[0].MaxValue = value.curData[value.curData.Count-1].DateTime;
          }

          switch (getSensorDataType(index))
          {
            case 1:
              cartesianChart1.Series[index].Values.AddRange(value.minData);

              
              cartesianChart2.Series[index].Values.AddRange(value.minData);
              break;
            case 2:
              cartesianChart1.Series[index].Values.AddRange(value.maxData);
              cartesianChart2.Series[index].Values.AddRange(value.maxData);
              break;

            default:
              cartesianChart1.Series[index].Values.AddRange(value.curData);
              cartesianChart2.Series[index].Values.AddRange(value.curData);
              break;

          }
        }
        else
        {
          int n = 0;
          while ((n < value.curData.Count) && (value.curData[n].DateTime < FirstLiveValue)) n++;
          if (n < value.curData.Count)
          {
            switch (getSensorDataType(index))
            {
              case 1:
                cartesianChart1.Series[index].Values.AddRange(value.minData.GetRange(n, value.minData.Count - n));
                cartesianChart2.Series[index].Values.AddRange(value.minData.GetRange(n, value.minData.Count - n));
                break;

              case 2:
                cartesianChart1.Series[index].Values.AddRange(value.maxData.GetRange(n, value.maxData.Count - n));
                cartesianChart2.Series[index].Values.AddRange(value.maxData.GetRange(n, value.maxData.Count - n));
                break;

              default:
                cartesianChart1.Series[index].Values.AddRange(value.curData.GetRange(n, value.curData.Count - n));
                cartesianChart2.Series[index].Values.AddRange(value.curData.GetRange(n, value.curData.Count - n));
                break;


            }

          }
        }
      }
    }

    public void showRecordedDatachanged()
    {
      if (prop == null) return;
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
        SourceChanged(i, s.DataSource_source);
      }

      AdjustXaxisLeftLimit();



    }



    public void DataloggerCompleted(CustomYSensor Source)
    {
      if (!prop.Graph_showRecordedData) return;

      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graphs_Series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(prop, null);
          if (s.DataSource_source == Source)
          {
            int index = int.Parse(name.Substring(13));
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
        if (name.StartsWith("Graphs_Series"))
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
      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
        if (s.DataSource_source == source)
        {

          switch (s.DataSource_datatype)
          {
            case 1:
              cartesianChart1.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.minData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              cartesianChart2.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.minData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              break;
            case 2:
              cartesianChart1.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.maxData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              cartesianChart2.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.maxData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              break;
            default:
              cartesianChart1.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.curData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              cartesianChart2.Series[i].Values.InsertRange(targetIndex, s.DataSource_source.curData.GetRange(sourceFromIndex, (sourcetoIndex - sourceFromIndex + 1)));
              break;
          }



          for (int j = 0; j < cartesianChart1.Series[i].Values.Count - 1; j++)
          {
            if (cartesianChart1.Series[i].Values[j] == cartesianChart1.Series[i].Values[j + 1])
              throw new Exception("Ooops, Twin value and index " + j.ToString());
          }

        }
      }
      AdjustXaxisLeftLimit();
      AdjustXaxisRightLimit();
      SetXAxislabelsFormat(1, cartesianChart1.AxisX[0].MaxValue - cartesianChart1.AxisX[0].MinValue);
      SetXAxislabelsFormat(2, cartesianChart2.AxisX[0].MaxValue - cartesianChart2.AxisX[0].MinValue);
    }

    private void AdjustCartesianChart2AxisX(ChartSerie s)
    {
     

    }

    public void SensorValuecallback(CustomYSensor source, YMeasure M)
    {
      if (prop == null) return;

      if (FirstLiveValue == 0) FirstLiveValue = M.get_endTimeUTC();

      for (int i = 0; i < SeriesCount; i++)
      {
        ChartSerie s = (ChartSerie)prop.GetType().GetProperty("Graphs_Series" + i.ToString()).GetValue(prop, null);
        if (s.DataSource_source == source)
        {
          if (!s.DataSource_source.isOnline())
          {
            offlinelist[i].Text = s.DataSource_source.get_friendlyName() + " is OFFLINE";
            offlinelist[i].Visible = true;
            return;
          }

          offlinelist[i].Visible = false;

          int index = s.DataSource_source.curData.Count - 1;

          switch (s.DataSource_datatype)
          {
            case 1: cartesianChart1.Series[i].Values.Add(s.DataSource_source.minData[index]); break;
            case 2: cartesianChart1.Series[i].Values.Add(s.DataSource_source.maxData[index]); break;
            default: cartesianChart1.Series[i].Values.Add(s.DataSource_source.curData[index]); break;
          }

          if (index > 1)
          { // ajust Maingraph scroll if aligned with right side
            double lasGraphTime = Math.Round(cartesianChart1.AxisX[0].MaxValue);
            double lastValueTime = Math.Round(s.DataSource_source.curData[index - 1].DateTime);
            if (lasGraphTime == lastValueTime)
            {
              double delta = lasGraphTime - cartesianChart1.AxisX[0].MinValue;
              cartesianChart1.AxisX[0].MaxValue = s.DataSource_source.curData[index].DateTime;
              cartesianChart1.AxisX[0].MinValue = cartesianChart1.AxisX[0].MaxValue - delta;
              AdjustXaxisRightLimit();
              cartesianChart2.Base.ScrollHorizontalFrom = cartesianChart1.AxisX[0].MinValue;
              cartesianChart2.Base.ScrollHorizontalTo = cartesianChart1.AxisX[0].MaxValue;
              switch (s.DataSource_datatype)
              {
                case 1: cartesianChart2.Series[i].Values.Add(s.DataSource_source.minData[index]); break;
                case 2: cartesianChart2.Series[i].Values.Add(s.DataSource_source.maxData[index]); break;
                default: cartesianChart2.Series[i].Values.Add(s.DataSource_source.curData[index]); break;
              }
              return;
            }
          }
          switch (s.DataSource_datatype)
          {
            case 1: cartesianChart2.Series[i].Values.Add(s.DataSource_source.minData[index]); break;
            case 2: cartesianChart2.Series[i].Values.Add(s.DataSource_source.maxData[index]); break;
            default: cartesianChart2.Series[i].Values.Add(s.DataSource_source.curData[index]); break;
          }



          AdjustXaxisRightLimit();

        }
      }




    }



    private void GraphForm_Enter(object sender, EventArgs e)
    {
      mainForm.ShowPropertyForm(this, prop, PropertyChanged, false);
    }

    private void cartesianChart2_DataClick(object sender, ChartPoint chartPoint)
    {
      return;
    


    }
  }

  public class BindingAssistant : INotifyPropertyChanged
  {
    private double _from;
    private double _to;

    public double From
    {
      get { return _from; }
      set
      {
        _from = value;
        OnPropertyChanged("From");
      }
    }

    public double To
    {
      get { return _to; }
      set
      {
        _to = value;
        OnPropertyChanged("To");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
      if (PropertyChanged != null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }


}
