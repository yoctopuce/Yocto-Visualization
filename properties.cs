/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  all widgets customizable properties are described here 
 *  feel free to remove/modify/add more.
 * 
 * 
 *  Prefix describe the widget
 *  Name must match the widget properties
 *  example:
 *   GaugeFormProperties contains
 *     SolidGauge_Uses360Mode which matches the  SolidGauge's Uses360Mode properties
 *    
 *  Some properties might have an  index
 *  example
 *    for graphs
 *    Graphs_Series0  is the first series
 *    ....
 *    Graphs_Series3  is the fourth series
 *    if you need a fifth series, just create a Graphs_Series4 properties
 *    
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace YoctoVisualisation
{

  //****************************
  //  Solid gauge
  //****************************

  public class GaugeFormProperties : GenericProperties
  {
    public GaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { }

    private bool _SolidGauge_Uses360Mode = false;
    [DisplayName("360 mode"),
     CategoryAttribute("Gauge"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Gauge geometry")]
    public bool SolidGauge_Uses360Mode
    {
      get { return _SolidGauge_Uses360Mode; }
      set { _SolidGauge_Uses360Mode = value; }
    }

    private double _SolidGauge_From = 0;
    [DisplayName("Minimium value"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Minimum values displayable by the gauge")]
    public double SolidGauge_From
    {
      get { return _SolidGauge_From; }
      set { _SolidGauge_From = value; }
    }


    private Color _SolidGauge_FromColor = Color.Green;
    [DisplayName("Minimium value Color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Gauge color when minimum value is reached")]
    public Color SolidGauge_FromColor
    {
      get { return _SolidGauge_FromColor; }
      set { _SolidGauge_FromColor = value; }
    }


    private Color _SolidGauge_GaugeBackground = Color.LightGray;
    [DisplayName("Background color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Gauge background color")]
    public Color SolidGauge_GaugeBackground
    {
      get { return _SolidGauge_GaugeBackground; }
      set { _SolidGauge_GaugeBackground = value; }
    }

    private doubleNan _SolidGauge_InnerRadius = new doubleNan();
    [DisplayName("Inner radius"),
     CategoryAttribute("Gauge"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Gauge inner radius, leave blank for automatic behavior.")]
    public doubleNan SolidGauge_InnerRadius
    {
      get { return _SolidGauge_InnerRadius; }
      set { _SolidGauge_InnerRadius = value; }
    }

    private double _SolidGauge_To = 100;
    [DisplayName("Maximim value"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Maximum values displayable by the gauge")]
    public double SolidGauge_To
    {
      get { return _SolidGauge_To; }
      set { _SolidGauge_To = value; }
    }


    private Color _SolidGauge_ToColor = Color.Red;
    [DisplayName("Maximim value Color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Gauge color when maximum value is reached")]
    public Color SolidGauge_ToColor
    {
      get { return _SolidGauge_ToColor; }
      set { _SolidGauge_ToColor = value; }
    }


    private Color _SolidGauge_Stroke = Color.Black;
    [DisplayName("Stroke  Color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Gauge edges color")]
    public Color SolidGauge_Stroke
    {
      get { return _SolidGauge_Stroke; }
      set { _SolidGauge_Stroke = value; }
    }

    private double _SolidGauge_StrokeThickness = 0;
    [DisplayName("Stroke thickness"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Gauge edges thickness")]
    public double SolidGauge_StrokeThickness
    {
      get { return _SolidGauge_StrokeThickness; }
      set { _SolidGauge_StrokeThickness = value; }
    }

    private Font _SolidGauge_Font = new Font("Arial", 8, System.Drawing.FontStyle.Regular);
    [DisplayName("Font"),
      CategoryAttribute("Gauge"),
      DescriptionAttribute("Gauge font")]
    public Font SolidGauge_Font
    {
      get { return _SolidGauge_Font; }
      set { _SolidGauge_Font = value; }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }


    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      DescriptionAttribute("Yoctopuce sensor feeding the gauge.")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        ((gaugeForm)ownerForm).SourceChanged(value);

      }
    }
    [DisplayName("Sensor freq"),
    TypeConverter(typeof(FrequencyConverter)),
       CategoryAttribute("Data source"),
    NotSavedInXMLAttribute(true),
    DescriptionAttribute("Sensor data acquisition frequency. Note that modifying this setting will affect the device configuration.")]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private string _DataSource_precision = "0.1";
    [TypeConverter(typeof(FormatterPrecisionConverter)),
      DisplayName("Precision"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("How many digits shown after the decimal point")]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }


  }

  //****************************
  //  angular gauge
  //****************************

  public class AngularGaugeFormProperties : GenericProperties
  {
    public AngularGaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("Yoctopuce sensor feeding the gauge.")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        ((angularGaugeForm)ownerForm).SourceChanged(value);

      }
    }

    [DisplayName("Sensor freq"),
    TypeConverter(typeof(FrequencyConverter)),
    CategoryAttribute("Data source"),
    NotSavedInXMLAttribute(true),
    DescriptionAttribute("Sensor data acquisition frequency. Note that modifying this setting will affect the device configuration.")]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private double _AngularGauge_FromValue = 0;
    [DisplayName("Minimium value"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Minimum value displayable by the gauge.")]
    public double SolidGauge_FromValue
    {
      get { return _AngularGauge_FromValue; }
      set { _AngularGauge_FromValue = value; }
    }



    private double _AngularGauge_ToValue = 100;
    [DisplayName("Maximim value"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Maximum value displayable by the gauge.")]
    public double AngularGauge_ToValue
    {
      get { return _AngularGauge_ToValue; }
      set { _AngularGauge_ToValue = value; }
    }


    private double _AngularGauge_Wedge = 300;
    [DisplayName("Wedge"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Opening angle in the gauge.")]
    public double AngularGauge_Wedge
    {
      get { return _AngularGauge_Wedge; }
      set { _AngularGauge_Wedge = value; }
    }


    private double _AngularGauge_TickStep = 2;
    [DisplayName("Ticks step"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Separation between every tick in the gauge.")]
    public double AngularGauge_TickStep
    {
      get { return _AngularGauge_TickStep; }
      set { _AngularGauge_TickStep = value; }
    }

    private double _AngularGauge_LabelsStep = 10;
    [DisplayName("Labels step"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Separation between every label in the gauge.")]
    public double AngularGauge_LabelsStep
    {
      get { return _AngularGauge_LabelsStep; }
      set { _AngularGauge_LabelsStep = value; }
    }

    private double _AngularGauge_TicksStrokeThickness = 2;
    [DisplayName("Ticks thickness"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Ticks stroke thickness")]
    public double AngularGauge_TicksStrokeThickness
    {
      get { return _AngularGauge_TicksStrokeThickness; }
      set { _AngularGauge_TicksStrokeThickness = value; }
    }

    private Color _AngularGauge_TicksForeground = Color.Silver;
    [DisplayName("Ticks color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Color of the ticks.")]
    public Color AngularGauge_TicksForeground
    {
      get { return _AngularGauge_TicksForeground; }
      set { _AngularGauge_TicksForeground = value; }
    }

    private Color _AngularGauge_NeedleFill = Color.DarkGray;
    [DisplayName("Needle color"),
     CategoryAttribute("Gauge"),
     DescriptionAttribute("Color of the gauge needle")]
    public Color AngularGauge_NeedleFill
    {
      get { return _AngularGauge_NeedleFill; }
      set { _AngularGauge_NeedleFill = value; }
    }


    private YAngularSection _AngularGauge_Sections0 = new YAngularSection(Color.Green);
    [TypeConverter(typeof(YAngularSectionParamConverter)),
     DisplayName("Section 1"),
     CategoryAttribute("Sections"),
     DescriptionAttribute("Properties of the first section. Expand for more.")]
    public YAngularSection AngularGauge_Sections0
    {
      get { return _AngularGauge_Sections0; }
      set { _AngularGauge_Sections0 = value; }
    }

    private YAngularSection _AngularGauge_Sections1 = new YAngularSection(Color.Yellow);
    [TypeConverter(typeof(YAngularSectionParamConverter)),
     DisplayName("Section 2"),
     CategoryAttribute("Sections"),
     DescriptionAttribute("Properties of the first section. Expand for more.")]
    public YAngularSection AngularGauge_Sections1
    {
      get { return _AngularGauge_Sections1; }
      set { _AngularGauge_Sections1 = value; }
    }

    private YAngularSection _AngularGauge_Sections2 = new YAngularSection(Color.Red);
    [TypeConverter(typeof(YAngularSectionParamConverter)),
     DisplayName("Section 3"),
     CategoryAttribute("Sections"),
     DescriptionAttribute("Properties of the first section. Expand for more.")]
    public YAngularSection AngularGauge_Sections2
    {
      get { return _AngularGauge_Sections2; }
      set { _AngularGauge_Sections2 = value; }
    }



  }


  public class YAngularSection
  {
    public YAngularSection(Color intialcolor)
    {
      _color = intialcolor;

    }

    private double _FromValue = 0;
    [DisplayName("From"),
     DescriptionAttribute("Anglar section start. Set both \"from\" and \"To\" values to zero to disable the section.")]
    public double FromValue
    {
      get { return _FromValue; }
      set { _FromValue = value; }
    }

    private double _ToValue = 0;
    [DisplayName("To"),
     DescriptionAttribute("Anglar section stop. Set both \"from\" and \"To\"  values to zero to disable the section")]
    public double ToValue
    {
      get { return _ToValue; }
      set { _ToValue = value; }
    }

    private Color _color;
    [DisplayName("Color"),
     DescriptionAttribute("Anglar section Color")]
    public Color Fill
    {
      get { return _color; }
      set { _color = value; }
    }



  }


  //****************************
  //  Digital display
  //****************************


  public class digitalDisplayFormProperties : GenericProperties
  {
    public digitalDisplayFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("Yoctopuce sensor feeding the display.")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        ((digitalDisplayForm)ownerForm).SourceChanged(value);

      }
    }

    [DisplayName("Sensor freq"),
     TypeConverter(typeof(FrequencyConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Sensor data acquisition frequency. Note that modifying this setting will affect the device configuration.")]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    private string _DataSource_precision = "0.1";
    [TypeConverter(typeof(FormatterPrecisionConverter)),
      DisplayName("Precision"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("How many digits shown after the decimal point.")]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }


    private Font _Label_Font = new Font("Arial", 48, System.Drawing.FontStyle.Regular);
    [DisplayName("Font"),
     CategoryAttribute("Display"),
     DescriptionAttribute("Display font")]
    public Font Label_Font
    {
      get { return _Label_Font; }
      set { _Label_Font = value; }
    }

    private Color _Label_ForeColor = Color.Black;
    [DisplayName("Color"),
    CategoryAttribute("Display"),
     DescriptionAttribute("Display digits color.")]
    public Color Label_ForeColor
    {
      get { return _Label_ForeColor; }
      set { _Label_ForeColor = value; }
    }

    private Color _Label_OORColor = Color.Red;
    [DisplayName("Out of range Color"),
     CategoryAttribute("Range Control"),
     DescriptionAttribute("Display digits Color when value is out of range.")]
    public Color Label_OORColor
    {
      get { return _Label_OORColor; }
      set { _Label_OORColor = value; }
    }


    private doubleNan _Label_MaxValue = new doubleNan(Double.NaN);
    [DisplayName("Maximum value"),
     CategoryAttribute("Range Control"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Regular range maximxum value. if value is outside this range, color will turn to \"Out of range Color\". Leave blank if you don't want to define such a range. ")]
    public doubleNan Label_MaxValue
    {
      get { return _Label_MaxValue; }
      set { _Label_MaxValue = value; }
    }

    private doubleNan _Label_MinValue = new doubleNan(Double.NaN);
    [DisplayName("Minimum value"),
     CategoryAttribute("Range Control"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Regular range minimum value. if value goes  outside this range, color will turn to \"Out of range Color\".Leave blank if you don't want to define such a range. ")]
    public doubleNan Label_MinValue
    {
      get { return _Label_MinValue; }
      set { _Label_MinValue = value; }
    }





  }




  /************************************
   *  Yaxis
   */

  public class YAxisParam
  {
    public YAxisParam(LiveCharts.AxisPosition initialposition, bool visible)
    {
      _Position = initialposition;
      _ShowLabels = visible;
    }

    private doubleNan _MinValue = new doubleNan(Double.NaN);
    [DisplayName("Minimum"),
      TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Vertical axis minimum displayable value, leave blank for automatic behavior.")]
    public doubleNan MinValue
    {
      get { return _MinValue; }
      set { _MinValue = value; }
    }

    private string _FontFamily = "Calibri";
    [DisplayName("Font Family"),
    TypeConverter(typeof(FontConverter)),
    Description("Y-Axis Labels font family.")]
    public string FontFamily
    {
      get { return _FontFamily; }
      set { _FontFamily = value; }
    }






    private bool _ShowLabels = true;
    [DisplayName("Shown"),
     TypeConverter(typeof(YesNoConverter)),
     Description("Show / Hide the vertical axis.")]
    public bool ShowLabels
    {
      get { return _ShowLabels; }
      set { _ShowLabels = value; }
    }




    private string _Labels_precision = "0.1";
    [TypeConverter(typeof(FormatterPrecisionConverter)),
      DisplayName("Labels precision"),
      DescriptionAttribute("How many digits shown after the decimal point in the vertical axis labels.")]
    public string Labels_precision
    {
      get { return _Labels_precision; }
      set { _Labels_precision = value; }
    }


    private doubleNan _MaxValue = new doubleNan(Double.NaN);
    [DisplayName("Maximum"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Vertical axis maximum displayable value, leave blank for automatic behavior.")]
    public doubleNan MaxValue
    {
      get { return _MaxValue; }
      set { _MaxValue = value; }
    }


    private string _Title = "";
    [DisplayName("Title"),
     DescriptionAttribute("Axis title.")]
    public string Title
    {
      get { return _Title; }
      set { _Title = value; }
    }
    /*
    private double _BarUnit = 1;
    [DisplayName("Bar unit"),
     DescriptionAttribute("Bar unit")]
    public double BarUnit
    {
      get { return _BarUnit; }
      set { _BarUnit = value; }
    }
    */
    private Color _Foreground = Color.Gray;
    [DisplayName("Color"),
     DescriptionAttribute("Axis Color.")]
    public Color Foreground
    {
      get { return _Foreground; }
      set { _Foreground = value; }
    }

    private double _FontSize = 16;
    [DisplayName("Font size"),
     DescriptionAttribute("Axis Font Size.")]
    public double FontSize
    {
      get { return _FontSize; }
      set { _FontSize = value; }
    }

    private LiveCharts.AxisPosition _Position = LiveCharts.AxisPosition.LeftBottom;
    [DisplayName("Position"),
     TypeConverter(typeof(YAxisPositionConverter)),
     DescriptionAttribute("Axis left/right position.")]
    public LiveCharts.AxisPosition Position
    {
      get { return _Position; }
      set { _Position = value; }
    }

    private AxisSeparator _separator = new AxisSeparator();
    [DisplayName("Separator"),
     TypeConverter(typeof(AxisSeparatorConverter)),
     DescriptionAttribute("Horizontal grid lines")]
    public AxisSeparator Separator
    {
      get { return _separator; }
      set { _separator = value; }
    }

    private YAxisSection _Sections0 = new YAxisSection(Color.Green);
    [TypeConverter(typeof(YAxisSectionParamConverter)),
     DisplayName("Zone 1"),
     DescriptionAttribute("Properties of the first horizontal zone, expand for more.")]
    public YAxisSection Sections0
    {
      get { return _Sections0; }
      set { _Sections0 = value; }
    }

    private YAxisSection _Sections1 = new YAxisSection(Color.Yellow);
    [TypeConverter(typeof(YAxisSectionParamConverter)),
     DisplayName("Zone 2"),
     DescriptionAttribute("Properties of the second zone, expand for more.")]
    public YAxisSection Sections1
    {
      get { return _Sections1; }
      set { _Sections1 = value; }
    }

    private YAxisSection _Sections2 = new YAxisSection(Color.Red);
    [TypeConverter(typeof(YAxisSectionParamConverter)),
     DisplayName("Zone 3"),
     DescriptionAttribute("Properties of the third horizontal zone, expand for more.")]
    public YAxisSection Sections2
    {
      get { return _Sections2; }
      set { _Sections2 = value; }
    }


  }
  /****************************************
   * Y axis section
   */

  public class YAxisSection
  {
    public YAxisSection(Color intialcolor)
    {
      _color = intialcolor;
    }

    private double _Value = 5;
    [DisplayName("Start"),
     DescriptionAttribute("Axis zone start. Set width to 0 disable the section.")]
    public double Value
    {
      get { return _Value; }
      set { _Value = value; }
    }

    private double _SectionWidth = 0;
    [DisplayName("Width"),
     DescriptionAttribute("Axis zone width. Set it to zero to disable the section.")]
    public double SectionWidth
    {
      get { return _SectionWidth; }
      set { _SectionWidth = value; }
    }

    private Color _color;
    [DisplayName("Color"),
     DescriptionAttribute("Zone fill color")]
    public Color Fill
    {
      get { return _color; }
      set { _color = value; }
    }
    /*  a bit useless
    private Color _Stroke;
    [DisplayName("Edges Color"),
     DescriptionAttribute("Section edges color")]
    public Color Stroke
    {
      get { return _Stroke; }
      set { _Stroke = value; }
    }

    private double _StrokeThickness = 0;
    [DisplayName("Edges Thickness"),
     DescriptionAttribute("Section edges thickness")]
    public double StrokeThickness
    {
      get { return _StrokeThickness; }
      set { _StrokeThickness = value; }
    }
    */
  }



  /*******************************
  * Xaxis (basic)
  */
  public class XAxisBasicParam
  {

    private Color _Foreground = Color.Gray;
    [DisplayName("Color"),
     DescriptionAttribute("X Axis Color.")]
    public Color Foreground
    {
      get { return _Foreground; }
      set { _Foreground = value; }
    }

    private double _FontSize = 16;
    [DisplayName("Font size"),
     DescriptionAttribute("X axis labels font size.")]
    public double FontSize
    {
      get { return _FontSize; }
      set { _FontSize = value; }
    }

    private string _FontFamily = "Calibri";
    [DisplayName("Font Family"),
    TypeConverter(typeof(FontConverter)),
    Description("X axis labelsfont family.")]
    public string FontFamily
    {
      get { return _FontFamily; }
      set { _FontFamily = value; }
    }


    private AxisSeparator _separator = new AxisSeparator();
    [DisplayName("Separator"),
     TypeConverter(typeof(AxisSeparatorConverter)),
     DescriptionAttribute("Vertical grid lines.")]
    public AxisSeparator Separator
    {
      get { return _separator; }
      set { _separator = value; }
    }


  }

  /*******************************
   * Xaxis (extended)
   */

  public class XAxisParam : XAxisBasicParam
  {

    private string _Title = "";
    [DisplayName("Title"),
     DescriptionAttribute("Axis title")]
    public string Title
    {
      get { return _Title; }
      set { _Title = value; }
    }
    /* couldn't find out what the effect was.
    private double _BarUnit = 1;
    [DisplayName("Bar unit"),
     DescriptionAttribute("Bar unit")]
    public double BarUnit
    {
      get { return _BarUnit; }
      set { _BarUnit = value; }
    }

   */

    private double _initialZoom = 60;
    [TypeConverter(typeof(XAxisZoomConverter)),
     DisplayName("Initial Zoom"),
     DescriptionAttribute("Zoom level at application startup.")]
    public double initialZoom
    {
      get { return _initialZoom; }
      set { _initialZoom = value; }
    }

  }

  /************************************
   * separator
   */

  public class AxisSeparator
  {
    public AxisSeparator()
    { }
    public AxisSeparator(bool enabled, double t, Color c)
    {
      IsEnabled = enabled;
      StrokeThickness = t;
      Stroke = c;
    }

    bool _IsEnabled = false;
    [DisplayName("Enabled"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Enable separator lines perpendicular to axis.")
      ]
    public bool IsEnabled
    {
      get { return _IsEnabled; }
      set { _IsEnabled = value; }
    }

    double _StrokeThickness = 1.0;
    [DisplayName("Thickness"),
     DescriptionAttribute("Separator lines thickness.")
    ]
    public double StrokeThickness
    {
      get { return _StrokeThickness; }
      set { _StrokeThickness = value; }
    }


    Color _Stroke = Color.Gainsboro;
    [DisplayName("Color"),
     DescriptionAttribute("Separator lines color.")
    ]
    public Color Stroke
    {
      get { return _Stroke; }
      set { _Stroke = value; }
    }


  }

  /*********************************
   * SERIES
   */

  public class ChartSerie
  {

    private Form ownerForm = null;
    private int index = -1;

    public ChartSerie(Color defaultColor)
    {
      _Stroke = defaultColor;
    }

    public void Init(Form owner, int serieIndex)
    {
      ownerForm = owner;
      index = serieIndex;

    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
      DescriptionAttribute("Yoctopuce sensor feeding the graph")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        if (ownerForm != null)
          ((GraphForm)ownerForm).SourceChanged(index, value);

      }
    }


    [DisplayName("Sensor frequency"),
     TypeConverter(typeof(FrequencyConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Sensor data acquisition frequency. Not that modifying this setting will affect the device configuration.")]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    private int _DataType = 0;
    [DisplayName("Sensor data"),
     TypeConverter(typeof(AvgMinMaxConverter))
     DescriptionAttribute("Which data for sensor are displayed on the graph. Min and Max are available only for frequencies <1Hz")]
    public int DataSource_datatype
    {
      get { return _DataType; }
      set
      {
        _DataType = value;
        if (ownerForm != null)
          ((GraphForm)ownerForm).SourceChanged(index, _DataSource_source);
      }
    }



    [DisplayName("Sensor recording"),
     TypeConverter(typeof(YesNoConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Enable/ disable sensor data recording in the device onboard datalogger. Note that modifying this setting will affect the device configuration.")]

    public bool DataSource_recording
    {
      get { return _DataSource_source.get_recording(); }
      set { _DataSource_source.set_recording(value); }

    }

    private double _StrokeThickness = 2.0;
    [DisplayName("Thickness"),
     DescriptionAttribute("Line thikhness.")]
    public double StrokeThickness
    {
      get { return _StrokeThickness; }
      set { _StrokeThickness = value; }
    }

    private string _Title = "";
    [DisplayName("Title"),
     DescriptionAttribute("Title of the series.")]
    public string Title
    {
      get { return _Title; }
      set { _Title = value; }
    }

    private Color _Stroke = Color.Red;

    [DisplayName("Color"),
     DescriptionAttribute("Line color.")]
    public Color Stroke
    {
      get { return _Stroke; }
      set { _Stroke = value; }
    }

    private int _ScalesYAt = 0;
    [TypeConverter(typeof(YAxisChooserConverter))

      DisplayName("Y axis"),
      DescriptionAttribute("Choose which Y axis the data with be scaled to.")]
    public int ScalesYAt
    {
      get { return _ScalesYAt; }
      set { _ScalesYAt = value; }
    }


  }

  //****************************
  //  graph display
  //  graph_ prefix:  apply to main graph
  //  graphs_ prefix: apply to both graphe
  //  graph2_ prefix:  applies to navigator graph only.
  //****************************


  public class GraphFormProperties : GenericProperties
  {


    public GraphFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { }

    public override bool IsDataSourceAssigned()
    {
      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graphs_Series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(this, null);
          if (!(s.DataSource_source is NullYSensor)) return true;

        }
      }
      return false;

    }

    ChartSerie _Series0 = new ChartSerie(Color.Tomato);
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 1"),
      CategoryAttribute("Data Sources"),
      DescriptionAttribute("First data series, expand for more.")]
    public ChartSerie Graphs_Series0
    {
      get { return _Series0; }
      set { _Series0 = value; }
    }

    ChartSerie _Series1 = new ChartSerie(Color.DodgerBlue);
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 2"),
      CategoryAttribute("Data Sources"),
      DescriptionAttribute("Second data series, expand for more.")]
    public ChartSerie Graphs_Series1
    {
      get { return _Series1; }
      set { _Series1 = value; }
    }

    ChartSerie _Series2 = new ChartSerie(Color.SeaGreen);
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 3"),
      CategoryAttribute("Data Sources"),
      DescriptionAttribute("Third series, expand for more.")]
    public ChartSerie Graphs_Series2
    {
      get { return _Series2; }
      set { _Series2 = value; }
    }


    ChartSerie _Series3 = new ChartSerie(Color.Gold);
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 4"),
      CategoryAttribute("Data Sources"),
      DescriptionAttribute("Fourth series, expand for more.")]
    public ChartSerie Graphs_Series3
    {
      get { return _Series3; }
      set { _Series3 = value; }
    }


    private XAxisParam _Graph_AxisX0 = new XAxisParam();
    [TypeConverter(typeof(XAxisParamConverter)),
     DisplayName("X axis "),
     CategoryAttribute("X/Y Axes"),
     DescriptionAttribute("Properties of the X axis, expand for more.")]
    public XAxisParam Graph_AxisX0
    {
      get { return _Graph_AxisX0; }
      set { _Graph_AxisX0 = value; }
    }

    private YAxisParam _Graph_AxisY0 = new YAxisParam(LiveCharts.AxisPosition.LeftBottom, true);
    [TypeConverter(typeof(YAxisParamConverter)),
     DisplayName("Yaxis 1"),
     CategoryAttribute("X/Y Axes"),
     DescriptionAttribute("Properties of the first Y axis, expand for more.")]

    public YAxisParam Graph_AxisY0
    {
      get { return _Graph_AxisY0; }
      set { _Graph_AxisY0 = value; }
    }

    private YAxisParam _Graph_AxisY1 = new YAxisParam(LiveCharts.AxisPosition.RightTop, false);
    [TypeConverter(typeof(YAxisParamConverter)),
     DisplayName("Yaxis 2"),
     CategoryAttribute("X/Y Axes"),
     DescriptionAttribute("Properties of the second Y axis, expand for more.")]

    public YAxisParam Graph_AxisY1
    {
      get { return _Graph_AxisY1; }
      set { _Graph_AxisY1 = value; }
    }


    private bool _Graph_showNavigator = false;
    [
     DisplayName("Enabled"),
     CategoryAttribute("Navigator"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Enable / disable the navigator, a small graph showing the whole data set below the main graph.")]
    public bool Graph_showNavigator
    {
      get { return _Graph_showNavigator; }
      set { _Graph_showNavigator = value; }
    }

    private bool _Graph_DisableAnimations = true;
    [
     DisplayName("Disbale Aninations"),
     CategoryAttribute("Graph"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Enable / disable the rendering animations. If you experience under responsiveness, disable animations.")]
    public bool Graph_DisableAnimations
    {
      get { return _Graph_DisableAnimations; }
      set { _Graph_DisableAnimations = value; }
    }





    private int _Graph_navigatorHeight = 100;
    [
     DisplayName("Height"),
     CategoryAttribute("Navigator"),
     DescriptionAttribute("Navigator height, in pixels.")]
    public int Graph_navigatorHeight
    {
      get { return _Graph_navigatorHeight; }
      set { _Graph_navigatorHeight = value; }
    }

    private Color _Graph_BackColor = Color.FromArgb(0xff, 0xfa, 0xfa, 0xfa);
    [
     DisplayName("Background Color"),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Graph background color.")]
    public Color Graph_BackColor
    {
      get { return _Graph_BackColor; }
      set { _Graph_BackColor = value; }
    }

    private bool _Graph_showRecordedData = true;
    [
     DisplayName("Use datalogger data"),
     TypeConverter(typeof(YesNoConverter)),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Makes the graph show the datalogger contents.")]
    public bool Graph_showRecordedData
    {
      get { return _Graph_showRecordedData; }
      set
      {
        if (_Graph_showRecordedData != value)
        {
          _Graph_showRecordedData = value;
          if (ownerForm != null)
            ((GraphForm)ownerForm).showRecordedDatachanged();
        }
      }
    }

    private Color _Graph2_BackColor = Color.FromArgb(0xff, 245, 245, 245);
    [
     DisplayName("Background Color"),
     CategoryAttribute("Navigator"),
     DescriptionAttribute("Navigator background color.")]
    public Color Graph2_BackColor
    {
      get { return _Graph2_BackColor; }
      set { _Graph2_BackColor = value; }
    }


    private Color _Graph2_ScrollBarFill = Color.FromArgb(0xff, 192, 192, 255);
    [
     DisplayName("Cursor Color"),
     CategoryAttribute("Navigator"),
     DescriptionAttribute("Navigator cursor color.")]
    public Color Graph2_ScrollBarFill
    {
      get { return _Graph2_ScrollBarFill; }
      set { _Graph2_ScrollBarFill = value; }
    }

    private XAxisBasicParam _Graph2_AxisX0 = new XAxisBasicParam();
    [TypeConverter(typeof(XAxisParamConverter)),
     DisplayName("Horzontal axis "),
     CategoryAttribute("Navigator"),
     DescriptionAttribute("Properties of navigator horzontal axis.")]
    public XAxisBasicParam Graph2_AxisX0
    {
      get { return _Graph2_AxisX0; }
      set { _Graph2_AxisX0 = value; }
    }



  }

}
