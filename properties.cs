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
 *    Graphs_series0  is the first series
 *    ....
 *    Graphs_series3  is the fourth series
 *    if you need a fifth series, just create a Graphs_series4 properties
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
using YDataRendering;
using YColors;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Globalization;

namespace YoctoVisualisation
{

  //****************************
  //  Solid gauge
  //****************************

  public static class GenericHints
  {
    public const string DevConfAffected = " Changing this value will affect the device configuration.";
    public const string CheckSensor = "If the sensor you want to use is connected, but not listed or listed as OFFLINE, check USB / Network configuration in the global configuration.";
    public const string AnnotationGraph = "Annotation text.  Use \\n for carriage returns. Some variables are available: $DAY$ $MONTH$ $YEAR$ for date, $HOUR$ $MINUTE$ $SECOND$ for time, $AVGVALUE1$ $MINVALUE1$ $MAXVALUE1$ $NAME1$ $UNIT1$  for first series data, $AVGVALUE2$ $MINVALUE2$ $MAXVALUE2$ $NAME2$ $UNIT2$  for second series data and so on";
    public const string Annotation = "Annotation text.  Use \\n for carriage returns. Some variables are available: $DAY$ $MONTH$ $YEAR$ for date, $HOUR$ $MINUTE$ $SECOND$ for time, $AVGVALUE$ $MINVALUE$ $MAXVALUE$ $NAME$ $UNIT$ for sensor related data.";       
  }

  public static class TypeDescription
  {
    public static string[] AllowedValues { get { return null; } }

  }



  public static class sensorFreqTypeDescription
  {
     static string[] _AllowedValues = new string[]   { "25/s", "10/s", "5/s", "4/s","3/s","2/s","1/s",
                                   "60/m","30/m","12/m","6/m","4/m","3/m","2/m","1/m",
                                   "30/h","12/h","6/h","4/h","3/h","2/h","1/h"};


     public static string[] AllowedValues { get { return _AllowedValues; } }

  }


  public static class yAxisDescription
  {
    static List<string> _AllowedValues = new List<string>();

    static yAxisDescription()
      {  
        int yaxiscount = 0;
        foreach (var p in typeof(GraphFormProperties).GetProperties())
          if (p.Name.StartsWith("Graph_yAxes")) yaxiscount++;
        for (int i=0;i< yaxiscount;i++)
          switch(i)
          {
            case 0: _AllowedValues.Add("1srt Y axis"); break;
            case 1: _AllowedValues.Add("2nd Y axis"); break;
            case 2: _AllowedValues.Add("3rd Y axis"); break;
            default: _AllowedValues.Add((i+1).ToString()+"th Y axis"); break;



        }

    }


    public static string[] AllowedValues { get { return _AllowedValues.ToArray(); } }

  }



  public static class AlarmTestTypeDescription
  {
    static string[] _AllowedValues = new string[] { "Disabled", ">", ">=", "=", "<=", "<" };
     public static string[] AllowedValues { get { return _AllowedValues; } }

  }

  public static class fontNameTypeDescription
  {
    static string[] _AllowedValues ;

    static  fontNameTypeDescription()
    {
      InstalledFontCollection _fontsCollection = new InstalledFontCollection();
      _AllowedValues = new string[_fontsCollection.Families.Length];
      int i = 0;
      foreach (FontFamily font in _fontsCollection.Families)
        _AllowedValues[i++]=font.Name;

    }

    public static string[] AllowedValues { get { return _AllowedValues; } }
  }




public static class sensorPrecisionTypeDescription
  {
    static string[] _AllowedValues = new string[] { "0", "0.1", "0.12", "0.123" };


    public static string[] AllowedValues { get { return _AllowedValues; } }

  }


  public static class sensorDataTypeDescription
  {
    static string[] _AllowedValues = new string[] { "Avg values", "Min values", "Max values" };

     
    public static string[] AllowedValues { get { return _AllowedValues; } }

  }






  public class GaugeFormProperties : GenericProperties
  {
    public GaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { PropagateDataSourceChange(); }

    private void PropagateDataSourceChange()
    {
      ((gaugeForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      ParamCategorySummaryAttribute("sensorDescription"),
      PreExpandedCategoryAttribute(true),
      ChangeCausesParentRefreshAttribute(true),
      DescriptionAttribute("Yoctopuce sensor feeding the gauge. ")]

    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();


      }
    }
   
    public string sensorDescription
    {
      get
      {
         return _DataSource_source is NullYSensor ? "none" : _DataSource_source.get_friendlyName();
      }
    }

    public bool isSensorReadOnly { get { return _DataSource_source.isReadOnly; } }

    [DisplayName("Sensor freq"),
     CategoryAttribute("Data source"),
     NotSavedInXMLAttribute(true),
     IsReadonlyCallAttribute("isSensorReadOnly"),
     ParamExtraDescription(typeof(sensorFreqTypeDescription)),
     DescriptionAttribute("Sensor data acquisition frequency."+ GenericHints.DevConfAffected)]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private string _DataSource_precision = "0.1";
    [
      DisplayName("Precision"),
      CategoryAttribute("Data source"),
      DescriptionAttribute("How many digits shown after the decimal point"),
      ParamExtraDescription(typeof(sensorPrecisionTypeDescription))]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }

    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
     ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),

     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
     ReadOnlyAttribute(true),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }




    private double _SolidGauge_min = 0;
    [DisplayName("Minimum value"),
     CategoryAttribute("Values range"),
     DescriptionAttribute("Minimum value displayable by the gauge.")]
    public double SolidGauge_min
    {
      get { return _SolidGauge_min; }
      set { _SolidGauge_min = value; }
    }

    private double _SolidGauge_max = 100;
    [DisplayName("Maximum value"),
     CategoryAttribute("Values range"),
     DescriptionAttribute("Maximum value displayable by the gauge.")]
    public double SolidGauge_max
    {
      get { return _SolidGauge_max; }
      set { _SolidGauge_max = value; }
    }

    private bool _SolidGauge_showMinMax = true;
    [DisplayName("show  min/max"),
     CategoryAttribute("Values range"),
     DescriptionAttribute("Show the min / max values.")]
    public bool SolidGauge_showMinMax
    {
      get { return _SolidGauge_showMinMax; }
      set { _SolidGauge_showMinMax = value; }
    }


    private YColor _SolidGauge_color1 = YColor.fromColor(Color.LightGreen);
    [DisplayName("Min Color"),
     CategoryAttribute("Values range"),
     
     DescriptionAttribute("Color for minimum value.")]
    public YColor SolidGauge_color1
    {
      get { return _SolidGauge_color1; }
      set { _SolidGauge_color1 = value; }
    }

    private YColor _SolidGauge_color2 = YColor.fromColor( Color.Red);
    [DisplayName("Max color"),
     CategoryAttribute("Values range"),
     
      DescriptionAttribute("Color for maximum value.")]
    public YColor SolidGauge_color2
    {
      get { return _SolidGauge_color2; }
      set { _SolidGauge_color2 = value; }
    }


    private FontDescription _SolidGauge_font = new FontDescription("Arial", 20, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Unit  Font"),
     CategoryAttribute("Fonts"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Font for displaying the value/status indicator")]
    public FontDescription SolidGauge_font
    {
      get { return _SolidGauge_font; }
      set { _SolidGauge_font = value; }
    }

    private FontDescription _SolidGauge_minMaxFont = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Min Max  Font"),
     CategoryAttribute("Fonts"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Font for displaying min/max values")]
    public FontDescription SolidGauge_minMaxFont
    {
      get { return _SolidGauge_minMaxFont; }
      set { _SolidGauge_minMaxFont = value; }
    }



    // ***************************************************************


    private YSolidGauge.DisplayMode _SolidGauge_displayMode = YSolidGauge.DisplayMode.DISPLAY90;
    [DisplayName("Display mode"),
    CategoryAttribute("Dial"),
    DescriptionAttribute("Dial general shape")]
    public YSolidGauge.DisplayMode SolidGauge_displayMode
    {
      get { return _SolidGauge_displayMode; }
      set { _SolidGauge_displayMode = value; }
    }





    private YColor _SolidGauge_borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     CategoryAttribute("Dial"),
     
     DescriptionAttribute("Dial border color." )]
    public YColor SolidGauge_borderColor
    {
      get { return _SolidGauge_borderColor; }
      set { _SolidGauge_borderColor = value; }
    }

    private double _SolidGauge_borderThickness = 2;
    [DisplayName("Border thickness "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial border")]
    public double SolidGauge_borderThickness
    {
      get { return _SolidGauge_borderThickness; }
      set { _SolidGauge_borderThickness = value; }
    }


    private YColor _SolidGauge_backgroundColor1 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 1"),
     CategoryAttribute("Dial"),
     
     DescriptionAttribute("Dial background gradient color 1." )]
    public YColor SolidGauge_backgroundColor1
    {
      get { return _SolidGauge_backgroundColor1; }
      set { _SolidGauge_backgroundColor1 = value; }
    }

    private YColor _SolidGauge_backgroundColor2 = YColor.FromArgb(255, 200, 200, 200);
    [DisplayName("Background color 2"),
     CategoryAttribute("Dial"),
    
     DescriptionAttribute("Dial background gradient color 2." )]
    public YColor SolidGauge_backgroundColor2
    {
      get { return _SolidGauge_backgroundColor2; }
      set { _SolidGauge_backgroundColor2 = value; }
    }


    private double _SolidGauge_thickness = 25;
    [DisplayName("Dial thickness (%) "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial, in percentage relative to radius")]
    public double SolidGauge_thickness
    {
      get { return _SolidGauge_thickness; }
      set { _SolidGauge_thickness = value; }
    }

    private double _SolidGauge_maxSpeed = 1;
    [DisplayName("Max speed (%) "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Maximum speed of the dial in percentage relative to Max-Min. This is meant to limit \"teleporting\" effects.")]
    public double SolidGauge_maxSpeed
    {
      get { return _SolidGauge_maxSpeed; }
      set { _SolidGauge_maxSpeed = value; }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }


    private AnnotationPanelDescription _annotationPanels0 = new AnnotationPanelDescription(
        GenericPanel.HorizontalAlignPos.CENTER, GenericPanel.VerticalAlignPos.BOTTOM,0, false,"$NAME$",
        YColor.FromArgb(0, 127, 127, 127), YColor.FromArgb(0, 127, 127, 127),
        10.0, YColor.FromArgb(255, 0, 0, 0));
    [
     DisplayName("Annotation 1"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription SolidGauge_annotationPanels0
    {
      get { return _annotationPanels0; }
      set { _annotationPanels0 = value; }
    }

    private AnnotationPanelDescription _annotationPanels1 = new AnnotationPanelDescription(
         GenericPanel.HorizontalAlignPos.LEFT, GenericPanel.VerticalAlignPos.TOP,0, true, "$HOUR$:$MINUTE$",
         YColor.FromArgb(255, 255, 255, 255), YColor.FromArgb(255, 0, 0, 0),
         8.0, YColor.FromArgb(255, 0, 0, 0));
    [
     DisplayName("Annotation 2"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription SolidGauge_annotationPanels1
    {
      get { return _annotationPanels1; }
      set { _annotationPanels1 = value; }
    }



  }

  //****************************
  //  angular gauge
  //****************************

  public class AngularGaugeFormProperties : GenericProperties
  {
    public AngularGaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { PropagateDataSourceChange();
    }

    public  void PropagateDataSourceChange()
    {
      ((angularGaugeForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      ParamCategorySummaryAttribute("sensorDescription"),
      PreExpandedCategoryAttribute(true),
      ChangeCausesParentRefreshAttribute(true),
      DescriptionAttribute("Yoctopuce sensor feeding the gauge. "+GenericHints.CheckSensor)]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();

      }
    }

    
    public string sensorDescription
    {
      get
      {
        return _DataSource_source is NullYSensor ? "none" : _DataSource_source.get_friendlyName();
      }
    }

    public bool isSensorReadOnly { get { return _DataSource_source.isReadOnly; } }


    [DisplayName("Sensor freq"),
    CategoryAttribute("Data source"),
    NotSavedInXMLAttribute(true),
    IsReadonlyCallAttribute("isSensorReadOnly"),
    DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected),
    ParamExtraDescription(typeof(sensorFreqTypeDescription))]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),

     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }


    private double _AngularGauge_min = 0;
    [DisplayName("Minimum value"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Minimum value displayable by the gauge.")]
    public double AngularGauge_min
    {
      get { return _AngularGauge_min; }
      set { _AngularGauge_min = value; }
    }

    private double _AngularGauge_max = 100;
    [DisplayName("Maximum value"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Maximum value displayable by the gauge.")]
    public double AngularGauge_max
    {
      get { return _AngularGauge_max; }
      set { _AngularGauge_max = value; }
    }


    private double _AngularGauge_unitFactor = 1;
    [DisplayName("Unit factor"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Data  will be divided by this value before being displayed, this allows simpler gradation marks.")]
     public double AngularGauge_unitFactor
    {
        get { return _AngularGauge_unitFactor; }
        set { _AngularGauge_unitFactor = value; }
      }

    private double _AngularGauge_graduation = 10;
    [DisplayName("Main gradation steps"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Difference between two consecutive main gradation marks")]
    public double AngularGauge_graduation
    {
      get { return _AngularGauge_graduation; }
      set { _AngularGauge_graduation = value; }
    }

  

  


    private double _AngularGauge_graduationSize = 10;
    [DisplayName("Main gradation size (%)"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Main gradation marks size in percent, relative to dial radius")]
    public double AngularGauge_graduationSize
    {
      get { return _AngularGauge_graduationSize; }
      set { _AngularGauge_graduationSize = value; }
    }

    private double _AngularGauge_graduationOuterRadiusSize = 98;
    [DisplayName("Main gradation radius (%)"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Main gradation marks outer radius in percent, relative to dial radius")]
    public double AngularGauge_graduationOuterRadiusSize
    {
      get { return _AngularGauge_graduationOuterRadiusSize; }
      set { _AngularGauge_graduationOuterRadiusSize = value; }
    }

  

    private double _AngularGauge_graduationThickness = 2;
    [DisplayName("Main gradation thickness"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Main gradation marks thickness")]
    public double AngularGauge_graduationThickness
    {
      get { return _AngularGauge_graduationThickness; }
      set { _AngularGauge_graduationThickness = value; }
    }


    private YColor _AngularGauge_graduationColor = YColor.fromColor(Color.Black);
    [DisplayName("Main gradation color"),
     CategoryAttribute("Gauge gradations"),
     
     DescriptionAttribute("Main gradation marks color." )]
    public YColor AngularGauge_graduationColor
    {
      get { return _AngularGauge_graduationColor; }
      set { _AngularGauge_graduationColor = value; }
    }

    private FontDescription _AngularGauge_graduationFont = new FontDescription("Arial", 20, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Main gradation font"),
     CategoryAttribute("Gauge gradations"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Font used for gradation labels")]
    public FontDescription AngularGauge_graduationFont
    {
      get { return _AngularGauge_graduationFont; }
      set { _AngularGauge_graduationFont = value; }
    }

    private int _AngularGauge_subgraduationCount = 5;
    [DisplayName("Sub-gradation count"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("How many sub-gradation (+1) marks between two consecutive main graduation marks")]
    public int AngularGauge_subgraduationCount
    {
      get { return _AngularGauge_subgraduationCount; }
      set { _AngularGauge_subgraduationCount = value; }
    }

    private double _AngularGauge_subgraduationSize = 5;
    [DisplayName("Sub-gradation size (%)"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Sub-gradation marks size in percent, relative to dial radius")]
    public double AngularGauge_subgraduationSize
    {
      get { return _AngularGauge_subgraduationSize; }
      set { _AngularGauge_subgraduationSize = value; }
    }


    private double _AngularGauge_subgraduationThickness = 1;
    [DisplayName("Sub-gradation thickness"),
     CategoryAttribute("Gauge gradations"),
     DescriptionAttribute("Sub-gradation marks thickness")]
    public double AngularGauge_subgraduationThickness
    {
      get { return _AngularGauge_subgraduationThickness; }
      set { _AngularGauge_subgraduationThickness = value; }
    }


    private YColor _AngularGauge_subgraduationColor = YColor.fromColor(Color.Black);
    [DisplayName("Sub-gradation color"),
     CategoryAttribute("Gauge gradations"),
    
     DescriptionAttribute("Sub-gradation marks color." )]
    public YColor AngularGauge_subgraduationColor
    {
      get { return _AngularGauge_subgraduationColor; }
      set { _AngularGauge_subgraduationColor = value; }
    }



    private YColor _AngularGauge_needleColor = YColor.fromColor(Color.Red);
    [DisplayName("Needle color"),
     CategoryAttribute("Needle"),
     
     DescriptionAttribute("Needle filling color." )]
    public YColor AngularGauge_needleColor
    {
      get { return _AngularGauge_needleColor; }
      set { _AngularGauge_needleColor = value; }
    }

    private YColor _AngularGauge_needleContourColor = YColor.fromColor(Color.DarkRed);
    [DisplayName("Needle contour color"),
     CategoryAttribute("Needle"),
     
     DescriptionAttribute("Needle contour color." )]
     public YColor AngularGauge_needleContourColor
     {
       get { return _AngularGauge_needleContourColor; }
       set { _AngularGauge_needleContourColor = value; }
     }

    private double _AngularGauge_needleContourThickness = 1;
    [DisplayName("Needle contour thickness"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Thickness of the needle contour")]
     public double AngularGauge_needleContourThickness
     {
       get { return _AngularGauge_needleContourThickness; }
       set { _AngularGauge_needleContourThickness = value; }
     }




    private double _AngularGauge_needleLength1 = 90;
    [DisplayName("Needle main size (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Length of the needle part pointing to gradations, in % relative to radius")]
    public double AngularGauge_needleLength1
    {
      get { return _AngularGauge_needleLength1; }
      set { _AngularGauge_needleLength1 = value; }
    }

    private double _AngularGauge_needleLength2 = 15;
    [DisplayName("Needle foot size (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Length of the needle part not pointing to gradations, in % relative to radius")]
     public double AngularGauge_needleLength2
      {
        get { return _AngularGauge_needleLength2; }
        set { _AngularGauge_needleLength2 = value; }
     }

    private double _AngularGauge_needleWidth = 5;
    [DisplayName("Needle width (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Width of the needle, in % relative to radius")]
    public double AngularGauge_needleWidth
    {
      get { return _AngularGauge_needleWidth; }
      set { _AngularGauge_needleWidth = value; }
    }

    private double _AngularGauge_needleMaxSpeed = 0.5;
    [DisplayName("Needle max speed (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Needle Maximum speed, in % relative to (max-min). This is meant to limit \"teleporting\" effects.")]
    public double AngularGauge_needleMaxSpeed
    {
      get { return _AngularGauge_needleMaxSpeed; }
      set { _AngularGauge_needleMaxSpeed = value; }
    }

   


    private FontDescription _AngularGauge_unitFont = new FontDescription("Arial", 24, YColor.fromColor(Color.DarkGray), false, true);
    [DisplayName("Unit Line Font"),
     CategoryAttribute("Text lines"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Font used in the text line describing unit")]
    public FontDescription AngularGauge_unitFont
    {
      get { return _AngularGauge_unitFont; }
      set { _AngularGauge_unitFont = value; }
    }

   private FontDescription _AngularGauge_statusFont = new FontDescription("Arial", 24, YColor.fromColor(Color.DarkGray), false, true);
    [DisplayName("Status Line Font"),
      ReadOnlyAttribute(true),
     CategoryAttribute("Text lines"),
     DescriptionAttribute("Font used in the text line gauge status")]
    public FontDescription AngularGauge_statusFont
    {
      get { return _AngularGauge_statusFont; }
      set { _AngularGauge_statusFont = value; }
    }



    private YColor _AngularGauge_borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     CategoryAttribute("Dial"),
    
     DescriptionAttribute("Dial border color." )]
    public YColor AngularGauge_borderColor
    {
      get { return _AngularGauge_borderColor; }
      set { _AngularGauge_borderColor = value; }
    }

    private double _AngularGauge_borderThickness = 5;
    [DisplayName("Border thickness "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial border")]
     public double AngularGauge_borderThickness
     {
       get { return _AngularGauge_borderThickness; }
       set { _AngularGauge_borderThickness = value; }
     }


    private YColor _AngularGauge_backgroundColor1 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 1"),
     CategoryAttribute("Dial"),
     
     DescriptionAttribute("Dial background gradient color 1." )]
     public YColor AngularGauge_backgroundColor1
     {
       get { return _AngularGauge_backgroundColor1; }
       set { _AngularGauge_backgroundColor1 = value; }
    }

    private YColor _AngularGauge_backgroundColor2 = YColor.FromArgb(255, 200, 200, 200);
    [DisplayName("Background color 2"),
     CategoryAttribute("Dial"),
     
     DescriptionAttribute("Dial background gradient color 2." )]
     public YColor AngularGauge_backgroundColor2
     {
       get { return _AngularGauge_backgroundColor2; }
       set { _AngularGauge_backgroundColor2 = value; }
     }


    private AngularZoneDescription _AngularGauge_zones0 = new AngularZoneDescription(0, 50, YColor.fromColor(Color.LightGreen));
    [DisplayName("Zone 1"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    DescriptionAttribute("Zone 1 parameters")]
    public AngularZoneDescription AngularGauge_zones0
    {
      get { return _AngularGauge_zones0; }
      set { _AngularGauge_zones0 = value; }
    }

    private AngularZoneDescription _AngularGauge_zones1 = new AngularZoneDescription(50, 80, YColor.fromColor(Color.Yellow));
    [DisplayName("Zone 2"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    DescriptionAttribute("Zone 2 parameters")]
    public AngularZoneDescription AngularGauge_zones1
    {
      get { return _AngularGauge_zones1; }
      set { _AngularGauge_zones1 = value; }
    }

    private AngularZoneDescription _AngularGauge_zones2 = new AngularZoneDescription(80, 100, YColor.fromColor(Color.Red));
    [DisplayName("Zone 3"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    DescriptionAttribute("Zone 3 parameters")]
    public AngularZoneDescription AngularGauge_zones2
    {
      get { return _AngularGauge_zones2; }
      set { _AngularGauge_zones2 = value; }
    }

    private AnnotationPanelDescription _annotationPanels0 = new AnnotationPanelDescription(
       GenericPanel.HorizontalAlignPos.CENTER, GenericPanel.VerticalAlignPos.CENTER,-15, true, "$NAME$",
       YColor.FromArgb(0, 127, 127, 127), YColor.FromArgb(0, 127, 127, 127),
       8.0, YColor.FromArgb(255, 127, 127, 127));
    [
     DisplayName("Annotation 1"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription AngularGauge_annotationPanels0
    {
      get { return _annotationPanels0; }
      set { _annotationPanels0 = value; }
    }

    private AnnotationPanelDescription _annotationPanels1 = new AnnotationPanelDescription(
         GenericPanel.HorizontalAlignPos.CENTER, GenericPanel.VerticalAlignPos.BOTTOM,0, false, "$HOUR$:$MINUTE$",
         YColor.FromArgb(255, 255, 255, 255), YColor.FromArgb(255, 127, 127, 127),
         12.0, YColor.FromArgb(255, 0, 0, 0));
    [
     DisplayName("Annotation 2"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription AngularGauge_annotationPanels1
    {
      get { return _annotationPanels1; }
      set { _annotationPanels1 = value; }
    }


  }

  public class ZoneDescription
  {

   
    double _min;
    double _max;
    bool _visible = false;
    YColor _color;

    public ZoneDescription(double min, double max, YColor color)
    {
      _min = min;
      _max = max;
      _color = color;

    }

    public string summary
    {
      get { return _visible ?  _min.ToString()+".."+ _max.ToString() : "Disabled"; }
    }

    [DisplayName("Visible"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone visibility."  )]
    public bool visible { get { return _visible; } set { _visible = value; } }


    [DisplayName("Minimum value"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone minimum value")]
    public double min { get { return _min; } set { _min = value; } }

    [DisplayName("Maximum value"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone maximum value")]
    public double max { get { return _max; } set { _max = value; } }


    [DisplayName("Color"),
    
   
    DescriptionAttribute("Zone color")]
    public YColor color { get { return _color; } set { _color = value; } }
  }

  public class AngularZoneDescription
  {

    double _width = 5;
    double _outerRadius =98;
    double _min;
    double _max;
    bool _visible = true;
    YColor _color;

    public string summary
    {
      get { return _visible ?  _min.ToString() + ".." + _max.ToString()  : "Disabled"; }
    }

    public AngularZoneDescription(double min, double max, YColor color)
    {
      _min = min;
      _max = max;
      _color = color;

    }


    [DisplayName("Visible"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone visibility."  )]
     public bool visible { get { return _visible; } set { _visible = value; } }


    [DisplayName("Minimum value"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone minimum value")]
     public double min { get { return _min; } set { _min = value; } }

    [DisplayName("Maximum value"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Zone maximum value")]
     public double max { get { return _max; } set { _max = value; } }


    [DisplayName("Color"),
    
   
    DescriptionAttribute("Zone color")]
    public YColor color { get { return _color; } set { _color = value; } }

    [DisplayName("Outer radius (%)"),
     
     DescriptionAttribute("Zone outer radius, in percentage relative to dial radius ")]
     public double outerRadius { get { return _outerRadius; } set { _outerRadius = value; } }

    [DisplayName("Width (%)"),
     
     DescriptionAttribute("Zone  width, in percentage relative to dial radius ")]
     public double width { get { return _width; } set { _width = value; } }
  }


  //****************************
  //  Fonts
  //****************************



  public class FontDescription
  {
    public FontDescription(string name, double size, YColor color, bool italic, bool bold)
    {
      _name = name;
      _size = size;
      _color = color;
      _italic = italic;
      _bold = bold;

    }

    public override string ToString()
    {
      return _name + " " + _size.ToString();
    }

    public string summary
     {
      get { return _name + " " + _size.ToString(); }
     }

    private string _name;
    [
     DisplayName("Font name"),
     ParamExtraDescription(typeof(fontNameTypeDescription)),
      ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Name of the font")]
    public string name { get { return _name; } set { _name = value; } }

    private double _size;
    [DisplayName("Font size"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Size of the font")]
    public double size { get { return _size; } set { _size = value; } }

    private YColor _color;
    [DisplayName("Font color"),
    
    
     DescriptionAttribute("Color of the font." )]
    public YColor color { get { return _color; } set { _color = value; } }

    private bool _italic;
    [DisplayName("Italic"),
     
     DescriptionAttribute("Is the font style italic?."  )]
    public bool italic { get { return _italic; } set { _italic = value; } }


    private bool _bold;
    [DisplayName("Bold"),
     
     DescriptionAttribute("Is the font style bold?."  )]
    public bool bold { get { return _bold; } set { _bold = value; } }


  }

  //****************************
  //  Digital display
  //****************************




  public class digitalDisplayFormProperties : GenericProperties
  {
    public digitalDisplayFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    {
      PropagateDataSourceChange();

    }

    private void PropagateDataSourceChange()
    {
      ((digitalDisplayForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        { 
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
     
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      ParamCategorySummaryAttribute("sensorDescription"),
      PreExpandedCategoryAttribute(true),
      ChangeCausesParentRefreshAttribute(true),
      DescriptionAttribute("Yoctopuce sensor feeding the display. " + GenericHints.CheckSensor)]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();

      }
    }

    public bool isSensorReadOnly { get { return _DataSource_source.isReadOnly; } }

   
    public string sensorDescription
    {
      get
      {
        return _DataSource_source is NullYSensor ? "none" : _DataSource_source.get_friendlyName();
      }
    }

    [DisplayName("Sensor freq"),
     CategoryAttribute("Data source"),
     NotSavedInXMLAttribute(true),
     IsReadonlyCallAttribute("isSensorReadOnly"),
     DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected),
     ParamExtraDescription(typeof(sensorFreqTypeDescription))] 
    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    

    private string _DataSource_precision = "0.1";
    [
      DisplayName("Precision"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("How many digits shown after the decimal point."),
     ParamExtraDescription(typeof(sensorPrecisionTypeDescription)) ]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }
    
    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),
    
     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }

   

    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
      ReadOnlyAttribute(true),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }



    private FontDescription _font = new FontDescription("Arial", 48, YColor.fromColor(Color.LightGreen),false,true);

    [DisplayName("Font"),
     CategoryAttribute("Display"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Display font")]
    public FontDescription display_font
    {
      get { return _font; }
      set { _font = value; }
    }

    private YColor _backgroundColor1 = YColor.fromColor(Color.Black);
    [DisplayName("Background color 1"),
     CategoryAttribute("Display"),
     
     DescriptionAttribute("Display background gradient color1." )]
     public YColor display_backgroundColor1
    {
       get { return _backgroundColor1; }
       set { _backgroundColor1 = value; }
     }

    private YColor _backgroundColor2 = YColor.fromColor(Color.Black);
    [DisplayName("Background color 2"),
     CategoryAttribute("Display"),
     
     DescriptionAttribute("Display background gradient color 2." )]
    public YColor display_backgroundColor2
    {
      get { return _backgroundColor2; }
      set { _backgroundColor2 = value; }
    }



    private YDigitalDisplay.HrzAlignment _hrzAlignment = YDigitalDisplay.HrzAlignment.RIGHT;
    [DisplayName("Hrz alignment method"),
    CategoryAttribute("Display"),
     
    DescriptionAttribute("Horizontal alignment method")]
    public YDigitalDisplay.HrzAlignment display_hrzAlignment
    {
      get { return _hrzAlignment; }
      set { _hrzAlignment = value; }
    }

    private Double _hrzAlignmentOfset = 5.0;
    [DisplayName("Hrz alignment offset"),
    CategoryAttribute("Display"),
    DescriptionAttribute("Horizontal alignment offset in percentage. No effect when chosen horizontal alignment is CENTER")]
    public double display_hrzAlignmentOfset
    {
      get { return _hrzAlignmentOfset; }
      set { _hrzAlignmentOfset = value; }
    }


    private doubleNan _outOfRangeMin = new doubleNan(Double.NaN);
    [DisplayName("Minimum value"),
     CategoryAttribute("Range Control"),
     DescriptionAttribute("Regular range minimum value. if value goes  outside regular  range, color will turn to \"Out of range Color\".Leave blank if you don't want to define such a range. ")]
    public doubleNan display_outOfRangeMin
    {
      get { return _outOfRangeMin; }
      set { _outOfRangeMin = value; }
    }

    private doubleNan _outOfRangeMax = new doubleNan(Double.NaN);
    [DisplayName("Maximum value"),
     CategoryAttribute("Range Control"),
     DescriptionAttribute("Regular range maximum value. if value is outside regular range, color will turn to \"Out of range Color\". Leave blank if you don't want to define such a range. ")]
    public doubleNan display_outOfRangeMax
    {
      get { return _outOfRangeMax; }
      set {_outOfRangeMax = value; }
    }


    private YColor _outOfRangeColor = YColor.fromColor(Color.Red);
    [DisplayName("Out of range Color"),
     CategoryAttribute("Range Control"),
     
     DescriptionAttribute("Digits color when value is out of range." )]
    public YColor display_outOfRangeColor
    {
      get { return _outOfRangeColor; }
      set { _outOfRangeColor = value; }
    }

    private AnnotationPanelDescription _annotationPanels0 =  new AnnotationPanelDescription(
         GenericPanel.HorizontalAlignPos.CENTER, GenericPanel.VerticalAlignPos.BOTTOM,0, false, "$NAME$",
         YColor.FromArgb(0, 127, 127, 127), YColor.FromArgb(0, 127, 127, 127),
         10.0, YColor.FromArgb(255, 128, 255, 128));
    [
     DisplayName("Annotation 1"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription display_annotationPanels0
    {
      get { return _annotationPanels0; }
      set { _annotationPanels0 = value; }
    }

    private AnnotationPanelDescription _annotationPanels1 = new AnnotationPanelDescription(
         GenericPanel.HorizontalAlignPos.CENTER, GenericPanel.VerticalAlignPos.TOP,0, false, "$HOUR$:$MINUTE$",
         YColor.FromArgb(0, 127, 127, 127), YColor.FromArgb(0, 127, 127, 127),
         8.0, YColor.FromArgb(255, 128, 255, 128));
    [
     DisplayName("Annotation 2"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panels")]
    public AnnotationPanelDescription display_annotationPanels1
    {
      get { return _annotationPanels1; }
      set { _annotationPanels1 = value; }
    }


  }






  /****************************************
   * Alarm section
   */


  public class AlarmSection
  {

    
    CustomYSensor _sensor = sensorsManager.getNullSensor();
    int _index = 0;

    public string summary
    { get {

        int c = _sensor.getAlarmCondition(_index);
        if (c == 0) return "Disabled";
        return "Enabled";
         
      }

    }


    public AlarmSection(int index)
    {
      _index = index;
    }

    [DisplayName("Data source type"),
   
    NotSavedInXMLAttribute(true),
    ParamExtraDescription(typeof(sensorDataTypeDescription)),
    DescriptionAttribute("Alarm sensor data source (Average, minimum or maximum value during last interval)")]
    public int source
    {
      get
      {
        return _sensor.getAlarmSource(_index);
      }
      set
      {
        _sensor.setAlarmSource(_index, value);
      }
    }


    [DisplayName("Test Condition"),
     NotSavedInXMLAttribute(true),
      ParamExtraDescription(typeof(AlarmTestTypeDescription)),
       ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Alarm trigger condition")]
    public int condition
    {
      get {
        return _sensor.getAlarmCondition(_index); 
      }
      set {
        _sensor.setAlarmCondition(_index,value); }
    }

    public void setDataSource(CustomYSensor sensor)
    {
      _sensor = sensor;
    }

   
    [DisplayName("Test Value"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("Value for the Alarm trigger")]
    public double value
    {
      get {
        return _sensor.getAlarmValue(_index);
      }
      set {
        _sensor.setAlarmValue(_index, value); }
    }

   
    [DisplayName("Trigger action"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("System command line executed each time the alarm is triggered, you can use the following variables: $SENSORVALUE$, $UNIT$, $HWDID$, $NAME$, $CONDITION$, $TRIGGER$, $DATATYPE$, $NOW$. You can check logs to find out if your alarm command line works.")]
    public string commandLine
    {
      get {
         return _sensor.getAlarmCommandline(_index); 
      }
      set { _sensor.setAlarmCommandline(_index, value); }
    }

   
    [DisplayName("Trigger delay"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("Minimum delay, in seconds, between two alarms. Think carefully and make sure you won't create alarm storms.")]
    public int delay
    {
      get {
         return _sensor.getAlarmDelay(_index); 
      }
      set { _sensor.setAlarmDelay(_index,value); }
    }
  }








  /*********************************
   * SERIES
   */

  public class ChartSerie
  {

    private Form ownerForm = null;
    private int index = -1;

    public ChartSerie(YColor defaultColor)
    {
      _color = defaultColor;

    }

    public string summary
    { get { return _DataSource_source is NullYSensor ? "none" : _DataSource_source.get_friendlyName(); }
    }

    public void Init(Form owner, int serieIndex)
    {
      ownerForm = owner;
      index = serieIndex;
      PropagateDataSourceChange(_DataSource_source);
    }

    private void PropagateDataSourceChange(CustomYSensor value)
    {
      ((GraphForm)ownerForm).SourceChanged(index, value);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }



    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [
      DisplayName("Sensor"),
      ChangeCausesParentRefreshAttribute(true),
      DescriptionAttribute("Yoctopuce sensor feeding the graph. " + GenericHints.CheckSensor)]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        if (ownerForm != null) PropagateDataSourceChange(_DataSource_source);


      }
    }

    public bool  isSensorReadOnly
      { get{ return _DataSource_source.isReadOnly; } 
      }


    [DisplayName("Sensor frequency"),
     NotSavedInXMLAttribute(true),
     IsReadonlyCallAttribute("isSensorReadOnly"),
     DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected),
     ParamExtraDescription(typeof(sensorFreqTypeDescription))]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    private int _DataType = 0;
    [DisplayName("Sensor data"),
     ParamExtraDescription(typeof(sensorDataTypeDescription)),
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
     NotSavedInXMLAttribute(true),
     IsReadonlyCallAttribute("isSensorReadOnly"),
     DescriptionAttribute("Enable/ disable sensor data recording in the device on-board datalogger."+  GenericHints.DevConfAffected  )]

    public bool DataSource_recording
    {
      get { return _DataSource_source.get_recording(); }
      set { _DataSource_source.set_recording(value); }

    }

    private double _thickness = 2.0;
    [DisplayName("Thickness"),
     DescriptionAttribute("Line thickness.")]
    public double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private string _legend = "";
    [DisplayName("Legend"),
     DescriptionAttribute("Short description of the series.")]
    public string legend
    {
      get { return _legend; }
      set { _legend = value; }
    }

    private YColor _color = YColor.fromColor(Color.Red);

    [DisplayName("Color"),
    
     DescriptionAttribute("Line color." )]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private int _yAxisIndex = 0;
    [
      ParamExtraDescription(typeof(yAxisDescription)),
      DisplayName("Y axis"),
      DescriptionAttribute("Choose which Y axis the data with be scaled to.")]
    public int yAxisIndex
    {
      get { return _yAxisIndex; }
      set { _yAxisIndex = value; }
    }

    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [
     DisplayName("Sensor value alarm 2"),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }


  }

  //****************************
  //  Legend panel
  //  
  //****************************

  public class LegendPanelDescription
  {

    public string summary
    {
      get { return _enabled ? "Enabled" : "Disabled"; }
    }

    private bool _enabled = false;
    [DisplayName("Enabled"),
      ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should the legend panel be shown or not" )]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }

    private bool _overlap = false;
    [DisplayName("Overlap"),
     DescriptionAttribute("Can the panel overlap the graph data, or should we explicitly make space for it?"  )]
    public bool overlap { get { return _overlap; } set { _overlap = value; } }

    private LegendPanel.Position _position = LegendPanel.Position.RIGHT;
    [DisplayName("Position"),
     DescriptionAttribute("Position of the legend panel")]
     public LegendPanel.Position position { get { return _position; } set { _position = value; } }

    private FontDescription _font = new FontDescription("Arial", 7, YColor.FromArgb(255, 32, 32, 32), false, false);
    [DisplayName("Font"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Legend panel contents fonts")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }



    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 255);
    [DisplayName("Background color "),    
    DescriptionAttribute("Legend panel background color." )]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value;  } }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color "),    
    DescriptionAttribute("Legend panel border color." )]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value; } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
     DescriptionAttribute("Legend panel border thickness")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 10;
    [DisplayName("Padding "),
     DescriptionAttribute("Distance between the panel border and the panel contents")]
    public double padding { get { return _padding; } set { _padding = value; } }

    private double _verticalMargin = 10;
    [DisplayName("Vertical margin "),
     DescriptionAttribute("Vertical distance between the panel border and its surroundings")]
    public double verticalMargin { get { return _verticalMargin; } set { _verticalMargin = value; } }

    private double _horizontalMargin = 10;
    [DisplayName("Horizontal margin "),
    DescriptionAttribute("Distance between the panel border and its surroundings")]
    public double horizontalMargin { get { return _horizontalMargin; } set { _horizontalMargin = value; } }

    private double _traceWidthFactor = 1;
    [DisplayName("Color Indicator Factor"),
    DescriptionAttribute("Factor used to enlarge series color indicators shown in the legend panel")]
    public double traceWidthFactor { get { return _traceWidthFactor; } set { _traceWidthFactor = value; } }

  }



  public class AnnotationPanelDescription 
  {

    public string summary
    {
      get { return _enabled ? "Enabled" : "Disabled"; }
    }

    protected bool _enabled = false;
    [DisplayName("Enabled"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should the annotation panel be shown or not")]
     public virtual bool enabled { get { return _enabled; } set { _enabled = value; } }

    protected  bool _overlap = false;
    [DisplayName("Overlap"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Can the annotation panel overlap the display zone, or should the display zone be squeezed to make space for the panel?")]
     public virtual bool overlap { get { return _overlap; } set { _overlap = value; } }

    protected String _text = "Date: $DAY$/$MONTH$/$YEAR$";
    [DisplayName("Text "),
     DescriptionAttribute(GenericHints.Annotation)]
     public virtual string text { get { return _text; } set { _text = value; } }

    
    protected GenericPanel.TextAlign _panelTextAlign = GenericPanel.TextAlign.CENTER;
    [DisplayName("Text Alignment"),
     DescriptionAttribute("How text is aligned, makes sense on multi-lines text only.")]
    public GenericPanel.TextAlign panelTextAlign { get { return _panelTextAlign; } set { _panelTextAlign = value;  } }


    private GenericPanel.HorizontalAlignPos _panelHrzAlign = GenericPanel.HorizontalAlignPos.CENTER;
    [DisplayName("X Position"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Annotation panel X position")]
     public GenericPanel.HorizontalAlignPos panelHrzAlign { get { return _panelHrzAlign; } set { _panelHrzAlign = value; } }

    private double _positionOffsetX = 0;
    [DisplayName("X Offset"),
     DescriptionAttribute("X Position offset, in %  (overlap mode only)")]
     public double positionOffsetX { get { return _positionOffsetX; } set { _positionOffsetX = value; } }

    private GenericPanel.VerticalAlignPos _panelVrtAlign = GenericPanel.VerticalAlignPos.TOP;
    [DisplayName("Y Position"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Annotation panel Y position")]
     public GenericPanel.VerticalAlignPos panelVrtAlign { get { return _panelVrtAlign; } set { _panelVrtAlign = value; } }

    private double _positionOffsetY = 0;
    [DisplayName("Y Offset"),
     DescriptionAttribute("Y Position offset, in %  (overlap mode only)")]
     public double positionOffsetY { get { return _positionOffsetY; } set { _positionOffsetY = value; } }

    private FontDescription _font = new FontDescription("Arial", 12, YColor.FromArgb(255, 32, 32, 32), false, false);
    [DisplayName("Font"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Panel font")]
     public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 255);
    [DisplayName("Background color "),
    DescriptionAttribute("Legend panel background color." )]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value; } }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color "),

    DescriptionAttribute("Legend panel border color." )]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value; } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
     DescriptionAttribute("Panel border thickness")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 5;
    [DisplayName("Padding "),
     DescriptionAttribute("Distance between the panel border and the panel contents")]
    public double padding { get { return _padding; } set { _padding = value; } }

   

    public  AnnotationPanelDescription()
    { }

   

    public  AnnotationPanelDescription(GenericPanel.HorizontalAlignPos hrzAlignInit,GenericPanel.VerticalAlignPos vrtAlignInit , double offsetY, bool overlap, string textInit, YColor BgColorInit, YColor BorderColorInit,  double fontSizeInit, YColor FontColorInit )
    { // allows alternate default value
      _text = textInit;
      _positionOffsetY = offsetY;
      _overlap = overlap;
      _bgColor = BgColorInit;
      _borderColor = BorderColorInit;
      _font.size = fontSizeInit;
      _font.color = FontColorInit;
      _panelVrtAlign = vrtAlignInit;
      _panelHrzAlign = hrzAlignInit;
    }

  }

  public class AnnotationPanelDescriptionGraph : AnnotationPanelDescription
  { // overrided  propoerty will be moved back in first 
    // position, 

    [DisplayName("Enabled"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should the annotation panel be shown or not")]

    public override bool enabled { get { return _enabled; }  set { _enabled = value; } }
    [DisplayName("Overlap"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Can the annotation panel overlap the graph, or should the graph be squeezed to make space for the panel?")]

    public override bool overlap { get { return _overlap; } set { _overlap = value; } }
    [DisplayName("Text "), 
     DescriptionAttribute(GenericHints.AnnotationGraph)]
    public override string text { get { return _text; } set { _text = value; } }
  }

  public class DataTrackerDescription
  {

    public string summary
    {
      get { return _enabled ? "Enabled" : "Disabled"; }
    }


    private bool _enabled = false;
    [DisplayName("Enabled"),
      ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should the data tracker be shown or not."  )]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }

    private bool _showSerieName = false;
    [DisplayName("Show Series legend"),
     DescriptionAttribute("Should the data tracker show the value's series legend.")]
     public bool showSerieName { get { return _showSerieName; } set { _showSerieName = value; } }

    private bool _showTimeStamp = false;
    [DisplayName("Show Timestamp"),
     DescriptionAttribute("Should the data tracker show the value's timestamp.")]
    public bool showTimeStamp { get { return _showTimeStamp; } set { _showTimeStamp = value; } }


    public enum DataPrecision
    {
      [Description("Sensor precision")]
      PRECISION_NOLIMIT = DataTracker.DataPrecision.PRECISION_NOLIMIT,      
      [Description("1")]
      PRECISION_1 = DataTracker.DataPrecision.PRECISION_1,
      [Description("0.1")]
      PRECISION_01 = DataTracker.DataPrecision.PRECISION_01,
      [Description("0.01")]
      PRECISION_001 = DataTracker.DataPrecision.PRECISION_001,
      [Description("0.001")]
      PRECISION_0001 = DataTracker.DataPrecision.PRECISION_0001,

    };

   DataPrecision _dataPrecision = DataPrecision.PRECISION_NOLIMIT;
    [DisplayName("Precision"),
     DescriptionAttribute("A way to limit data precision to meaningful digits in the data tracker.")]
     public  DataPrecision dataPrecision { get { return _dataPrecision; } set { _dataPrecision = value; } }

    private double _diameter = 5;
    [DisplayName("Point Diameter"),
    DescriptionAttribute("Data point diameter, in pixels")]
    public double diameter { get { return _diameter; } set { _diameter = value; } }

    private double _handleLength = 25;
    [DisplayName("Handle size"),
    DescriptionAttribute("size of the handle between the data point and the value panel")]
    public double handleLength { get { return _handleLength; } set { _handleLength = value; } }

    private FontDescription _font = new FontDescription("Arial", 7, YColor.FromArgb(255, 32, 32, 32), false, false);
    [DisplayName("Font"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Data tracker label fonts")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }



    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 255);
    [DisplayName("Background color"),
     
     DescriptionAttribute("Value panel ground color." )]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value;  } }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     
    DescriptionAttribute("Value panel border and handle  color." )]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value;  } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
    DescriptionAttribute("Value panel border and handle  thickness")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 5;
    [DisplayName("Padding"),
     DescriptionAttribute("Distance between the panel border and its contents ")]
    public double padding { get { return _padding; } set { _padding = value;  } }

    private int _detectionDistance = 50;
    [DisplayName("Detection distance"),
     DescriptionAttribute("Maximum distance, in pixels, between the mouse and a data point for data tracker to show. Use zero for infinite distance")]
    public int detectionDistance { get { return _detectionDistance; } set { _detectionDistance = value; } }




  }


  //****************************
  //  graph navigator
  //  
  //****************************

  public class NavigatorDescription
  {

    public string summary
    {
      get { return _enabled ? "Enabled" : "Disabled"; }
    }


    public NavigatorDescription() {}

    private bool _enabled = false;
    [DisplayName("Enabled"),
      ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should the navigator be shown or not."  )]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }


    private YColor _bgColor1 = YColor.FromArgb(255, 225, 225, 225);
    [DisplayName("Background color 1"),
     
     DescriptionAttribute("Navigator background gradient color 1." )]
    public YColor bgColor1
    {
      get { return _bgColor1; }
      set { _bgColor1 = value; }
    }


    private YColor _bgColor2 = YColor.FromArgb(255, 225, 225, 225);
    [DisplayName("Background color 2"),
     
     DescriptionAttribute("Navigator background gradient color 2." )]
    public YColor bgColor2
    {
      get { return _bgColor2; }
      set { _bgColor2 = value; }
    }

    private Double _borderThickness = 1.0;
    [DisplayName("Border thickness"),
     DescriptionAttribute("Navigator")]
    public Double borderThickness
    {
      get { return _borderThickness; }
      set { _borderThickness = value; }
    }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     
     DescriptionAttribute("Navigator Border color." )]
    public YColor borderColor
    {
      get { return _borderColor; }
      set { _borderColor = value; }
    }

    private YColor _cursorColor = YColor.FromArgb(25, 0, 255, 0);
    [DisplayName("Navigator cursor fill color"),
      
     DescriptionAttribute("Navigator")]
    public YColor cursorColor
    {
      get { return _cursorColor; }
      set { _cursorColor = value; }
    }

    private YColor _cursorBorderColor = YColor.FromArgb(255, 96, 96, 96);
    [DisplayName("Navigator cursor left/right border color." ),
    
     DescriptionAttribute("Navigator")]
    public YColor cursorBorderColor
    {
      get { return _cursorBorderColor; }
      set { _cursorBorderColor = value; }
    }


    private Navigator.YAxisHandling _yAxisHandling = Navigator.YAxisHandling.AUTO;
    [DisplayName("Y axis range"),
     DescriptionAttribute("Is navigator Y axis zoom automatic or inherited from main view settings?")]
    public Navigator.YAxisHandling yAxisHandling
    {
      get { return _yAxisHandling; }
      set { _yAxisHandling = value; }
    }

    private Double _xAxisThickness = 1.0;
    [DisplayName("X axis thickness"),
     DescriptionAttribute("Navigator")]
     public Double xAxisThickness
    {
      get { return _xAxisThickness; }
      set { _xAxisThickness = value; }
    }

    private YColor _xAxisColor = YColor.fromColor(Color.Black);
    [DisplayName("X axis color"),
     
     DescriptionAttribute("Navigator X axis color." )]
     public YColor xAxisColor
    {
      get { return _xAxisColor; }
      set { _xAxisColor = value; }
    }

 

   

    private FontDescription _font = new FontDescription("Arial",7, YColor.FromArgb(255,32,32,32), false, false);
    [DisplayName("X-Axis Font"),
     
     ReadOnlyAttribute(true),
     DescriptionAttribute("Navigator X axis font")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

  
  }

  //****************************
  //  graph Legend
  //  
  //****************************

  public class LegendDescription
  {

    private String _title = "";
    [DisplayName("Text"),
     
     DescriptionAttribute("Legend text")]
    public String title
    {
      get { return _title; }
      set { _title = value; }
    }


    private FontDescription _font = new FontDescription("Arial", 12, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Font"),
     
      ReadOnlyAttribute(true),
     DescriptionAttribute("Legend font")]
     public FontDescription font
     {
       get { return _font; }
       set { _font = value; }
     }

  }




    //****************************
    //  graph Y-axis
    //  
    //****************************

    public class YaxisDescription

  {

    public YaxisDescription(int index , bool shown)
    {
      _visible = shown;
      _showGrid = index == 0;
      _position = index == 0 ? YAxis.HrzPosition.LEFT : YAxis.HrzPosition.RIGHT;
    }

    public string summary
    {
      get { return _visible ? "Enabled" : "Disabled"; }
    }


    private bool _visible = false;
    [DisplayName("Visible"),
      ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should that YAxis be shown?."  )]
    public bool visible
    {
      get { return _visible; }
      set { _visible = value; }
    }

    private YAxis.HrzPosition _position = YAxis.HrzPosition.LEFT;
    [DisplayName("Position"),
     DescriptionAttribute("Y Axis position (left / right)")]
    public YAxis.HrzPosition position
    {
      get { return _position; }
      set { _position = value; }
    }

    private doubleNan _min = new doubleNan(Double.NaN);
    [DisplayName("Minimum value"),
    
     DescriptionAttribute("YAxis minimum value, leave blank for automatic behavior.")]
    public doubleNan min
    {
      get { return _min; }
      set { _min = value; }
    }

    private doubleNan _max = new doubleNan(Double.NaN);
    [DisplayName("Maximum value"),
    
     DescriptionAttribute("YAxis maximum value, leave blank for automatic behavior.")]
    public doubleNan max
    {
      get { return _max; }
      set { _max = value; }
    }

    private doubleNan _step = new doubleNan(Double.NaN);
    [DisplayName("Steps"),
    
     DescriptionAttribute("YAxis step size, leave blank for automatic behavior.")]
    public doubleNan step
    {
      get { return _step; }
      set { _step = value; }
    }

    private YColor _color = YColor.FromArgb(255,127, 127, 127);
    [DisplayName("Color"),
     
    
     DescriptionAttribute("Y Axis Color." )]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private Double _thickness = 1.0;
    [DisplayName("Thickness"),
    
     DescriptionAttribute("Axis thickness")]
    public Double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private bool _showGrid = false;
    [DisplayName("Show Grid"),
     
     DescriptionAttribute("Show grid horizontal lines, or not."  )]
    public bool showGrid { get { return _showGrid; } set { _showGrid = value; } }

    private YColor _gridColor = YColor.FromArgb(255,210, 210, 210);
    [DisplayName("Grid Color"),
    
    
     DescriptionAttribute("Grid horizontal lines color." )]
    public YColor gridColor
    {
      get { return _gridColor; }
      set { _gridColor = value; }
    }

    private Double _gridThickness = 1.0;
    [DisplayName("Grid thickness"),
    
     DescriptionAttribute("Grid horizontal lines thickness")]
    public Double gridThickness
    {
      get { return _gridThickness; }
      set { _gridThickness = value; }
    }

    private FontDescription _font = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, false);
    [DisplayName("Font"),
     
      ReadOnlyAttribute(true),
     DescriptionAttribute("Axis font")]
     public FontDescription font
     {
       get { return _font; }
       set { _font = value; }
     }

    private LegendDescription _legend = new LegendDescription();
    [DisplayName("Legend"),
     
     DescriptionAttribute("Axis legend")]
     public LegendDescription legend
      {  get { return _legend; }
        set { _legend = value; }
      }

    private ZoneDescription _zones0 = new ZoneDescription(0, 50, YColor.fromColor(Color.LightGreen));
    [DisplayName("Zone 1"),
     CategoryAttribute("Zones"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Zone 1 parameters")]
    public ZoneDescription zones0
    {
      get { return _zones0; }
      set { _zones0 = value; }
    }

    private ZoneDescription _zones1 = new ZoneDescription(50, 80, YColor.fromColor(Color.Yellow));
    [DisplayName("Zone 2"),
     CategoryAttribute("Zones"),
     ReadOnlyAttribute(true),
    DescriptionAttribute("Zone 2 parameters")]
    public ZoneDescription zones1
    {
      get { return _zones1; }
      set { _zones1 = value; }
    }

    private ZoneDescription _zones2 = new ZoneDescription(80, 100, YColor.fromColor(Color.Red));
    [DisplayName("Zone 3"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    DescriptionAttribute("Zone 3 parameters")]
    public ZoneDescription zones2
    {
      get { return _zones2; }
      set { _zones2 = value; }
    }


  }


  //****************************
  //  graph X-axis markers
  //  
  //****************************

  public class MarkerDescription
  {

    public MarkerDescription(string defaultText)
    { _text = defaultText;  }

    public string summary
    {
      get { return _enabled ? "Enabled" : "Disabled"; }
    }

    

    private bool _enabled = false;
    [DisplayName("Enabled"),
     ChangeCausesParentRefreshAttribute(true),
     DescriptionAttribute("Should that marker be shown?.")]
    public bool enabled
    {
      get { return _enabled; }
      set { _enabled = value; }
    }


    private String _text = "";
    [DisplayName("Text"),

    DescriptionAttribute("Marker text. Use \\n for multi-line text. Some variables are allowed such as $MARKERTIME$, $LEGEND1$, $VALUE1$, $UNIT1$, $LEGEND1$, $VALUE2$ etc.. Extensive use of marker variables migh make the graph rendering significantly slower.")]
    public String text
    {
      get { return _text; }
      set { _text = value; }
    }

    

    private Marker.TextAlign _textAlign = Marker.TextAlign.CENTER;
    [DisplayName("Text Alignment"),
        DescriptionAttribute("How text is aligned, makes sense on multi-lines text only.")]
    public Marker.TextAlign textAlign { get { return _textAlign; } set { _textAlign = value; } }


    private TimeConverter.TimeReference _timereference = TimeConverter.TimeReference.ABSOLUTE;
    [DisplayName("Time reference"),
     ChangeCausesParentRefreshAttribute(true),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Should the marker time position be absolute or relative to first data timestamp? Note: relative markers won't be drawn until there is actual data.")]
    public TimeConverter.TimeReference timereference
    {
      get { return _timereference; }
      set { _timereference = value; _positionOnXAxis.relative = value == (TimeConverter.TimeReference.RELATIVE);  }
    }

    private xAxisPosition _positionOnXAxis = new xAxisPosition(TimeConverter.ToUnixTime(DateTime.UtcNow), false);
    [DisplayName("Time position"),
     DescriptionAttribute("Marker position on X axis." )]
    public xAxisPosition positionOnXAxis
    {
      get { return _positionOnXAxis; }
      set { _positionOnXAxis = value; timereference = value.relative ? TimeConverter.TimeReference.RELATIVE : TimeConverter.TimeReference.ABSOLUTE; }
    }

    private double _yposition = 95;
    [DisplayName("Vrt position (%)"),
     DescriptionAttribute("Vertical position of the marker label in % of available space. Zero is bottom")]
    public double yposition
    {
      get { return _yposition; }
      set { _yposition = value; }
    }

    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 192);
    [DisplayName("Background color "),
    DescriptionAttribute("Marker  background color." )]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value; } }

    private YColor _borderColor = YColor.fromColor(Color.DarkRed);
    [DisplayName("Border color "),
    DescriptionAttribute("Marker border color." )]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value; } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
     DescriptionAttribute("Marker border thickness, in pixels.")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 5;
    [DisplayName("Padding "),
     DescriptionAttribute("Distance between the marker border and the marker contents, in pixels.")]
    public double padding { get { return _padding; } set { _padding = value; } }


    private FontDescription _font = new FontDescription("Arial", 7, YColor.fromColor(Color.Black), false, false);
    [DisplayName("Font"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Marker Font")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

  }


  //****************************
  //  graph X-axis
  //  
  //****************************

  public class XaxisDescription

  {
    private double _initialZoom = 60;
    [
     DisplayName("Initial Zoom"),
     DescriptionAttribute("Zoom level at application startup, i.e. width of the view-port in seconds.")]
    public double initialZoom
    {
      get { return _initialZoom; }
      set { _initialZoom = value; }
    }

    private double _initialOffset = 0;
    [
     DisplayName("Initial Offset"),
     DescriptionAttribute("Offset of the first data point in percentage of the viewport width. For instance a 50% value will put the first point in the middle of the viewport. This can be used to give some room for datalogger contents.")]
    public double initialOffset
    {
      get { return _initialOffset; }
      set { _initialOffset = value; }
    }



    private XAxis.VrtPosition _position = XAxis.VrtPosition.BOTTOM;
    [DisplayName("Position"),
     DescriptionAttribute("X Axis position (top / bottom)")]
    public XAxis.VrtPosition position
    {
      get { return _position; }
      set { _position = value; }
    }

    private TimeConverter.TimeReference _timeReference = TimeConverter.TimeReference.ABSOLUTE;
    [DisplayName("Time reference"),
    DescriptionAttribute("Are gradation timestamps absolute or relative to experiment start time? ")]
    public TimeConverter.TimeReference timeReference
    {
      get { return _timeReference; }
      set { _timeReference = value; }
    }


    private XAxis.OverflowHandling _overflowHandling = XAxis.OverflowHandling.SCROLL;
    [DisplayName("Overflow Handling"),
     DescriptionAttribute("What to do when new data are about to reach the graph right border")]
    public XAxis.OverflowHandling overflowHandling
    {
      get { return _overflowHandling; }
      set { _overflowHandling = value; }
    }


    private YColor _color = YColor.FromArgb(255, 127, 127, 127);
    [DisplayName("Color"),

     DescriptionAttribute("X Axis Color." )]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private Double _thickness = 1.0;
    [DisplayName("Thickness"),

     DescriptionAttribute("Axis thickness")]
    public Double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private bool _showGrid = false;
    [DisplayName("Show Grid"),
     DescriptionAttribute("Show grid vertical lines, or not."  )]
    public bool showGrid { get { return _showGrid; } set { _showGrid = value; } }

    private YColor _gridColor = YColor.FromArgb(50, 0, 0, 0);
    [DisplayName("Grid Color"),
     
     DescriptionAttribute("Grid vertical lines color." )]
    public YColor gridColor
    {
      get { return _gridColor; }
      set { _gridColor = value; }
    }

    private Double _gridThickness = 1.0;
    [DisplayName("Grid thickness"),
     DescriptionAttribute("Grid vertical lines thickness")]
    public Double gridThickness
    {
      get { return _gridThickness; }
      set { _gridThickness = value; }
    }

    private FontDescription _font = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, false);
    [DisplayName("Font"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Axis font")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

    private LegendDescription _legend = new LegendDescription();
    [DisplayName("Legend"),
      ReadOnlyAttribute(true),
     DescriptionAttribute("X Axis legend")]
    public LegendDescription legend
    {
      get { return _legend; }
      set { _legend = value; }
    }

    private MarkerDescription _markers0 = new MarkerDescription("Marker 1");
    [ DisplayName("Marker 1"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("First marker parameters")]
    public MarkerDescription markers0
    {
      get { return _markers0; }
      set { _markers0 = value; }
    }

    private MarkerDescription _markers1 = new MarkerDescription("Marker 2");
    [DisplayName("Marker 2"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Second marker parameters")]
    public MarkerDescription markers1
    {
      get { return _markers1; }
      set { _markers1 = value; }
    }

    private MarkerDescription _markers2 = new MarkerDescription("Marker 3");
    [DisplayName("Marker 3"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Third marker parameters")]
    public MarkerDescription markers2
    {
      get { return _markers2; }
      set { _markers2 = value; }
    }

    private MarkerDescription _markers3 = new MarkerDescription("Marker 4");
    [DisplayName("Marker 4"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Fourth marker parameters")]
    public MarkerDescription markers3
    {
      get { return _markers3; }
      set { _markers3 = value; }
    }

    private MarkerDescription _markers4 = new MarkerDescription("Marker 5");
    [DisplayName("Marker 5"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Fith marker parameters")]
    public MarkerDescription markers4
    {
      get { return _markers4; }
      set { _markers4 = value; }
    }


    private MarkerDescription _markers5 = new MarkerDescription("Marker 6");
    [DisplayName("Marker 6"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Sixth marker parameters")]
    public MarkerDescription markers5
    {
      get { return _markers5; }
      set { _markers5 = value; }
    }

    private MarkerDescription _markers6 = new MarkerDescription("Marker 7");
    [DisplayName("Marker 7"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Seventh marker parameters")]
    public MarkerDescription markers6
    {
      get { return _markers6; }
      set { _markers6 = value; }
    }

    private MarkerDescription _markers7 = new MarkerDescription("Marker 8");
    [DisplayName("Marker 8"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Markers"),
      DescriptionAttribute("Heighth marker parameters")]
    public MarkerDescription markers7
    {
      get { return _markers7; }
      set { _markers7 = value; }
    }



  }


  //****************************
  //  graph display
  //  
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
        if (name.StartsWith("Graphs_series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(this, null);
          if (!(s.DataSource_source is NullYSensor)) return true;

        }
      }
      return false;

    }



    ChartSerie _Graph_series0 = new ChartSerie(YColor.fromColor(Color.Tomato));
    [
      DisplayName("Series 1"),
      CategoryAttribute("Data Sources"),
      PreExpandedCategoryAttribute(true),
      PreExpandedAttribute(true),
      ReadOnlyAttribute(true),
      DescriptionAttribute("First data series, expand for more.")]
    public ChartSerie Graph_series0
    {
      get { return _Graph_series0; }
      set { _Graph_series0 = value; }
    }

    ChartSerie _Graph_series1 = new ChartSerie(YColor.fromColor(Color.DodgerBlue));
    [
      DisplayName("Series 2"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Second data series, expand for more.")]
    public ChartSerie Graph_series1
    {
      get { return _Graph_series1; }
      set { _Graph_series1 = value; }
    }

    ChartSerie _Graph_series2 = new ChartSerie(YColor.fromColor(Color.SeaGreen));
    [
      DisplayName("Series 3"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Third series, expand for more.")]
    public ChartSerie Graph_series2
    {
      get { return _Graph_series2; }
      set { _Graph_series2 = value; }
    }


    ChartSerie _Graph_series3 = new ChartSerie(YColor.fromColor(Color.Gold));
    [
      DisplayName("Series 4"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Fourth series, expand for more.")]
    public ChartSerie Graph_series3
    {
      get { return _Graph_series3; }
      set { _Graph_series3 = value; }
    }

    // Wanna add a 5th series? just uncomment the section below
   /*
    ChartSerie _Graph_series4 = new ChartSerie(YColor.fromColor(Color.Orange));
    [
      DisplayName("Series 5"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Fifth series, expand for more.")]
    public ChartSerie Graph_series4
    {
      get { return _Graph_series4; }
      set { _Graph_series4 = value; }
    }
   */

    private bool _Graph_showRecordedData = false;
    [
     DisplayName("Use datalogger data"),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Makes the graph show the datalogger contents."  )]
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

    private YColor _Graph_borderColor = YColor.fromColor(Color.LightGray);
    [DisplayName("Border color"),
     CategoryAttribute("Graph"),
    
     DescriptionAttribute("Canvas borders color." )]
    public YColor Graph_borderColor
    {
      get { return _Graph_borderColor; }
      set { _Graph_borderColor = value; }
    }

    private Double _Graph_borderThickness = 1.0;
    [DisplayName("Border thickness"),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Canvas borders thickness.")]
    public Double Graph_borderThickness
    {
      get { return _Graph_borderThickness; }
      set { _Graph_borderThickness = value; }
    }

    private YColor _Graph_bgColor1 = YColor.FromArgb(255, 220, 220, 220);
    [DisplayName("Background color 1"),
     CategoryAttribute("Graph"),
     
     DescriptionAttribute("Background gradient color 1." )]
    public YColor Graph_bgColor1
    {
      get { return _Graph_bgColor1; }
      set { _Graph_bgColor1 = value; }
    }

    private YColor _Graph_bgColor2 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 2"),
     CategoryAttribute("Graph"),
    
     DescriptionAttribute("Background gradient color 2." )]
    public YColor Graph_bgColor2
    {
      get { return _Graph_bgColor2; }
      set { _Graph_bgColor2 = value; }
    }

    private Proportional.ResizeRule _Graph_resizeRule = Proportional.ResizeRule.FIXED;
    [DisplayName("Font sizes"),
     CategoryAttribute("Graph"),
     
     DescriptionAttribute("Are font sizes fixed or do they change when window is resized?")]
    public Proportional.ResizeRule Graph_resizeRule
    {
      get { return _Graph_resizeRule; }
      set { _Graph_resizeRule = value; }
    }

    private XaxisDescription _Graph_xAxis = new XaxisDescription();
    [DisplayName("X Axis"),
     CategoryAttribute("X/Y Axes"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("X-Axis, expand for more")]
    public XaxisDescription Graph_xAxis
    { get { return _Graph_xAxis; }
      set { _Graph_xAxis = value; }
    }

    private YaxisDescription _Graph_yAxes0 = new YaxisDescription(0, true);
    [
      DisplayName("YAxis 1"),
      ReadOnlyAttribute(true),
      CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("First Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes0
    {
      get { return _Graph_yAxes0; }
      set { _Graph_yAxes0 = value; }
    }


    private YaxisDescription _Graph_yAxes1 = new YaxisDescription(1, false);
    [
      DisplayName("YAxis 2"),
      ReadOnlyAttribute(true),
     CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("Second Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes1
    {
      get { return _Graph_yAxes1; }
      set { _Graph_yAxes1 = value; }
    }

    private YaxisDescription _Graph_yAxes2 = new YaxisDescription(2, false);
    [
      DisplayName("YAxis 3"),
      ReadOnlyAttribute(true),
      CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("Third Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes2
    {
      get { return _Graph_yAxes2; }
      set { _Graph_yAxes2 = value; }
    }

    private LegendPanelDescription _Graph_legendPanel = new LegendPanelDescription();
    [
      DisplayName("Legend Panel"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Graph"),
      DescriptionAttribute("Panel containing a description of all data series. Expand for more.")]
    public LegendPanelDescription Graph_legendPanel
    {
      get { return _Graph_legendPanel; }
      set { _Graph_legendPanel = value; }
    }

    private DataTrackerDescription _Graph_dataTracker = new DataTrackerDescription();
    [
      DisplayName("Data tracker"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Graph"),
      DescriptionAttribute("Show dynamic value labels while the mouse is moving over the graph. Expand for more.")]
    public DataTrackerDescription Graph_dataTracker
    {
      get { return _Graph_dataTracker; }
      set { _Graph_dataTracker = value; }
    }

    private NavigatorDescription _Graph_navigator = new NavigatorDescription();
    [
      DisplayName("Navigator"),
      CategoryAttribute("Graph"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Small additional graph showing the whole data set and allowing to quickly navigate among data. Expand for more.")]
    public NavigatorDescription Graph_navigator
    {
      get { return _Graph_navigator; }
     set { _Graph_navigator = value; }
    }

    private AnnotationPanelDescription _annotationPanels0 = new AnnotationPanelDescriptionGraph();
    [
     DisplayName("Annotation 1"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panel 1")]
     public AnnotationPanelDescription Graph_annotationPanels0
    {
      get { return _annotationPanels0; }
      set { _annotationPanels0 = value; }
     }

    private AnnotationPanelDescription _annotationPanels1 = new AnnotationPanelDescriptionGraph();
    [
     DisplayName("Annotation 2"),
     CategoryAttribute("Annotations"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("Customizable text panel 2")]
     public AnnotationPanelDescription Graph_annotationPanels1
    {
       get { return _annotationPanels1; }
       set { _annotationPanels1 = value; }
     }




  }

}
